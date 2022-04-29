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

        public MainWindow()
        {
            this.InitializeComponent();
            hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            //hWndChild = FindWindowEx(hWnd, IntPtr.Zero, "Microsoft.UI.Content.ContentWindowSiteBridge", null);
            //hWnd = hWndChild;

            HRESULT hr = CaptureVideo();

            SubClassDelegate = new SUBCLASSPROC(WindowSubClass);
            bool bRet = SetWindowSubclass(hWnd, SubClassDelegate, 0, 0);

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
                                    hr = g_pWC.SetVideoClippingWindow(hWnd);
                                    //hr = pWC.SetBorderColor((uint)ColorTranslator.ToWin32(System.Drawing.Color.Red));
                                    //RECT rcSrc = new RECT(0, 0, 0, 0);
                                    RECT rcDest = new RECT(nXCaptureWindow, nYCaptureWindow, nWidthCaptureWindow + nXCaptureWindow, nHeightCaptureWindow + nYCaptureWindow);
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
    }        
}
