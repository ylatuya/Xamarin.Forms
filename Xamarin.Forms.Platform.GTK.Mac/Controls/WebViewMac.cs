using Gtk;
using System;
using System.Runtime.InteropServices;

namespace Xamarin.Forms.Platform.GTK.Mac
{
    public class WebViewMac : EventBox
    {
        const string LibGdk = "libgdk-quartz-2.0.dylib";
        const string LibGtk = "libgtk-quartz-2.0";

        private VBox _vbox = null;
        private WebKit.WebView _webview = null;
        private Widget _webViewGtkWidget = null;

        public event EventHandler LoadStarted;
        public event EventHandler LoadFinished;

        [DllImport(LibGtk)]
        extern static IntPtr gtk_ns_view_new(IntPtr nsview);

        public WebViewMac()
        {
            BuildWebView();
        }

        public WebKit.WebView WebView
        {
            get { return _webview; }
        }

        public string Uri
        {
            get
            {
                return _webview.MainFrameUrl;
            }
            set
            {
                _webview.MainFrameUrl = value;
            }
        }

        public void Navigate(string uri)
        {
            _webview.MainFrameUrl = uri;
        }

        public void LoadHTML(string html, string baseUrl)
        {
            _webview.MainFrame.LoadHtmlString(new Foundation.NSString(html), new Foundation.NSUrl(baseUrl));
        }

        public bool CanGoBack()
        {
            return _webview.CanGoBack();
        }

        public void GoBack()
        {
            _webview.GoBack();
        }

        public bool CanGoForward()
        {
            return _webview.CanGoForward();
        }

        public void GoForward()
        {
            _webview.GoForward();
        }

        public void ExecuteScript(string script)
        {
            _webview.StringByEvaluatingJavaScriptFromString(script);
        }

        internal Widget NSViewToGtkWidget(object view)
        {
            var prop = view.GetType().GetProperty("Handle");
            var handle = prop.GetValue(view, null);
            return new Widget(gtk_ns_view_new((IntPtr)handle));
        }

        private void BuildWebView()
        {
            CreateWebView();

            ScrolledWindow scroll = new ScrolledWindow();
            _webViewGtkWidget = NSViewToGtkWidget(_webview);
            scroll.AddWithViewport(_webViewGtkWidget);

            _vbox = new VBox(false, 1);
            _vbox.PackStart(scroll, true, true, 0);

            Add(_vbox);
            ShowAll();
        }

        private void CreateWebView()
        {
            _webview = new WebKit.WebView();
            _webview.Editable = false;

            _webview.CommitedLoad += (sender, args) =>
            {
                LoadStarted?.Invoke(this, args);
            };

            _webview.FinishedLoad += (sender, args) =>
            {
                LoadFinished?.Invoke(this, args);
            };
        }
    }
}