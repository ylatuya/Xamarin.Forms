using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class WebViewRenderer : ViewRenderer<WebView, Controls.WebView>, IWebViewDelegate, IEffectControlProvider
    {
        private bool _disposed;
        private bool _ignoreSourceChanges;
        private WebNavigationEvent _lastBackForwardEvent;
        private WebNavigationEvent _lastEvent;

        IWebViewController WebViewController => Element;

        void IEffectControlProvider.RegisterEffect(Effect effect)
        {
            var platformEffect = effect as PlatformEffect;
            if (platformEffect != null)
                platformEffect.SetContainer(Container);
        }

        void IWebViewDelegate.LoadHtml(string html, string baseUrl)
        {
            if (string.IsNullOrEmpty(html))
            {
                var urlWebViewSource = Element.Source as HtmlWebViewSource;

                if (urlWebViewSource != null)
                {
                    html = urlWebViewSource.Html;
                }
            }

            if (Control != null)
            {
                Control.LoadString(html, baseUrl);
            }
        }

        void IWebViewDelegate.LoadUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                var urlWebViewSource = Element.Source as UrlWebViewSource;

                if (urlWebViewSource != null)
                {
                    url = urlWebViewSource.Url;
                }
            }

            if (Control != null)
            {
                Control.Open(url);
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    try
                    {
                        Control = new Controls.WebView();
                    }
                    catch (Exception ex)
                    {
                        Log.Warning("WebView loading", $"WebView load failed: {ex}");
                    }

                    SetNativeControl(Control);

                    if (Control != null)
                    {
                        Control.Browser.LoadFinished += OnLoadFinished;
                    }

                    WebViewController.GoBackRequested += OnGoBackRequested;
                    WebViewController.GoForwardRequested += OnGoForwardRequested;
                }
            }

            Load();

            EffectUtilities.RegisterEffectControlProvider(this, e.OldElement, e.NewElement);
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

                Control.Browser.LoadFinished -= OnLoadFinished;
                WebViewController.GoBackRequested -= OnGoBackRequested;
                WebViewController.GoForwardRequested -= OnGoForwardRequested;
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

        private void UpdateCanGoBackForward()
        {
            if (Element == null)
                return;

            if (Control != null)
            {
                WebViewController.CanGoBack = Control.Browser.CanGoBack();
                WebViewController.CanGoForward = Control.Browser.CanGoForward();
            }
        }

        private void OnLoadFinished(object o, WebKit.LoadFinishedArgs args)
        {
            if (Control == null)
            {
                return;
            }

            _ignoreSourceChanges = true;
            ElementController?.SetValueFromRenderer(WebView.SourceProperty,
                new UrlWebViewSource { Url = Control.Browser.Uri });
            _ignoreSourceChanges = false;

            _lastEvent = _lastBackForwardEvent;
            WebViewController?.SendNavigated(new WebNavigatedEventArgs(
                _lastEvent,
                Element?.Source,
                Control.Browser.Uri,
                WebNavigationResult.Success));

            UpdateCanGoBackForward();
        }

        private void OnGoBackRequested(object sender, EventArgs eventArgs)
        {
            if (Control == null)
            {
                return;
            }

            if (Control.Browser.CanGoBack())
            {
                _lastBackForwardEvent = WebNavigationEvent.Back;
                Control.Browser.GoBack();
            }

            UpdateCanGoBackForward();
        }

        private void OnGoForwardRequested(object sender, EventArgs eventArgs)
        {
            if (Control == null)
            {
                return;
            }

            if (Control.Browser.CanGoForward())
            {
                _lastBackForwardEvent = WebNavigationEvent.Forward;
                Control.Browser.GoForward();
            }

            UpdateCanGoBackForward();
        }
    }
}