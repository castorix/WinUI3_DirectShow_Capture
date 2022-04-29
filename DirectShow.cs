using System;
using System.Runtime.InteropServices;

namespace DirectShow
{
    internal class DirectShowTools
    {
        public static Guid CLSID_FilterGraph = new Guid("E436EBB3-524F-11CE-9F53-0020AF0BA770");
        public static Guid CLSID_CaptureGraphBuilder2 = new Guid("BF87B6E1-8C27-11d0-B3F0-00AA003761C5");
        public static Guid CLSID_VideoMixingRenderer9 = new Guid("{51B4ABF3-748F-4E3B-A276-C828330E926A}");
        public static Guid CLSID_SystemDeviceEnum = new Guid("62BE5D10-60EB-11D0-BD3B-00A0C911CE86");
        public static Guid CLSID_VideoInputDeviceCategory = new Guid("860BB310-5D01-11D0-BD3B-00A0C911CE86");
        public static Guid CLSID_AudioRendererCategory = new Guid("E0F158E1-CB04-11D0-BD4E-00A0C911CE86");      
        public static Guid PIN_CATEGORY_PREVIEW = new Guid("fb6c4282-0353-11d1-905f-0000c0cc16ba");
        public static Guid MEDIATYPE_Video = new Guid("73646976-0000-0010-8000-00AA00389B71");       
    }

    public enum HRESULT : int
    {
        S_OK = 0,
        S_FALSE = 1,
        E_NOTIMPL = unchecked((int)0x80004001),
        E_NOINTERFACE = unchecked((int)0x80004002),
        E_POINTER = unchecked((int)0x80004003),
        E_FAIL = unchecked((int)0x80004005),
        E_UNEXPECTED = unchecked((int)0x8000FFFF),
        E_OUTOFMEMORY = unchecked((int)0x8007000E),
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
        public RECT(int Left, int Top, int Right, int Bottom)
        {
            left = Left;
            top = Top;
            right = Right;
            bottom = Bottom;
        }
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("56a868a9-0ad4-11ce-b03a-0020af0ba770")]
    public interface IGraphBuilder : IFilterGraph
    {
        #region IFilterGraph
        new HRESULT AddFilter(IBaseFilter pFilter, string pName);
        new HRESULT RemoveFilter(IBaseFilter pFilter);
        new HRESULT EnumFilters(out IEnumFilters ppEnum);
        new HRESULT FindFilterByName(string pName, out IBaseFilter ppFilter);
        //new HRESULT ConnectDirect(IPin ppinOut, IPin ppinIn, AM_MEDIA_TYPE pmt);
        new HRESULT ConnectDirect(IPin ppinOut, IPin ppinIn, IntPtr pmt);
        new HRESULT Reconnect(IPin ppin);
        new HRESULT Disconnect(IPin ppin);
        new HRESULT SetDefaultSyncSource();
        #endregion

        HRESULT Connect(IPin ppinOut, IPin ppinIn);
        HRESULT Render(IPin ppinOut);
        HRESULT RenderFile(string lpcwstrFile, string lpcwstrPlayList);
        HRESULT AddSourceFilter(string lpcwstrFileName, string lpcwstrFilterName, out IBaseFilter ppFilter);
        HRESULT SetLogFile(IntPtr hFile);
        HRESULT Abort();
        HRESULT ShouldOperationContinue();
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("36b73882-c2c8-11cf-8b46-00805f6cef60")]
    public interface IFilterGraph2 : IGraphBuilder
    {
        #region IGraphBuilder
        #region IFilterGraph
        new HRESULT AddFilter(IBaseFilter pFilter, string pName);
        new HRESULT RemoveFilter(IBaseFilter pFilter);
        new HRESULT EnumFilters(out IEnumFilters ppEnum);
        new HRESULT FindFilterByName(string pName, out IBaseFilter ppFilter);
        //new HRESULT ConnectDirect(IPin ppinOut, IPin ppinIn, AM_MEDIA_TYPE pmt);
        new HRESULT ConnectDirect(IPin ppinOut, IPin ppinIn, IntPtr pmt);
        new HRESULT Reconnect(IPin ppin);
        new HRESULT Disconnect(IPin ppin);
        new HRESULT SetDefaultSyncSource();
        #endregion

        new HRESULT Connect(IPin ppinOut, IPin ppinIn);
        new HRESULT Render(IPin ppinOut);
        new HRESULT RenderFile(string lpcwstrFile, string lpcwstrPlayList);
        new HRESULT AddSourceFilter(string lpcwstrFileName, string lpcwstrFilterName, out IBaseFilter ppFilter);
        new HRESULT SetLogFile(IntPtr hFile);
        new HRESULT Abort();
        new HRESULT ShouldOperationContinue();
        #endregion

        //HRESULT AddSourceFilterForMoniker(IMoniker pMoniker, IBindCtx pCtx, string lpcwstrFilterName, out IBaseFilter ppFilter);
        HRESULT AddSourceFilterForMoniker(IMoniker pMoniker, IntPtr pCtx, string lpcwstrFilterName, out IBaseFilter ppFilter);
        HRESULT ReconnectEx(IPin ppin, AM_MEDIA_TYPE pmt);
        HRESULT RenderEx(IPin pPinOut, uint dwFlags, ref uint pvContext);
    }

    public enum AM_RENSDEREXFLAGS
    {
        AM_RENDEREX_RENDERTOEXISTINGRENDERERS = 0x1
    };

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("56a8689f-0ad4-11ce-b03a-0020af0ba770")]
    public interface IFilterGraph
    {
        HRESULT AddFilter(IBaseFilter pFilter, string pName);
        HRESULT RemoveFilter(IBaseFilter pFilter);
        HRESULT EnumFilters(out IEnumFilters ppEnum);
        HRESULT FindFilterByName(string pName, out IBaseFilter ppFilter);
        //HRESULT ConnectDirect(IPin ppinOut, IPin ppinIn, AM_MEDIA_TYPE pmt);
        HRESULT ConnectDirect(IPin ppinOut, IPin ppinIn, IntPtr pmt);
        HRESULT Reconnect(IPin ppin);
        HRESULT Disconnect(IPin ppin);
        HRESULT SetDefaultSyncSource();
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("56a86893-0ad4-11ce-b03a-0020af0ba770")]
    public interface IEnumFilters
    {
        HRESULT Next(uint cFilters, out IBaseFilter ppFilter, out uint pcFetched);
        HRESULT Skip(uint cFilters);
        HRESULT Reset();
        HRESULT Clone(out IEnumFilters ppEnum);
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("0000010c-0000-0000-C000-000000000046")]
    public interface IPersist
    {
        HRESULT GetClassID(out Guid pClassID);
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("56a86899-0ad4-11ce-b03a-0020af0ba770")]
    public interface IMediaFilter : IPersist
    {
        #region IPersist
        new HRESULT GetClassID(out Guid pClassID);
        #endregion
        HRESULT Stop();
        HRESULT Pause();
        HRESULT Run(Int64 tStart);
        HRESULT GetState(int dwMilliSecsTimeout, out FILTER_STATE State);
        HRESULT SetSyncSource(IntPtr pClock);
        HRESULT GetSyncSource(out IntPtr pClock);
        //HRESULT SetSyncSource(IReferenceClock pClock);
        //HRESULT GetSyncSource(out IReferenceClock pClock);
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("56a86895-0ad4-11ce-b03a-0020af0ba770")]
    public interface IBaseFilter : IMediaFilter
    {
        #region IMediaFilter
        #region IPersist
        new HRESULT GetClassID(out Guid pClassID);
        #endregion
        new HRESULT Stop();
        new HRESULT Pause();
        new HRESULT Run(Int64 tStart);
        new HRESULT GetState(int dwMilliSecsTimeout, out FILTER_STATE State);
        new HRESULT SetSyncSource(IntPtr pClock);
        new HRESULT GetSyncSource(out IntPtr pClock);
        #endregion

        HRESULT EnumPins(out IEnumPins ppEnum);
        HRESULT FindPin(string Id, out IPin ppPin);
        HRESULT QueryFilterInfo(out FILTER_INFO pInfo);
        HRESULT JoinFilterGraph(IFilterGraph pGraph, string pName);
        HRESULT QueryVendorInfo(out string pVendorInfo);
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("56a86892-0ad4-11ce-b03a-0020af0ba770")]
    public interface IEnumPins
    {
        HRESULT Next(uint cPins, out IPin ppPins, out uint pcFetched);
        HRESULT Skip(uint cPins);
        HRESULT Reset();
        HRESULT Clone(out IEnumPins ppEnum);
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("56a86891-0ad4-11ce-b03a-0020af0ba770")]
    public interface IPin
    {
        HRESULT Connect(IPin pReceivePin, AM_MEDIA_TYPE pmt);
        HRESULT ReceiveConnection(IPin pConnector, AM_MEDIA_TYPE pmt);
        HRESULT Disconnect();
        HRESULT ConnectedTo(out IPin pPin);
        HRESULT ConnectionMediaType(out AM_MEDIA_TYPE pmt);
        HRESULT QueryPinInfo(out PIN_INFO pInfo);
        HRESULT QueryDirection(out PIN_DIRECTION pPinDir);
        HRESULT QueryId(out string Id);
        HRESULT QueryAccept(AM_MEDIA_TYPE pmt);
        HRESULT EnumMediaTypes(out IEnumMediaTypes ppEnum);
        HRESULT QueryInternalConnections(out IPin apPin, ref uint nPin);
        HRESULT EndOfStream();
        HRESULT BeginFlush();
        HRESULT EndFlush();
        HRESULT NewSegment(Int64 tStart, Int64 tStop, double dRate);
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("89c31040-846b-11ce-97d3-00aa0055595a")]
    public interface IEnumMediaTypes
    {
        HRESULT Next(uint cMediaTypes, out AM_MEDIA_TYPE ppMediaTypes, out uint pcFetched);
        HRESULT Skip(uint cMediaTypes);
        HRESULT Reset();
        HRESULT Clone(out IEnumMediaTypes ppEnum);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct AM_MEDIA_TYPE
    {
        public Guid majortype;
        public Guid subtype;
        public bool bFixedSizeSamples;
        public bool bTemporalCompression;
        public uint lSampleSize;
        public Guid formattype;
        // public pUnk As IUnknown
        public IntPtr pUnk;
        public uint cbFormat;
        public byte pbFormat;
    }

    public enum PIN_DIRECTION : int
    {
        PINDIR_INPUT = 0,
        PINDIR_OUTPUT = (PINDIR_INPUT + 1)
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class PIN_INFO
    {
        public IBaseFilter filter;
        public PIN_DIRECTION dir;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string name;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class FILTER_INFO
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string achName;
        [MarshalAs(UnmanagedType.IUnknown)] // IFilterGraph
        public object pUnk;
    }

    public enum FILTER_STATE
    {
        State_Stopped = 0,
        State_Paused = (State_Stopped + 1),
        State_Running = (State_Paused + 1)
    }

    [ComImport()]
    [Guid("56A868B1-0AD4-11CE-B03A-0020AF0BA770")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaControl
    {
        HRESULT Run();
        HRESULT Pause();
        HRESULT Stop();
        HRESULT GetState(int msTimeout, out int pfs);
        HRESULT RenderFile(string strFilename);
        HRESULT AddSourceFilter(string strFilename, out object ppUnk);
        HRESULT get_FilterCollection(out object ppUnk);
        HRESULT get_RegFilterCollection(out object ppUnk);
        //HRESULT AddSourceFilter(string strFilename, out IDispatch ppUnk);
        //HRESULT get_FilterCollection(out IDispatch ppUnk);
        //HRESULT get_RegFilterCollection(out IDispatch ppUnk);
        HRESULT StopWhenReady();
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("56a868b4-0ad4-11ce-b03a-0020af0ba770")]
    public interface IVideoWindow
    {
        HRESULT put_Caption(string strCaption);
        HRESULT get_Caption(out string strCaption);
        HRESULT put_WindowStyle(int WindowStyle);
        HRESULT get_WindowStyle(out int WindowStyle);
        HRESULT put_WindowStyleEx(int WindowStyleEx);
        HRESULT get_WindowStyleEx(out int WindowStyleEx);
        HRESULT put_AutoShow(int AutoShow);
        HRESULT get_AutoShow(out int AutoShow);
        HRESULT put_WindowState(int WindowState);
        HRESULT get_WindowState(out int WindowState);
        HRESULT put_BackgroundPalette(int BackgroundPalette);
        HRESULT get_BackgroundPalette(out int pBackgroundPalette);
        [PreserveSig]
        HRESULT put_Visible(int Visible);
        HRESULT get_Visible(out int pVisible);
        HRESULT put_Left(int Left);
        HRESULT get_Left(out int pLeft);
        HRESULT put_Width(int Width);
        HRESULT get_Width(out int pWidth);
        HRESULT put_Top(int Top);
        HRESULT get_Top(out int pTop);
        HRESULT put_Height(int Height);
        HRESULT get_Height(out int pHeight);
        [PreserveSig]
        HRESULT put_Owner(IntPtr Owner);
        HRESULT get_Owner(out IntPtr Owner);
        HRESULT put_MessageDrain(IntPtr Drain);
        HRESULT get_MessageDrain(out IntPtr Drain);
        HRESULT get_BorderColor(out int Color);
        HRESULT put_BorderColor(int Color);
        HRESULT get_FullScreenMode(out int FullScreenMode);
        HRESULT put_FullScreenMode(int FullScreenMode);
        HRESULT SetWindowForeground(int Focus);
        HRESULT NotifyOwnerMessage(IntPtr hwnd, int uMsg, int wParam, IntPtr lParam);
        [PreserveSig]
        HRESULT SetWindowPosition(int Left, int Top, int Width, int Height);
        HRESULT GetWindowPosition(out int pLeft, out int pTop, out int pWidth, out int pHeight);
        HRESULT GetMinIdealImageSize(out int pWidth, out int pHeight);
        HRESULT GetMaxIdealImageSize(out int pWidth, out int pHeight);
        HRESULT GetRestorePosition(out int pLeft, out int pTop, out int pWidth, out int pHeight);
        HRESULT HideCursor(int HideCursor);
        HRESULT IsCursorHidden(out int CursorHidden);
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("56a868c0-0ad4-11ce-b03a-0020af0ba770")]
    public interface IMediaEventEx : IMediaEvent
    {
        #region IMediaEvent
        new HRESULT GetEventHandle(out IntPtr hEvent);
        [PreserveSig]
        new HRESULT GetEvent(out int lEventCode, out IntPtr lParam1, out IntPtr lParam2, int msTimeout);
        new HRESULT WaitForCompletion(int msTimeout, out int pEvCode);
        new HRESULT CancelDefaultHandling(long lEvCode);
        new HRESULT RestoreDefaultHandling(int lEvCode);
        new HRESULT FreeEventParams(int lEvCode, IntPtr lParam1, IntPtr lParam2);
        #endregion

        HRESULT SetNotifyWindow(IntPtr hwnd, int lMsg, IntPtr lInstanceData);
        HRESULT SetNotifyFlags(int lNoNotifyFlags);
        HRESULT GetNotifyFlags(out int lplNoNotifyFlags);
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("56a868b6-0ad4-11ce-b03a-0020af0ba770")]
    public interface IMediaEvent
    {
        HRESULT GetEventHandle(out IntPtr hEvent);
        [PreserveSig]
        HRESULT GetEvent(out int lEventCode, out IntPtr lParam1, out IntPtr lParam2, int msTimeout);
        HRESULT WaitForCompletion(int msTimeout, out int pEvCode);
        HRESULT CancelDefaultHandling(long lEvCode);
        HRESULT RestoreDefaultHandling(int lEvCode);
        HRESULT FreeEventParams(int lEvCode, IntPtr lParam1, IntPtr lParam2);
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("93E5A4E0-2D50-11d2-ABFA-00A0C9C6E38D")]
    public interface ICaptureGraphBuilder2
    {
        HRESULT SetFiltergraph(IGraphBuilder pfg);
        HRESULT GetFiltergraph(out IGraphBuilder ppfg);
        HRESULT SetOutputFileName(ref Guid pType, string lpstrFile, out IBaseFilter ppf, out IFileSinkFilter ppSink);
        HRESULT FindInterface(ref Guid pCategory, ref Guid pType, IBaseFilter pf, ref Guid riid, out IntPtr ppint);
        //HRESULT RenderStream(ref Guid pCategory, ref Guid pType, IUnknown pSource,IBaseFilter pfCompressor,IBaseFilter pfRenderer);
        [PreserveSig]
        HRESULT RenderStream(ref Guid pCategory, ref Guid pType, IntPtr pSource, IBaseFilter pfCompressor, IBaseFilter pfRenderer);
        HRESULT ControlStream(ref Guid pCategory, ref Guid pType, IBaseFilter pFilter, Int64 pstart, Int64 pstop, ushort wStartCookie, ushort wStopCookie);
        HRESULT AllocCapFile(string lpstr, UInt64 dwlSize);
        HRESULT CopyCaptureFile(string lpwstrOld, string lpwstrNew, int fAllowEscAbort, IAMCopyCaptureFileProgress pCallback);
        //HRESULT FindPin(IUnknown pSource, PIN_DIRECTION pindir, ref Guid pCategory, ref Guid pType, bool fUnconnected, int num, out IPin ppPin);
        HRESULT FindPin(IntPtr pSource, PIN_DIRECTION pindir, ref Guid pCategory, ref Guid pType, bool fUnconnected, int num, out IPin ppPin);
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("a2104830-7c70-11cf-8bce-00aa00a3f1a6")]
    public interface IFileSinkFilter
    {
        HRESULT SetFileName(string pszFileName, AM_MEDIA_TYPE pmt);
        HRESULT GetCurFile(out string ppszFileName, out AM_MEDIA_TYPE pmt);
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("670d1d20-a068-11d0-b3f0-00aa003761c5")]
    public interface IAMCopyCaptureFileProgress
    {
        HRESULT Progress(int iProgress);
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("29840822-5B84-11D0-BD3B-00A0C911CE86")]
    public interface ICreateDevEnum
    {
        HRESULT CreateClassEnumerator(ref Guid clsidDeviceClass, out IEnumMoniker ppEnumMoniker, int dwFlags);
    }

    [ComImport]
    [Guid("00000102-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumMoniker
    {
        HRESULT Next(uint celt, out IMoniker rgelt, out uint pceltFetched);
        HRESULT Skip(uint celt);
        HRESULT Reset();
        HRESULT Clone(out IEnumMoniker ppenum);
    }

    [ComImport]
    [Guid("0000000f-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMoniker : IPersistStream
    {
        #region IPersist
        new HRESULT GetClassID([Out] out Guid pClassID);
        #endregion

        #region IPersistStream
        new HRESULT IsDirty();
        new HRESULT Load(System.Runtime.InteropServices.ComTypes.IStream pStm);
        new HRESULT Save(System.Runtime.InteropServices.ComTypes.IStream pStm, bool fClearDirty);
        new HRESULT GetSizeMax(out ULARGE_INTEGER pcbSize);
        #endregion

        //HRESULT BindToObject(System.Runtime.InteropServices.ComTypes.IBindCtx pbc, IMoniker pmkToLeft, ref Guid riidResult, ref IntPtr ppvResult);
        HRESULT BindToObject(IntPtr pbc, IMoniker pmkToLeft, ref Guid riidResult, ref IntPtr ppvResult);

        //HRESULT BindToStorage(IBindCtx pbc, IMoniker pmkToLeft, ref Guid riid, out IntPtr ppvObj);
        HRESULT BindToStorage(IntPtr pbc, IMoniker pmkToLeft, ref Guid riid, out IntPtr ppvObj);

        //HRESULT Reduce(IBindCtx pbc, int dwReduceHowFar, ref IMoniker ppmkToLeft, out IMoniker ppmkReduced);
        HRESULT Reduce(IntPtr pbc, int dwReduceHowFar, ref IMoniker ppmkToLeft, out IMoniker ppmkReduced);

        HRESULT ComposeWith(IMoniker pmkRight, bool fOnlyIfNotGeneric, out IMoniker ppmkComposite);

        HRESULT Enum(bool fForward, out IEnumMoniker ppenumMoniker);

        HRESULT IsEqual(IMoniker pmkOtherMoniker);

        HRESULT Hash(out int pdwHash);

        //HRESULT IsRunning(IBindCtx pbc, IMoniker pmkToLeft, IMoniker pmkNewlyRunning);
        HRESULT IsRunning(IntPtr pbc, IMoniker pmkToLeft, IMoniker pmkNewlyRunning);

        //HRESULT GetTimeOfLastChange(IBindCtx pbc, IMoniker pmkToLeft, out FILETIME pFileTime);
        HRESULT GetTimeOfLastChange(IntPtr pbc, IMoniker pmkToLeft, out System.Runtime.InteropServices.ComTypes.FILETIME pFileTime);

        HRESULT Inverse(out IMoniker ppmk);

        HRESULT CommonPrefixWith(IMoniker pmkOther, out IMoniker ppmkPrefix);

        HRESULT RelativePathTo(IMoniker pmkOther, out IMoniker ppmkRelPath);

        //HRESULT GetDisplayName(IBindCtx pbc, IMoniker pmkToLeft, out LPOLESTR ppszDisplayName);
        HRESULT GetDisplayName(IntPtr pbc, IMoniker pmkToLeft, out string ppszDisplayName);

        //HRESULT ParseDisplayName(IBindCtx pbc, IMoniker pmkToLeft, LPOLESTR pszDisplayName, out uint pchEaten, out IMoniker ppmkOut);
        HRESULT ParseDisplayName(IntPtr pbc, IMoniker pmkToLeft, string pszDisplayName, out uint pchEaten, out IMoniker ppmkOut);

        HRESULT IsSystemMoniker(out int pdwMksys);
    }

    [ComImport]
    [Guid("00000109-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersistStream : IPersist
    {
        #region IPersist
        new HRESULT GetClassID([Out] out Guid pClassID);
        #endregion
        HRESULT IsDirty();
        HRESULT Load(System.Runtime.InteropServices.ComTypes.IStream pStm);
        HRESULT Save(System.Runtime.InteropServices.ComTypes.IStream pStm, bool fClearDirty);
        HRESULT GetSizeMax(out ULARGE_INTEGER pcbSize);
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ULARGE_INTEGER
    {
        [FieldOffset(0)]
        public int LowPart;
        [FieldOffset(4)]
        public int HighPart;
        [FieldOffset(0)]
        public long QuadPart;
    }

    [ComImport]
    [Guid("55272A00-42CB-11CE-8135-00AA004BB851")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyBag
    {
        //HRESULT Read(string pszPropName, ref VARIANT* pVar, IErrorLog pErrorLog);
        //HRESULT Read(string pszPropName, ref IntPtr pVar, IErrorLog pErrorLog);
        HRESULT Read(string pszPropName, out PROPVARIANT pVar, IErrorLog pErrorLog);

        //HRESULT Write(string pszPropName, VARIANT* pVar);
        HRESULT Write(string pszPropName, IntPtr pVar);
    }

    [ComImport]
    [Guid("3127CA40-446E-11CE-8135-00AA004BB851")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IErrorLog
    {
        //HRESULT AddError(LPCOLESTR pszPropName, System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo);
        HRESULT AddError([In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName, System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PROPARRAY
    {
        public UInt32 cElems;
        public IntPtr pElems;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct PROPVARIANT
    {
        [FieldOffset(0)]
        public ushort varType;
        [FieldOffset(2)]
        public ushort wReserved1;
        [FieldOffset(4)]
        public ushort wReserved2;
        [FieldOffset(6)]
        public ushort wReserved3;

        [FieldOffset(8)]
        public byte bVal;
        [FieldOffset(8)]
        public sbyte cVal;
        [FieldOffset(8)]
        public ushort uiVal;
        [FieldOffset(8)]
        public short iVal;
        [FieldOffset(8)]
        public UInt32 uintVal;
        [FieldOffset(8)]
        public Int32 intVal;
        [FieldOffset(8)]
        public UInt64 ulVal;
        [FieldOffset(8)]
        public Int64 lVal;
        [FieldOffset(8)]
        public float fltVal;
        [FieldOffset(8)]
        public double dblVal;
        [FieldOffset(8)]
        public short boolVal;
        [FieldOffset(8)]
        public IntPtr pclsidVal; // GUID ID pointer
        [FieldOffset(8)]
        public IntPtr pszVal; // Ansi string pointer
        [FieldOffset(8)]
        public IntPtr pwszVal; // Unicode string pointer
        [FieldOffset(8)]
        public IntPtr punkVal; // punkVal (interface pointer)
        [FieldOffset(8)]
        public PROPARRAY ca;
        [FieldOffset(8)]
        public System.Runtime.InteropServices.ComTypes.FILETIME filetime;
    }

    public enum VARENUM
    {
        VT_EMPTY = 0,
        VT_NULL = 1,
        VT_I2 = 2,
        VT_I4 = 3,
        VT_R4 = 4,
        VT_R8 = 5,
        VT_CY = 6,
        VT_DATE = 7,
        VT_BSTR = 8,
        VT_DISPATCH = 9,
        VT_ERROR = 10,
        VT_BOOL = 11,
        VT_VARIANT = 12,
        VT_UNKNOWN = 13,
        VT_DECIMAL = 14,
        VT_I1 = 16,
        VT_UI1 = 17,
        VT_UI2 = 18,
        VT_UI4 = 19,
        VT_I8 = 20,
        VT_UI8 = 21,
        VT_INT = 22,
        VT_UINT = 23,
        VT_VOID = 24,
        VT_HRESULT = 25,
        VT_PTR = 26,
        VT_SAFEARRAY = 27,
        VT_CARRAY = 28,
        VT_USERDEFINED = 29,
        VT_LPSTR = 30,
        VT_LPWSTR = 31,
        VT_RECORD = 36,
        VT_INT_PTR = 37,
        VT_UINT_PTR = 38,
        VT_FILETIME = 64,
        VT_BLOB = 65,
        VT_STREAM = 66,
        VT_STORAGE = 67,
        VT_STREAMED_OBJECT = 68,
        VT_STORED_OBJECT = 69,
        VT_BLOB_OBJECT = 70,
        VT_CF = 71,
        VT_CLSID = 72,
        VT_VERSIONED_STREAM = 73,
        VT_BSTR_BLOB = 0xfff,
        VT_VECTOR = 0x1000,
        VT_ARRAY = 0x2000,
        VT_BYREF = 0x4000,
        VT_RESERVED = 0x8000,
        VT_ILLEGAL = 0xffff,
        VT_ILLEGALMASKED = 0xfff,
        VT_TYPEMASK = 0xfff
    };


    [ComImport]
    [Guid("5a804648-4f66-4867-9c43-4f5c822cf1b8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRFilterConfig9
    {
        HRESULT SetImageCompositor(IVMRImageCompositor9 lpVMRImgCompositor);
        HRESULT SetNumberOfStreams(uint dwMaxStreams);
        HRESULT GetNumberOfStreams(out uint pdwMaxStreams);
        HRESULT SetRenderingPrefs(uint dwRenderFlags);
        HRESULT GetRenderingPrefs(out uint pdwRenderFlags);
        HRESULT SetRenderingMode(uint Mode);
        HRESULT GetRenderingMode(out uint pMode);
    }

    [ComImport]
    [Guid("4a5c89eb-df51-4654-ac2a-e48e02bbabf6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRImageCompositor9
    {
        HRESULT InitCompositionDevice(IntPtr pD3DDevice);
        HRESULT TermCompositionDevice(IntPtr pD3DDevice);
        HRESULT SetStreamMediaType(uint dwStrmID, AM_MEDIA_TYPE pmt, bool fTexture);
        //HRESULT CompositeImage(IntPtr pD3DDevice, IDirect3DSurface9* pddsRenderTarget, AM_MEDIA_TYPE pmtRenderTarget, long rtStart,
        //     long rtEnd, uint dwClrBkGnd, VMR9VideoStreamInfo pVideoStreamInfo, uint cStreams);
        HRESULT CompositeImage(IntPtr pD3DDevice, IntPtr pddsRenderTarget, AM_MEDIA_TYPE pmtRenderTarget, long rtStart,
            long rtEnd, uint dwClrBkGnd, ref VMR9VideoStreamInfo pVideoStreamInfo, uint cStreams);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VMR9VideoStreamInfo
    {
        //public IDirect3DSurface9* pddsVideoSurface;
        public IntPtr pddsVideoSurface;
        public uint dwWidth;
        public uint dwHeight;
        public uint dwStrmID;
        public float fAlpha;
        VMR9NormalizedRect rNormal;
        long rtStart;
        long rtEnd;
        VMR9_SampleFormat SampleFormat;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VMR9NormalizedRect
    {
        public float left;
        public float top;
        public float right;
        public float bottom;
    }

    public enum VMR9_SampleFormat
    {
        VMR9_SampleReserved = 1,
        VMR9_SampleProgressiveFrame = 2,
        VMR9_SampleFieldInterleavedEvenFirst = 3,
        VMR9_SampleFieldInterleavedOddFirst = 4,
        VMR9_SampleFieldSingleEven = 5,
        VMR9_SampleFieldSingleOdd = 6
    }

    public enum VMR9Mode
    {
        VMR9Mode_Windowed = 0x1,
        VMR9Mode_Windowless = 0x2,
        VMR9Mode_Renderless = 0x4,
        VMR9Mode_Mask = 0x7
    }

    [ComImport]
    [Guid("8f537d09-f85e-4414-b23b-502e54c79927")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRWindowlessControl9
    {
        HRESULT GetNativeVideoSize(out int lpWidth, out int lpHeight, out int lpARWidth, out int lpARHeight);
        HRESULT GetMinIdealVideoSize(out int lpWidth, out int lpHeight);
        HRESULT GetMaxIdealVideoSize(out int lpWidth, out int lpHeight);
        //HRESULT SetVideoPosition(ref RECT lpSRCRect, ref RECT lpDSTRect);
        HRESULT SetVideoPosition(IntPtr lpSRCRect, ref RECT lpDSTRect);
        //HRESULT SetVideoPosition(IntPtr lpSRCRect, IntPtr lpDSTRect);
        HRESULT GetVideoPosition(out RECT lpSRCRect, out RECT lpDSTRect);
        HRESULT GetAspectRatioMode(out uint lpAspectRatioMode);
        HRESULT SetAspectRatioMode(uint AspectRatioMode);
        HRESULT SetVideoClippingWindow(IntPtr hwnd);
        HRESULT RepaintVideo(IntPtr hwnd, IntPtr hdc);
        HRESULT DisplayModeChanged();
        //HRESULT GetCurrentImage(out BYTE** lpDib);
        HRESULT GetCurrentImage(out IntPtr lpDib);
        HRESULT SetBorderColor(uint Clr);
        HRESULT GetBorderColor(out uint lpClr);
    }

    //[ComImport]
    //[Guid("ced175e5-1935-4820-81bd-ff6ad00c9108")]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //public interface IVMRMixerBitmap9
    //{
    //    HRESULT SetAlphaBitmap(ref VMR9AlphaBitmap pBmpParms);
    //    HRESULT UpdateAlphaBitmapParameters(ref VMR9AlphaBitmap pBmpParms);
    //    HRESULT GetAlphaBitmapParameters(out VMR9AlphaBitmap pBmpParms);           
    //}

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
        Guid("ced175e5-1935-4820-81bd-ff6ad00c9108"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRMixerBitmap9
    {
        [PreserveSig]
        HRESULT SetAlphaBitmap([In] ref VMR9AlphaBitmap pBmpParms);

        [PreserveSig]
        HRESULT UpdateAlphaBitmapParameters([In] ref VMR9AlphaBitmap pBmpParms);

        [PreserveSig]
        HRESULT GetAlphaBitmapParameters([Out] out VMR9AlphaBitmap pBmpParms);
    }

    /// <summary> 
    /// From VMR9AlphaBitmap 
    /// </summary> 
    [StructLayout(LayoutKind.Sequential)]
    public struct VMR9AlphaBitmap
    {
        public uint dwFlags;
        public IntPtr hdc; // HDC 
        public IntPtr pDDS; // IDirect3DSurface9 
        public RECT rSrc;
        public VMR9NormalizedRect rDest;
        public float fAlpha;
        public int clrSrcKey;
        public uint dwFilterMode;
    }

    public enum VMRBITMAP
    {
        VMRBITMAP_DISABLE = 0x00000001,
        VMRBITMAP_HDC = 0x00000002,
        VMRBITMAP_ENTIREDDS = 0x00000004,
        VMRBITMAP_SRCCOLORKEY = 0x00000008,
        VMRBITMAP_SRCRECT = 0x00000010
    }
}
