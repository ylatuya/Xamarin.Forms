using Gtk;
using OpenTK.Graphics;
using OpenTK.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class GLWidgetLinux : DrawingArea, IDisposable
    {
        private bool isInitialized;
        protected uint timerId;
        private IGraphicsContext graphicsContext;
        private static int graphicsContextCount;
        private IWindowInfo windowInfo;
        protected float scaleFactor;

        public IntPtr WindowsHandler { get; private set; }

        public bool IsMinimized { get; set; }

        public bool IsDisposed { get; private set; }

        [Browsable(true)]
        public bool SingleBuffer { get; set; }

		public int ColorBPP { get; set; }

        public int AccumulatorBPP { get; set; }

        public int DepthBPP { get; set; }

        public int StencilBPP { get; set; }

        public int Samples { get; set; }

        public bool Stereo { get; set; }

        public GLWidgetLinux(GraphicsMode graphicsMode)
        {
            SingleBuffer = graphicsMode.Buffers == 1;
            ColorBPP = graphicsMode.ColorFormat.BitsPerPixel;
            AccumulatorBPP = graphicsMode.AccumulatorFormat.BitsPerPixel;
            DepthBPP = graphicsMode.Depth;
            StencilBPP = graphicsMode.Stencil;
            Samples = graphicsMode.Samples;
            Stereo = graphicsMode.Stereo;

            isInitialized = false;
            DoubleBuffered = false;

            scaleFactor = 1.0f;
        }

        ~GLWidgetLinux()
        {
            Dispose(false);
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
                graphicsContext.MakeCurrent(windowInfo);
                OnShuttingDown();
                if (GraphicsContext.ShareContexts && (Interlocked.Decrement(ref graphicsContextCount) == 0))
                {
                    OnGraphicsContextShuttingDown();
                    sharedContextInitialized = false;
                }
                graphicsContext.Dispose();
            }
        }

        public static event EventHandler GraphicsContextInitialized;
        static void OnGraphicsContextInitialized() { GraphicsContextInitialized?.Invoke(null, EventArgs.Empty); }

        public static event EventHandler GraphicsContextShuttingDown;
        static void OnGraphicsContextShuttingDown() { GraphicsContextShuttingDown?.Invoke(null, EventArgs.Empty); }

        public event EventHandler Initialized;
        protected virtual void OnInitialized() { Initialized?.Invoke(this, EventArgs.Empty); }

        public event EventHandler RenderFrame;
        protected virtual void OnRenderFrame() { RenderFrame?.Invoke(this, EventArgs.Empty); }

        public event EventHandler ShuttingDown;
        protected virtual void OnShuttingDown() { ShuttingDown?.Invoke(this, EventArgs.Empty); }

        static bool sharedContextInitialized = false;

        protected override bool OnExposeEvent(Gdk.EventExpose eventExpose)
        {
            if (!isInitialized)
            {
                GLib.Timeout.Add(10, new GLib.TimeoutHandler(CreateGraphicsContext));
                isInitialized = true;
            }
            else
            {
                if (graphicsContext != null)
                {
                    graphicsContext.MakeCurrent(windowInfo);
                }
            }

            bool result = base.OnExposeEvent(eventExpose);

            eventExpose.Window.Display.Sync(); // Add Sync call to fix resize rendering problem (Jay L. T. Cornwall) - How does this affect VSync?
                                               //graphicsContext.SwapBuffers();
            return result;
        }

        public bool CreateGraphicsContext()
        {
            GraphicsMode graphicsMode = GraphicsMode.Default;
            SingleBuffer = graphicsMode.Buffers == 1;
            ColorBPP = graphicsMode.ColorFormat.BitsPerPixel;
            AccumulatorBPP = graphicsMode.AccumulatorFormat.BitsPerPixel;
            DepthBPP = graphicsMode.Depth;
            StencilBPP = graphicsMode.Stencil;
            Samples = graphicsMode.Samples;
            Stereo = graphicsMode.Stereo;

            if (ColorBPP == 0)
            {
                ColorBPP = 32;

                if (DepthBPP == 0)
                    DepthBPP = 16;
            }

            ColorFormat colorBufferColorFormat = new ColorFormat(ColorBPP);

            ColorFormat accumulationColorFormat = new ColorFormat(AccumulatorBPP);

            int buffers = 2;
            if (SingleBuffer) buffers--;

            graphicsMode = new GraphicsMode(colorBufferColorFormat, DepthBPP, StencilBPP, Samples, accumulationColorFormat, buffers, Stereo);

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

                windowInfo = Utilities.CreateX11WindowInfo(display, screen, windowHandle, rootWindow, visualInfo);
                XFree(visualInfo);        

            graphicsContext = new GraphicsContext(graphicsMode, windowInfo);
            graphicsContext.MakeCurrent(windowInfo);

            if (GraphicsContext.ShareContexts)
            {
                Interlocked.Increment(ref graphicsContextCount);

                if (!sharedContextInitialized)
                {
                    sharedContextInitialized = true;
                    ((IGraphicsContextInternal)graphicsContext).LoadAll();
                    OnGraphicsContextInitialized();
                }
            }
            else
            {
                ((IGraphicsContextInternal)graphicsContext).LoadAll();
                OnGraphicsContextInitialized();
            }

            timerId = GLib.Timeout.Add(16, new GLib.TimeoutHandler(Render));

            return false;
        }

		public virtual bool Render()
        {
            OnRenderFrame();
            graphicsContext.SwapBuffers();

            return true;
        }

        protected override bool OnConfigureEvent(Gdk.EventConfigure evnt)
        {
            bool result = base.OnConfigureEvent(evnt);

            if (graphicsContext != null) graphicsContext.Update(windowInfo);

            return result;
        }

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
            public long blueMask;
            public int ColormapSize;
            public int BitsPerRgb;

            public override string ToString()
            {
                return String.Format("id ({0}), screen ({1}), depth ({2}), class ({3})",
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

                if (!SingleBuffer) attributeList.Add(GLX_DOUBLEBUFFER);

                if (Stereo) attributeList.Add(GLX_STEREO);

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