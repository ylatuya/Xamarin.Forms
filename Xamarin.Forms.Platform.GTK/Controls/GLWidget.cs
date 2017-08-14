using Gtk;
using OpenTK.Graphics;
using OpenTK.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using Xamarin.Forms.Platform.GTK.Helpers;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class GLWidget : DrawingArea, IDisposable
    {
        private IWindowInfo _windowInfo;
        private IGraphicsContext _graphicsContext;
        private static int _graphicsContextCount;
        private static bool _sharedContextInitialized = false;
        private bool _initialized = false;

        public GLWidget() : this(GraphicsMode.Default)
        {
        }

        public GLWidget(GraphicsMode graphicsMode) : this(graphicsMode, 1, 0, GraphicsContextFlags.Default)
        {
        }

        public GLWidget(GraphicsMode graphicsMode, int glVersionMajor, int glVersionMinor, GraphicsContextFlags graphicsContextFlags)
        {
            DoubleBuffered = false;

            SingleBuffer = graphicsMode.Buffers == 1;
            ColorBPP = graphicsMode.ColorFormat.BitsPerPixel;
            AccumulatorBPP = graphicsMode.AccumulatorFormat.BitsPerPixel;
            DepthBPP = graphicsMode.Depth;
            StencilBPP = graphicsMode.Stencil;
            Samples = graphicsMode.Samples;
            Stereo = graphicsMode.Stereo;

            GlVersionMajor = glVersionMajor;
            GlVersionMinor = glVersionMinor;
            GraphicsContextFlags = graphicsContextFlags;
        }

        ~GLWidget()
        {
            Dispose(false);
        }

        [Browsable(true)]
        public bool SingleBuffer { get; set; }

        public int ColorBPP { get; set; }

        public int AccumulatorBPP { get; set; }

        public int DepthBPP { get; set; }

        public int StencilBPP { get; set; }

        public int Samples { get; set; }

        public bool Stereo { get; set; }

        public int GlVersionMajor { get; set; }

        public int GlVersionMinor { get; set; }

        public GraphicsContextFlags GraphicsContextFlags
        {
            get;
            set;
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
            base.Dispose();
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _graphicsContext.MakeCurrent(_windowInfo);
                OnShuttingDown();

                if (GraphicsContext.ShareContexts && (Interlocked.Decrement(ref _graphicsContextCount) == 0))
                {
                    OnGraphicsContextShuttingDown();
                    _sharedContextInitialized = false;
                }

                _graphicsContext.Dispose();
            }
        }

        public static event EventHandler GraphicsContextInitialized;

        static void OnGraphicsContextInitialized()
        {
            GraphicsContextInitialized?.Invoke(null, EventArgs.Empty);
        }

        public static event EventHandler GraphicsContextShuttingDown;

        static void OnGraphicsContextShuttingDown()
        {
            GraphicsContextShuttingDown?.Invoke(null, EventArgs.Empty);
        }

        public event EventHandler Initialized;

        protected virtual void OnInitialized()
        {
            Initialized?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler RenderFrame;

        protected virtual void OnRenderFrame()
        {
            RenderFrame?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ShuttingDown;

        protected virtual void OnShuttingDown()
        {
            ShuttingDown?.Invoke(this, EventArgs.Empty);
        }

        protected override bool OnExposeEvent(Gdk.EventExpose eventExpose)
        {
            if (!_initialized)
            {
                _initialized = true;

                if (ColorBPP == 0)
                {
                    ColorBPP = 32;

                    if (DepthBPP == 0)
                        DepthBPP = 16;
                }

                ColorFormat colorBufferColorFormat = new ColorFormat(ColorBPP);

                ColorFormat accumulationColorFormat = new ColorFormat(AccumulatorBPP);

                int buffers = 2;
                if (SingleBuffer)
                    buffers--;

                GraphicsMode graphicsMode = 
                    new GraphicsMode(colorBufferColorFormat, DepthBPP, StencilBPP, Samples, accumulationColorFormat, buffers, Stereo);

                if (PlatformHelper.GetGTKPlatform() == GTKPlatform.Windows)
                {
                    IntPtr windowHandle = gdk_win32_drawable_get_handle(GdkWindow.Handle);
                    _windowInfo = Utilities.CreateWindowsWindowInfo(windowHandle);
                }
                else if (PlatformHelper.GetGTKPlatform() == GTKPlatform.MacOS)
                {
                    IntPtr windowHandle = gdk_x11_drawable_get_xid(GdkWindow.Handle);
                    const bool ownHandle = true;
                    const bool isControl = true;
                    _windowInfo = Utilities.CreateMacOSCarbonWindowInfo(windowHandle, ownHandle, isControl);
                }
                else if (PlatformHelper.GetGTKPlatform() == GTKPlatform.Linux)
                {
                    IntPtr display = gdk_x11_display_get_xdisplay(Display.Handle);
                    int screen = Screen.Number;
                    IntPtr windowHandle = gdk_x11_drawable_get_xid(GdkWindow.Handle);
                    IntPtr rootWindow = gdk_x11_drawable_get_xid(RootWindow.Handle);

                    IntPtr visualInfo;
                    if (graphicsMode.Index.HasValue)
                    {
                        XVisualInfo info = new XVisualInfo();
                        info.VisualID = graphicsMode.Index.Value;
                        int dummy;
                        visualInfo = XGetVisualInfo(display, XVisualInfoMask.ID, ref info, out dummy);
                    }
                    else
                    {
                        visualInfo = GetVisualInfo(display);
                    }

                    _windowInfo = Utilities.CreateX11WindowInfo(display, screen, windowHandle, rootWindow, visualInfo);
                    XFree(visualInfo);
                }
                else
                    throw new PlatformNotSupportedException();

                _graphicsContext = new GraphicsContext(graphicsMode, _windowInfo, GlVersionMajor, GlVersionMinor, GraphicsContextFlags);
                _graphicsContext.MakeCurrent(_windowInfo);

                if (GraphicsContext.ShareContexts)
                {
                    Interlocked.Increment(ref _graphicsContextCount);

                    if (!_sharedContextInitialized)
                    {
                        _sharedContextInitialized = true;
                        ((IGraphicsContextInternal)_graphicsContext).LoadAll();
                        OnGraphicsContextInitialized();
                    }
                }
                else
                {
                    ((IGraphicsContextInternal)_graphicsContext).LoadAll();
                    OnGraphicsContextInitialized();
                }

                OnInitialized();
            }
            else
            {
                _graphicsContext.MakeCurrent(_windowInfo);
            }

            bool result = base.OnExposeEvent(eventExpose);
            OnRenderFrame();
            eventExpose.Window.Display.Sync(); 
            _graphicsContext.SwapBuffers();
            return result;
        }

        protected void SwapBuffers()
        {
            if (_graphicsContext != null)
            {
                _graphicsContext.SwapBuffers();
            }
        }

        protected override bool OnConfigureEvent(Gdk.EventConfigure evnt)
        {
            bool result = base.OnConfigureEvent(evnt);

            if (_graphicsContext != null)
                _graphicsContext.Update(_windowInfo);

            return result;
        }

        [DllImport("libgdk-win32-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gdk_win32_drawable_get_handle(IntPtr d);

        public enum XVisualClass : int
        {
            StaticGray = 0,
            GrayScale = 1,
            StaticColor = 2,
            PseudoColor = 3,
            TrueColor = 4,
            DirectColor = 5,
        }

        [StructLayout(LayoutKind.Sequential)]
        struct XVisualInfo
        {
            public IntPtr Visual;
            public IntPtr VisualID;
            public int Screen;
            public int Depth;
            public XVisualClass Class;
            public long RedMask;
            public long GreenMask;
            public long BlueMask;
            public int ColormapSize;
            public int BitsPerRgb;

            public override string ToString()
            {
                return string.Format("id ({0}), screen ({1}), depth ({2}), class ({3})",
                    VisualID, Screen, Depth, Class);
            }
        }

        [Flags]
        internal enum XVisualInfoMask
        {
            No = 0x0,
            ID = 0x1,
            Screen = 0x2,
            Depth = 0x4,
            Class = 0x8,
            Red = 0x10,
            Green = 0x20,
            Blue = 0x40,
            ColormapSize = 0x80,
            BitsPerRGB = 0x100,
            All = 0x1FF,
        }

        [DllImport("libX11", EntryPoint = "XGetVisualInfo")]
        static extern IntPtr XGetVisualInfoInternal(IntPtr display, IntPtr vinfo_mask, ref XVisualInfo template, out int nitems);

        static IntPtr XGetVisualInfo(IntPtr display, XVisualInfoMask vinfo_mask, ref XVisualInfo template, out int nitems)
        {
            return XGetVisualInfoInternal(display, (IntPtr)(int)vinfo_mask, ref template, out nitems);
        }

        const string linux_libx11_name = "libX11.so.6";

        [SuppressUnmanagedCodeSecurity, DllImport(linux_libx11_name)]
        static extern void XFree(IntPtr handle);

        const string linux_libgdk_x11_name = "libgdk-x11-2.0.so.0";

        [SuppressUnmanagedCodeSecurity, DllImport(linux_libgdk_x11_name)]
        static extern IntPtr gdk_x11_drawable_get_xid(IntPtr gdkDisplay);

        [SuppressUnmanagedCodeSecurity, DllImport(linux_libgdk_x11_name)]
        static extern IntPtr gdk_x11_display_get_xdisplay(IntPtr gdkDisplay);

        IntPtr GetVisualInfo(IntPtr display)
        {
            try
            {
                int[] attributes = AttributeList.ToArray();
                return glXChooseVisual(display, Screen.Number, attributes);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("OpenGL dll not found!", e);
            }
            catch (EntryPointNotFoundException enf)
            {
                throw new EntryPointNotFoundException("Glx entry point not found!", enf);
            }
        }

        const int GLX_NONE = 0;
        const int GLX_USE_GL = 1;
        const int GLX_BUFFER_SIZE = 2;
        const int GLX_LEVEL = 3;
        const int GLX_RGBA = 4;
        const int GLX_DOUBLEBUFFER = 5;
        const int GLX_STEREO = 6;
        const int GLX_AUX_BUFFERS = 7;
        const int GLX_RED_SIZE = 8;
        const int GLX_GREEN_SIZE = 9;
        const int GLX_BLUE_SIZE = 10;
        const int GLX_ALPHA_SIZE = 11;
        const int GLX_DEPTH_SIZE = 12;
        const int GLX_STENCIL_SIZE = 13;
        const int GLX_ACCUM_RED_SIZE = 14;
        const int GLX_ACCUM_GREEN_SIZE = 15;
        const int GLX_ACCUM_BLUE_SIZE = 16;
        const int GLX_ACCUM_ALPHA_SIZE = 17;

        List<int> AttributeList
        {
            get
            {
                List<int> attributeList = new List<int>(24);

                attributeList.Add(GLX_RGBA);

                if (!SingleBuffer)
                    attributeList.Add(GLX_DOUBLEBUFFER);

                if (Stereo)
                    attributeList.Add(GLX_STEREO);

                attributeList.Add(GLX_RED_SIZE);
                attributeList.Add(ColorBPP / 4);

                attributeList.Add(GLX_GREEN_SIZE);
                attributeList.Add(ColorBPP / 4); 

                attributeList.Add(GLX_BLUE_SIZE);
                attributeList.Add(ColorBPP / 4); 

                attributeList.Add(GLX_ALPHA_SIZE);
                attributeList.Add(ColorBPP / 4); 

                attributeList.Add(GLX_DEPTH_SIZE);
                attributeList.Add(DepthBPP);

                attributeList.Add(GLX_STENCIL_SIZE);
                attributeList.Add(StencilBPP);

                attributeList.Add(GLX_ACCUM_RED_SIZE);
                attributeList.Add(AccumulatorBPP / 4);

                attributeList.Add(GLX_ACCUM_GREEN_SIZE);
                attributeList.Add(AccumulatorBPP / 4);

                attributeList.Add(GLX_ACCUM_BLUE_SIZE);
                attributeList.Add(AccumulatorBPP / 4);

                attributeList.Add(GLX_ACCUM_ALPHA_SIZE);
                attributeList.Add(AccumulatorBPP / 4);

                attributeList.Add(GLX_NONE);

                return attributeList;
            }
        }

        const string linux_libgl_name = "libGL.so.1";

        [SuppressUnmanagedCodeSecurity, DllImport(linux_libgl_name)]
        static extern IntPtr glXChooseVisual(IntPtr display, int screen, int[] attr);
    }
}