using Gtk;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Xamarin.Forms.Platform.GTK.Helpers;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class WebView : EventBox
    {
        private VBox _vbox = null;
        private WebKit.WebView _webview = null;
        private WebBrowser _browser = null;
        private GTKPlatform _platform;

        /// <summary>
        /// Imported unmanaged function for setting the parent of a window.
        /// it's used for setting the parent of a WebBrowser.
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "SetParent")]
        private static extern IntPtr SetParent([In] IntPtr hWndChild, [In] IntPtr hWndNewParent);

        public WebView()
        {
            _platform = PlatformHelper.GetGTKPlatform();
            BuildWebView();
        }

        public WebKit.WebView Browser
        {
            get { return _webview; }
        }

        public WebBrowser WindowsBrowser
        {
            get { return _browser; }
        }

        public void Open(string uri)
        {
            if (_platform == GTKPlatform.Linux || _platform == GTKPlatform.MacOS)
            {
                _webview.Open(uri);
            }
            else
            {
                _browser.Navigate(uri);
            }
        }

        public void LoadString(string html, string baseUrl)
        {
            if (_platform == GTKPlatform.Linux || _platform == GTKPlatform.MacOS)
            {
                _webview.LoadHtmlString(html, baseUrl);
            }
            else
            {
                _browser.DocumentText = html;
            }
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);

            if (IsRealized)
            {
                System.Drawing.Size size = new System.Drawing.Size(allocation.Width, allocation.Height);
                _browser.Size = size;
            }
        }

        private void BuildWebView()
        {
            CreateWebView();

            if (_platform == GTKPlatform.Linux || _platform == GTKPlatform.MacOS)
            {
                ScrolledWindow scroll = new ScrolledWindow();
                scroll.AddWithViewport(_webview);

                _vbox = new VBox(false, 1);
                _vbox.PackStart(scroll, true, true, 0);

                Add(_vbox);
                ShowAll();
            }
            else
            {
                ScrolledWindow scroll = new ScrolledWindow
                {
                    CanFocus = true,
                    ShadowType = ShadowType.None
                };

                _vbox = new VBox(false, 1);
                _vbox.PackStart(scroll, true, true, 0);

                var socket = new Socket();
                scroll.Add(socket);
                socket.Realize();

                IntPtr browserHandle = _browser.Handle;
                IntPtr socketHandle = (IntPtr)socket.Id;
                SetParent(browserHandle, socketHandle);

                Add(_vbox);
                ShowAll();
            }
        }

        private void CreateWebView()
        {
            if (_platform == GTKPlatform.Linux || _platform == GTKPlatform.MacOS)
            {
                _webview = new WebKit.WebView();
                _webview.Editable = false;
            }
            else
            {
                _browser = new WebBrowser();
                _browser.ScriptErrorsSuppressed = true;
                _browser.AllowWebBrowserDrop = false;
                _browser.DocumentText = string.Empty; // Force Document initialization
            }
        }
    }
}