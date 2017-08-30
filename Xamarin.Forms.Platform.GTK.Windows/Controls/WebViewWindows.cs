using Gtk;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Xamarin.Forms.Platform.GTK.Windows
{
    public class WebViewWindows : EventBox
    {
        [DllImport("libgdk-win32-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr gdk_win32_drawable_get_handle(IntPtr d);

        private WebBrowser _browser = null;

        public event EventHandler LoadStarted;
        public event EventHandler LoadFinished;

        /// <summary>
        /// Imported unmanaged function for setting the parent of a window.
        /// it's used for setting the parent of a WebBrowser.
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "SetParent")]
        private static extern IntPtr SetParent([In] IntPtr hWndChild, [In] IntPtr hWndNewParent);

        public WebViewWindows()
        {
            BuildWebView();
        }

        public WebBrowser WebBrowser
        {
            get { return _browser; }
        }

        public string Uri
        {
            get
            {
                return _browser.Url != null ? _browser.Url.ToString() : string.Empty;
            }
            set
            {
                _browser.Url = new Uri(value);
            }
        }

        public void Navigate(string uri)
        {
            Uri uriResult;
            bool result = System.Uri.TryCreate(uri, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == System.Uri.UriSchemeHttp || uriResult.Scheme == System.Uri.UriSchemeHttps);

            if (result)
            {
                _browser.Navigate(new Uri(uri));
            }
            else
            {
                string appPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                string filePath = System.IO.Path.Combine(appPath, uri);
                _browser.Url = new Uri(filePath);
            }
        }

        public void LoadHTML(string html, string baseUrl)
        {
            _browser.DocumentText = html;
            _browser.Update();
        }

        public bool CanGoBack()
        {
            return _browser.CanGoBack;
        }

        public void GoBack()
        {
            _browser.GoBack();
        }

        public bool CanGoForward()
        {
            return _browser.CanGoForward;
        }

        public void GoForward()
        {
            _browser.GoForward();
        }

        public void ExecuteScript(string script)
        {
            _browser.DocumentText = script;
            _browser.Document.InvokeScript(script);
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);

            if (IsRealized)
            {
                _browser.Bounds =
                    new System.Drawing.Rectangle(allocation.X, allocation.Y, allocation.Width, allocation.Height);
            }
        }

        private void BuildWebView()
        {
            CreateWebView();

            var browserHandle = _browser.Handle;

            ScrolledWindow scroll = new ScrolledWindow
            {
                CanFocus = true,
                ShadowType = ShadowType.None
            };

            var drawingArea = new DrawingArea();

            IntPtr windowHandle;

            drawingArea.ExposeEvent += (s, a) =>
            {
                IntPtr test = drawingArea.GdkWindow.Handle;
                windowHandle = gdk_win32_drawable_get_handle(test);

                // Embedding Windows Browser control into a gtk widget.
                SetParent(browserHandle, windowHandle);
            };

            scroll.Add(drawingArea);

            Add(scroll);
            ShowAll();
        }

        private void CreateWebView()
        {
            _browser = new WebBrowser();
            _browser.ScriptErrorsSuppressed = true;
            _browser.AllowWebBrowserDrop = false;

            _browser.Navigating += (sender, args) =>
            {
                LoadStarted?.Invoke(this, args);
            };

            _browser.Navigated += (sender, args) =>
            {
                LoadFinished?.Invoke(this, args);
            };
        }
    }
}
