using Gtk;
using System;

namespace Xamarin.Forms.Platform.GTK.Linux
{
    public class WebViewLinux : EventBox
    {
        private VBox _vbox = null;
        private WebKit.WebView _webview = null;

        public event EventHandler LoadStarted;
        public event EventHandler LoadFinished;

        public WebViewLinux()
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
                return _webview.Uri;
            }
            set
            {
                _webview.LoadUri(value);
            }
        }

        public void Navigate(string uri)
        {
            _webview.Open(uri);
        }

        public void LoadHTML(string html, string baseUrl)
        {
            _webview.LoadHtmlString(html, baseUrl);
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
            _webview.ExecuteScript(script);
        }

        private void BuildWebView()
        {
            CreateWebView();

            ScrolledWindow scroll = new ScrolledWindow();
            scroll.AddWithViewport(_webview);

            _vbox = new VBox(false, 1);
            _vbox.PackStart(scroll, true, true, 0);

            Add(_vbox);
            ShowAll();
        }

        private void CreateWebView()
        {
            _webview = new WebKit.WebView();
            _webview.Editable = false;

            _webview.LoadStarted += (sender, args) =>
            {
                LoadStarted?.Invoke(this, args);
            };

            _webview.LoadFinished += (sender, args) =>
            {
                LoadFinished?.Invoke(this, args);
            };
        }
    }
}