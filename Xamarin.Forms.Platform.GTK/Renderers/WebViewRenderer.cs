using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class WebViewRenderer : ViewRenderer<WebView, WebKit.WebView>, IWebViewDelegate
    {
        private bool _disposed;
        private bool _ignoreSourceChanges;
        private WebNavigationEvent _lastBackForwardEvent;
        private WebNavigationEvent _lastEvent;

        IWebViewController ElementController => Element;

        public void LoadHtml(string html, string baseUrl)
        {
            if (html != null)
                Control.MainFrame.LoadString(html, string.Empty, string.Empty, baseUrl);
        }

        public void LoadUrl(string url)
        {
            Control.Open(url);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var webView = new WebKit.WebView();
                    SetNativeControl(webView);

                    Control.LoadFinished += OnLoadFinished;
                    ElementController.EvalRequested += OnEvalRequested;
                    ElementController.GoBackRequested += OnGoBackRequested;
                    ElementController.GoForwardRequested += OnGoForwardRequested;
                }
            }

            Load();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == WebView.SourceProperty.PropertyName)
                Load();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                Control.LoadFinished -= OnLoadFinished;
                ElementController.EvalRequested -= OnEvalRequested;
                ElementController.GoBackRequested -= OnGoBackRequested;
                ElementController.GoForwardRequested -= OnGoForwardRequested;
            }

            base.Dispose(disposing);
        }

        private void Load()
        {
            if (_ignoreSourceChanges)
                return;

            Element?.Source?.Load(this);

            UpdateCanGoBackForward();
        }

        private void OnEvalRequested(object sender, EvalRequested eventArg)
        {
            // TODO:
        }

        private void UpdateCanGoBackForward()
        {
            if (Element == null)
                return;

            ElementController.CanGoBack = Control.CanGoBack();
            ElementController.CanGoForward = Control.CanGoForward();
        }

        private void OnLoadFinished(object o, WebKit.LoadFinishedArgs args)
        {
            _ignoreSourceChanges = true;
            ElementController?.SetValueFromRenderer(WebView.SourceProperty,
                new UrlWebViewSource { Url = Control.Uri });
            _ignoreSourceChanges = false;

            _lastEvent = _lastBackForwardEvent;
            ElementController?.SendNavigated(new WebNavigatedEventArgs(_lastEvent, Element?.Source, Control.Uri,
                WebNavigationResult.Success));

            UpdateCanGoBackForward();
        }

        private void OnGoBackRequested(object sender, EventArgs eventArgs)
        {
            if (Control.CanGoBack())
            {
                _lastBackForwardEvent = WebNavigationEvent.Back;
                Control.GoBack();
            }

            UpdateCanGoBackForward();
        }

        private void OnGoForwardRequested(object sender, EventArgs eventArgs)
        {
            if (Control.CanGoForward())
            {
                _lastBackForwardEvent = WebNavigationEvent.Forward;
                Control.GoForward();
            }

            UpdateCanGoBackForward();
        }
    }
}