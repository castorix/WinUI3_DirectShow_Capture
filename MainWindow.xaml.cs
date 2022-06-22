using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

using DirectShow;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

// Reference : https://github.com/pauldotknopf/WindowsSDK7-Samples/tree/master/multimedia/directshow/capture/playcap


namespace WinUI3_DirectShow_Capture
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool FillRect(IntPtr hdc, [In] ref RECT rect, IntPtr hbrush);

        [DllImport("gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport("gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int ExcludeClipRect(IntPtr hdc, int left, int top, int right, int bottom);

        public const int WM_SIZE = 0x0005;
        public const int WM_PAINT = 0x000F;
        public const int WM_ERASEBKGND = 0x0014;

        public delegate int SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern int DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        public const int SPI_GETWORKAREA = 0x30;

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, [In, Out] ref RECT pvParam, uint fWinIni);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_NOREDRAW = 0x0008;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_FRAMECHANGED = 0x0020;  /* The frame changed: send WM_NCCALCSIZE */
        public const int SWP_SHOWWINDOW = 0x0040;
        public const int SWP_HIDEWINDOW = 0x0080;
        public const int SWP_NOCOPYBITS = 0x0100;
        public const int SWP_NOOWNERZORDER = 0x0200;  /* Don't do owner Z ordering */
        public const int SWP_NOSENDCHANGING = 0x0400;  /* Don't send WM_WINDOWPOSCHANGING */
        public const int SWP_DRAWFRAME = SWP_FRAMECHANGED;
        public const int SWP_NOREPOSITION = SWP_NOOWNERZORDER;
        public const int SWP_DEFERERASE = 0x2000;
        public const int SWP_ASYNCWINDOWPOS = 0x4000;

        [StructLayout(LayoutKind.Sequential)]
        public class BITMAPINFOHEADER
        {
            public int biSize = 40;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(int dwExStyle, string lpClassName, string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        public const int WS_OVERLAPPED = 0x00000000,
            WS_POPUP = unchecked((int)0x80000000),
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_CAPTION = 0x00C00000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_TABSTOP = 0x00010000,
            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED |
                             WS_CAPTION |
                             WS_SYSMENU |
                             WS_THICKFRAME |
                             WS_MINIMIZEBOX |
                             WS_MAXIMIZEBOX;

        public const int WS_EX_DLGMODALFRAME = 0x00000001;
        public const int WS_EX_NOPARENTNOTIFY = 0x00000004;
        public const int WS_EX_TOPMOST = 0x00000008;
        public const int WS_EX_ACCEPTFILES = 0x00000010;
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WS_EX_MDICHILD = 0x00000040;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_WINDOWEDGE = 0x00000100;
        public const int WS_EX_CLIENTEDGE = 0x00000200;
        public const int WS_EX_CONTEXTHELP = 0x00000400;
        public const int WS_EX_RIGHT = 0x00001000;
        public const int WS_EX_LEFT = 0x00000000;
        public const int WS_EX_RTLREADING = 0x00002000;
        public const int WS_EX_LTRREADING = 0x00000000;
        public const int WS_EX_LEFTSCROLLBAR = 0x00004000;
        public const int WS_EX_RIGHTSCROLLBAR = 0x00000000;
        public const int WS_EX_CONTROLPARENT = 0x00010000;
        public const int WS_EX_STATICEDGE = 0x00020000;
        public const int WS_EX_APPWINDOW = 0x00040000;
        public const int WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const int WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        public const int WS_EX_LAYERED = 0x00080000;
        public const int WS_EX_NOINHERITLAYOUT = 0x00100000; // Disable inheritence of mirroring by children
        public const int WS_EX_NOREDIRECTIONBITMAP = 0x00200000;
        public const int WS_EX_LAYOUTRTL = 0x00400000; // Right to left mirroring
        public const int WS_EX_COMPOSITED = 0x02000000;
        public const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetSystemMetrics(int nIndex);

        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int iMode);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SelectClipRgn(IntPtr hdc, IntPtr hrgn);

        public const int RGN_AND = 1;
        public const int RGN_OR = 2;
        public const int RGN_XOR = 3;
        public const int RGN_DIFF = 4;
        public const int RGN_COPY = 5;
        public const int RGN_MIN = RGN_AND;
        public const int RGN_MAX = RGN_COPY;

        public const int ERROR = 0;
        public const int NULLREGION = 1;
        public const int SIMPLEREGION = 2;
        public const int COMPLEXREGION = 3;

        public const int WM_APP = 0x8000;


        IntPtr hWnd = IntPtr.Zero;
        IntPtr hWndChild = IntPtr.Zero;
        private Microsoft.UI.Windowing.AppWindow _apw;

        IVideoWindow g_pVW = null;
        IMediaControl g_pMC = null;
        IMediaEventEx g_pME = null;
        IGraphBuilder g_pGraph = null;
        ICaptureGraphBuilder2 g_pCapture = null;
        private enum PLAYSTATE { Stopped, Paused, Running, Init };
        //PLAYSTATE g_psCurrent = PLAYSTATE.Stopped;
        private const int WM_GRAPHNOTIFY = WM_APP + 1;
        IVMRMixerBitmap9 g_pMP = null;
        bool g_bOverLay = false;

        private SUBCLASSPROC SubClassDelegate;

        private int nXCaptureWindow = 10, nYCaptureWindow = 10, nWidthCaptureWindow = 640, nHeightCaptureWindow = 480;
        IVMRWindowlessControl9 g_pWC = null;

        IntPtr m_hWndContainer = IntPtr.Zero;

        public MainWindow()
        {
            this.InitializeComponent();
            hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // For 1.1.0 release 
            hWndChild = FindWindowEx(hWnd, IntPtr.Zero, "Microsoft.UI.Content.ContentWindowSiteBridge", null);
            //hWnd = hWndChild;
            //m_hWndContainer = CreateWindowEx(WS_EX_TRANSPARENT | WS_EX_LAYERED, "Static", "", WS_VISIBLE | WS_CHILD, nXCaptureWindow, nYCaptureWindow, nWidthCaptureWindow, nHeightCaptureWindow, hWnd, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            m_hWndContainer = CreateWindowEx(0, "Static", "", WS_VISIBLE | WS_CHILD, nXCaptureWindow, nYCaptureWindow, nWidthCaptureWindow, nHeightCaptureWindow, hWnd, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            RECT rect = new RECT(nXCaptureWindow, nYCaptureWindow, nWidthCaptureWindow, nHeightCaptureWindow);
            SetRegion(hWndChild, true, ref rect);

            HRESULT hr = CaptureVideo();

            //SubClassDelegate = new SUBCLASSPROC(WindowSubClass);
            //bool bRet = SetWindowSubclass(hWnd, SubClassDelegate, 0, 0);

            Microsoft.UI.WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            _apw = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(myWndId);
            _apw.Resize(new Windows.Graphics.SizeInt32(nWidthCaptureWindow + 190, nHeightCaptureWindow*2 + 60));
            CenterToScreen(hWnd);
        }

        private async void Click()
        {
            StackPanel sp = new StackPanel();
            FontIcon fi = new FontIcon()
            {
                FontFamily = new FontFamily("Segoe UI Emoji"),
                Glyph = "\U0001F42F",
                FontSize = 50
            };
            sp.Children.Add(fi);
            TextBlock tb = new TextBlock();
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.Text = "You clicked on the Button !";
            sp.Children.Add(tb);
            ContentDialog cd = new ContentDialog()
            {
                Title = "Information",
                Content = sp,                
                CloseButtonText = "Ok"
            };
            cd.XamlRoot = this.Content.XamlRoot;
            var res = await cd.ShowAsync();
        }

        private async void GrabImage()
        {
            IntPtr pDIB = IntPtr.Zero;
            HRESULT hr = g_pWC.GetCurrentImage(out pDIB);
            if (hr == HRESULT.S_OK)
            {
                BITMAPINFOHEADER bih = new BITMAPINFOHEADER();
                Marshal.PtrToStructure(pDIB, bih);

                int nSize = bih.biWidth * bih.biHeight * 4;
                byte[] pManagedArray = new byte[nSize];
                Marshal.Copy(new IntPtr(pDIB.ToInt32() + 40), pManagedArray, 0, nSize);

                // Test negative image
                //for (int i = 0; i < pManagedArray.Length; ++i)
                //{
                //    unchecked
                //    {
                //        pManagedArray[i] ^= (byte)(0x00FFFFFF);
                //    }
                //}

                Microsoft.UI.Xaml.Media.Imaging.WriteableBitmap wb = new Microsoft.UI.Xaml.Media.Imaging.WriteableBitmap(bih.biWidth, bih.biHeight);
                await wb.PixelBuffer.AsStream().WriteAsync(pManagedArray, 0, pManagedArray.Length);
                image1.Source = wb;

                //   <Image Source = "Assets/butterfly.png" RenderTransformOrigin = "0.5,0.5" >   
                //     <Image.RenderTransform>   
                //       <ScaleTransform ScaleY = "-1" > </ScaleTransform>    
                //     </Image.RenderTransform>
                //   </Image>

                image1.RenderTransformOrigin = new Point(0.5, 0.5);
                ScaleTransform scaleTransform1 = new ScaleTransform();
                //myScaleTransform.ScaleX = -1;
                scaleTransform1.ScaleY = -1;
                TransformGroup transformGroup1 = new TransformGroup();
                transformGroup1.Children.Add(scaleTransform1);
                image1.RenderTransform = transformGroup1;
            }
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            //myButton.Content = "Clicked";
            //Click();
            GrabImage();
        }

        private HRESULT CaptureVideo()
        {
            HRESULT hr = HRESULT.E_FAIL;
            IBaseFilter pSrcFilter = null;
            hr = GetInterfaces();
            if (hr == HRESULT.S_OK)
            {
                hr = g_pCapture.SetFiltergraph(g_pGraph);
                if (hr == HRESULT.S_OK)
                {
                    hr = FindCaptureDevice(ref pSrcFilter);
                    if (hr == HRESULT.S_OK)
                    {
                        IntPtr pSrcFilterPtr = Marshal.GetIUnknownForObject(pSrcFilter);
                        hr = g_pGraph.AddFilter(pSrcFilter, "Video Capture");
                        if (hr == HRESULT.S_OK)
                        {
                            IBaseFilter pVideoMixingRenderer9 = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(DirectShowTools.CLSID_VideoMixingRenderer9));
                            hr = g_pGraph.AddFilter(pVideoMixingRenderer9, "Video Mixing Renderer 9");
                            if (hr == HRESULT.S_OK)
                            {
                                IVMRFilterConfig9 pConfig = null;
                                pConfig = (IVMRFilterConfig9)pVideoMixingRenderer9;
                                hr = pConfig.SetRenderingMode((uint)VMR9Mode.VMR9Mode_Windowless);
                                //IVMRWindowlessControl9 pWC = null;
                                g_pWC = (IVMRWindowlessControl9)pVideoMixingRenderer9;
                                if (g_pWC != null)
                                {
                                    hr = g_pWC.SetVideoClippingWindow(m_hWndContainer);
                                    //hr = pWC.SetBorderColor((uint)ColorTranslator.ToWin32(System.Drawing.Color.Red));
                                    //RECT rcSrc = new RECT(0, 0, 0, 0);
                                    //RECT rcDest = new RECT(nXCaptureWindow, nYCaptureWindow, nWidthCaptureWindow + nXCaptureWindow, nHeightCaptureWindow + nYCaptureWindow);
                                    RECT rcDest = new RECT(0, 0, nWidthCaptureWindow, nHeightCaptureWindow);
                                    //RECT rcDest = new RECT(0, 0, 0, 0);
                                    //hr = pWC.SetVideoPosition(ref rcSrc, ref rcDest);
                                    hr = g_pWC.SetVideoPosition(IntPtr.Zero, ref rcDest);
                                    //hr = pWC.SetVideoPosition(IntPtr.Zero, IntPtr.Zero);                                   
                                }

                                g_pMP = (IVMRMixerBitmap9)pVideoMixingRenderer9;
                                hr = g_pCapture.RenderStream(DirectShowTools.PIN_CATEGORY_PREVIEW, DirectShowTools.MEDIATYPE_Video, pSrcFilterPtr, null, pVideoMixingRenderer9);
                                //Marshal.ReleaseComObject(pVideoMixingRenderer9);
                            }
                            hr = g_pMC.Run();
                            //g_psCurrent = PLAYSTATE.Running;
                        }
                        Marshal.ReleaseComObject(pSrcFilter);
                    }
                }
            }
            return hr;
        }

        private HRESULT GetInterfaces()
        {
            HRESULT hr = HRESULT.E_FAIL;            
            Type FilterGraphType = Type.GetTypeFromCLSID(DirectShowTools.CLSID_FilterGraph, true);
            object FilterGraph = Activator.CreateInstance(FilterGraphType);
            g_pGraph = (IGraphBuilder)FilterGraph;
            g_pMC = (IMediaControl)FilterGraph;
            g_pVW = (IVideoWindow)FilterGraph;
            g_pME = (IMediaEventEx)FilterGraph;
            if (g_pME != null)
                hr = g_pME.SetNotifyWindow(hWnd, WM_GRAPHNOTIFY, IntPtr.Zero);
            if (hr == HRESULT.S_OK)
            {
                Type CaptureGraphBuilder2Type = Type.GetTypeFromCLSID(DirectShowTools.CLSID_CaptureGraphBuilder2, true);
                object CaptureGraphBuilder2 = Activator.CreateInstance(CaptureGraphBuilder2Type);
                g_pCapture = (ICaptureGraphBuilder2)CaptureGraphBuilder2;
                hr = (g_pCapture != null) ? HRESULT.S_OK : HRESULT.E_FAIL;               
            }
            return hr;
        }

        private void btnOverlay_Click(object sender, RoutedEventArgs e)
        {
            HRESULT hr = HRESULT.E_FAIL;
            if (!g_bOverLay)
            {
                // Draw Red rectangle
                int nWidth = 200;
                int nHeight = 100;
                IntPtr hDC = GetDC(IntPtr.Zero);
                IntPtr hDCMem = CreateCompatibleDC(hDC);
                IntPtr hBitmap = CreateCompatibleBitmap(hDC, nWidth, nHeight);
                IntPtr hBitmapOld = SelectObject(hDCMem, hBitmap);
                RECT rc = new RECT(0, 0, nWidth, nHeight);
                IntPtr hBrush = CreateSolidBrush(System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.Red));
                FillRect(hDCMem, ref rc, hBrush);
                DeleteObject(hBrush);

                VMR9AlphaBitmap alphaBitmap = new VMR9AlphaBitmap();
                alphaBitmap.dwFlags = (uint)VMRBITMAP.VMRBITMAP_HDC;
                alphaBitmap.hdc = hDCMem;
                alphaBitmap.rSrc = new RECT(0, 0, nWidth, nHeight);
                alphaBitmap.rDest.left = 0.3F;
                alphaBitmap.rDest.right = 0.7F;
                alphaBitmap.rDest.top = 0.3F;
                alphaBitmap.rDest.bottom = 0.7F;
                alphaBitmap.clrSrcKey = System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.White);
                alphaBitmap.dwFlags |= (uint)VMRBITMAP.VMRBITMAP_SRCCOLORKEY;
                alphaBitmap.fAlpha = 0.6F;
                hr = g_pMP.SetAlphaBitmap(alphaBitmap);

                SelectObject(hDCMem, hBitmapOld);
                DeleteObject(hBitmap);
                DeleteDC(hDCMem);
                ReleaseDC(IntPtr.Zero, hDC);
                g_bOverLay = true;
                btnOverlay.Content = "Remove Overlay";
            }
            else
            {
                VMR9AlphaBitmap alphaBitmap = new VMR9AlphaBitmap();
                alphaBitmap.dwFlags = (uint)VMRBITMAP.VMRBITMAP_DISABLE;
                hr = g_pMP.SetAlphaBitmap(alphaBitmap);
                g_bOverLay = false;
                btnOverlay.Content = "Set Overlay";
            }
        }

        private HRESULT FindCaptureDevice(ref IBaseFilter ppSrcFilter)
        {
            HRESULT hr = HRESULT.E_FAIL;
            IBaseFilter pSrc = null;
           
            Type CreateDevEnumType = Type.GetTypeFromCLSID(DirectShowTools.CLSID_SystemDeviceEnum, true);
            object CreateDevEnum = Activator.CreateInstance(CreateDevEnumType);
            ICreateDevEnum pCreateDevEnum = (ICreateDevEnum)CreateDevEnum;
            IEnumMoniker pEm;
            hr = pCreateDevEnum.CreateClassEnumerator(DirectShowTools.CLSID_VideoInputDeviceCategory, out pEm, 0);
            if (hr == HRESULT.S_OK && pEm != null)
            {
                uint cFetched;
                IMoniker pMoniker = null;
                while ((hr = pEm.Next(1, out pMoniker, out cFetched)) == HRESULT.S_OK && cFetched > 0)
                {
                    IntPtr pBag = IntPtr.Zero;
                    if (pMoniker != null)
                    {
                        hr = pMoniker.BindToStorage(IntPtr.Zero, null, typeof(IPropertyBag).GUID, out pBag);
                        if (hr == HRESULT.S_OK)
                        {
                            IPropertyBag pPropertyBag = Marshal.GetObjectForIUnknown(pBag) as IPropertyBag;
                            PROPVARIANT var;
                            var.varType = (ushort)VARENUM.VT_BSTR;
                            // ou CLSID
                            hr = pPropertyBag.Read("FriendlyName", out var, null);
                            string sString = Marshal.PtrToStringUni(var.pwszVal);
                            Marshal.Release(pBag);
                            System.Diagnostics.Debug.WriteLine("Video Capture device name : " + sString);
                            this.Title = "Video Capture device name : " + sString;                           
                             //Console.WriteLine("Name = {0}", sString);

                            IntPtr pBaseFilterPtr = IntPtr.Zero;
                            hr = pMoniker.BindToObject(IntPtr.Zero, null, typeof(IBaseFilter).GUID, ref pBaseFilterPtr);
                            if (hr == HRESULT.S_OK)
                            {
                                pSrc = Marshal.GetObjectForIUnknown(pBaseFilterPtr) as IBaseFilter;
                                ppSrcFilter = pSrc;
                                Marshal.ReleaseComObject(pMoniker);
                                break;
                            }
                        }
                        Marshal.ReleaseComObject(pMoniker);
                    }
                    else break;
                }
                Marshal.ReleaseComObject(pEm);
            }
            return hr;
        }

        private int WindowSubClass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData)
        {
            switch (uMsg)
            {
                case WM_ERASEBKGND:
                    {
                        RECT rect;
                        GetClientRect(hWnd, out rect);

                        int nRet = ExcludeClipRect(wParam, nXCaptureWindow, nYCaptureWindow, nWidthCaptureWindow + nXCaptureWindow, nHeightCaptureWindow + nYCaptureWindow);

                        //IntPtr hBrush = CreateSolidBrush(System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.Red));
                        //IntPtr hBrush = CreateSolidBrush(System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.FromArgb(255, 32, 32, 32)));
                        IntPtr hBrush = CreateSolidBrush(System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.Black));
                        FillRect(wParam, ref rect, hBrush);
                        DeleteObject(hBrush);
                        return 1;
                    }
                    break;
            }
            return DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }

        private void CenterToScreen(IntPtr hWnd)
        {
            RECT rcWorkArea = new RECT();
            SystemParametersInfo(SPI_GETWORKAREA, 0, ref rcWorkArea, 0);
            RECT rc;
            GetWindowRect(hWnd, out rc);
            int nX = System.Convert.ToInt32((rcWorkArea.left + rcWorkArea.right) / (double)2 - (rc.right - rc.left) / (double)2);
            int nY = System.Convert.ToInt32((rcWorkArea.top + rcWorkArea.bottom) / (double)2 - (rc.bottom - rc.top) / (double)2);
            //SetWindowPos(hWnd, IntPtr.Zero, nX, nY, -1, -1, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
            SetWindowPos(hWnd, IntPtr.Zero, nX, nY, -1, -1, SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

        private void SetRegion(IntPtr hWnd, bool bRegion, ref RECT rect)
        {
            if (bRegion)
            {
                RECT rc;
                GetClientRect(hWnd, out rc);
                int nScreenWidth = GetSystemMetrics(SM_CXSCREEN);
                int nScreenHeight = GetSystemMetrics(SM_CYSCREEN);
                IntPtr WindowRgn = CreateRectRgn(0, 0, nScreenWidth, nScreenHeight);
                IntPtr HoleRgn = CreateRectRgn(rect.left, rect.top, rect.right, rect.bottom);
                CombineRgn(WindowRgn, WindowRgn, HoleRgn, RGN_DIFF);
                SetWindowRgn(hWnd, WindowRgn, true);
                DeleteObject(HoleRgn);
            }
            else
                SetWindowRgn(hWnd, IntPtr.Zero, true);
        }
    }        
}
