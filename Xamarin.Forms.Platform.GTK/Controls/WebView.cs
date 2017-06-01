using Gtk;
using WebKit;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class WebView : EventBox
    {
        private VBox _vbox = null;
        private WebKit.WebView _webview = null;

        public WebView()
        {
            BuildWebView();
        }

        public WebKit.WebView Browser
        {
            get { return _webview; }
        }

        public void Open(string uri)
        {
            _webview.Open(uri);
        }

        public void LoadString(string html, string baseUrl)
        {
            _webview.LoadString(html, string.Empty, string.Empty, baseUrl);
        }

        private void BuildWebView()
        {
            CreateWebView();

            ScrolledWindow scroll = new ScrolledWindow();
            scroll.Add(_webview);
            _vbox = new VBox(false, 1);
            _vbox.PackStart(scroll);

            Add(_vbox);
            ShowAll();
        }

        private void CreateWebView()
        {
            _webview = new WebKit.WebView();
            _webview.Editable = false;
        }
    }
}
