using Gtk;
using System;
using Xamarin.Forms.Platform.GTK.Helpers;
using Xamarin.Forms.Platform.GTK.Linux;
using Xamarin.Forms.Platform.GTK.Mac;
using Xamarin.Forms.Platform.GTK.Windows;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class WebView : EventBox
    {
        private GTKPlatform _platform;
        private WebViewWindows _webViewWindows; // System.Windows.Forms.WebBrowser
        private WebViewLinux _webViewLinux;     // webkit-sharp
        private WebViewMac _webViewMac;         // Xamarin.Mac WebKit
        public event EventHandler LoadStarted;
        public event EventHandler LoadFinished;

        public string Uri
        {
            get
            {
                switch (_platform)
                {
                    case GTKPlatform.Linux:
                        return _webViewLinux.Uri;
                    case GTKPlatform.MacOS:
                        return _webViewMac.Uri;
                    case GTKPlatform.Windows:
                        return _webViewWindows.Uri;
                    default:
                        return string.Empty;
                }
            }
            set
            {
                switch (_platform)
                {
                    case GTKPlatform.Linux:
                        _webViewLinux.Uri = value;
                        break;
                    case GTKPlatform.MacOS:
                        _webViewMac.Uri = value;
                        break;
                    case GTKPlatform.Windows:
                        _webViewWindows.Uri = value;
                        break;
                }
            }
        }

        public WebView()
        {
            BuildWebView();
        }

        private void BuildWebView()
        {
            _platform = PlatformHelper.GetGTKPlatform();

            switch (_platform)
            {
                case GTKPlatform.Linux:
                    _webViewLinux = new WebViewLinux();

                    _webViewLinux.LoadStarted += (sender, args) => { LoadStarted?.Invoke(this, args); };
                    _webViewLinux.LoadFinished += (sender, args) => { LoadFinished?.Invoke(this, args); };

                    Add(_webViewLinux);
                    break;
                case GTKPlatform.MacOS:
                    _webViewMac = new WebViewMac();

                    _webViewMac.LoadStarted += (sender, args) => { LoadStarted?.Invoke(this, args); };
                    _webViewMac.LoadFinished += (sender, args) => { LoadFinished?.Invoke(this, args); };

                    Add(_webViewMac);
                    break;
                case GTKPlatform.Windows:
                    _webViewWindows = new WebViewWindows();

                    _webViewWindows.LoadStarted += (sender, args) => { LoadStarted?.Invoke(this, args); };
                    _webViewWindows.LoadFinished += (sender, args) => { LoadFinished?.Invoke(this, args); };

                    Add(_webViewWindows);
                    break;
            }
        }

        public void Navigate(string uri)
        {
            switch (_platform)
            {
                case GTKPlatform.Linux:
                    _webViewLinux.Navigate(uri);
                    break;
                case GTKPlatform.MacOS:
                    _webViewMac.Navigate(uri);
                    break;
                case GTKPlatform.Windows:
                    _webViewWindows.Navigate(uri);
                    break;
            }
        }

        public void LoadHTML(string html, string baseUrl)
        {
            switch (_platform)
            {
                case GTKPlatform.Linux:
                    _webViewLinux.LoadHTML(html, baseUrl);
                    break;
                case GTKPlatform.MacOS:
                    _webViewMac.LoadHTML(html, baseUrl);
                    break;
                case GTKPlatform.Windows:
                    _webViewWindows.LoadHTML(html, baseUrl);
                    break;
            }
        }

        public bool CanGoBack()
        {
            switch (_platform)
            {
                case GTKPlatform.Linux:
                    return _webViewLinux.CanGoBack();
                case GTKPlatform.MacOS:
                    return _webViewMac.CanGoBack();
                case GTKPlatform.Windows:
                    return _webViewWindows.CanGoBack();
                default:
                    return false;
            }
        }

        public void GoBack()
        {
            switch (_platform)
            {
                case GTKPlatform.Linux:
                    _webViewLinux.GoBack();
                    break;
                case GTKPlatform.MacOS:
                    _webViewMac.GoBack();
                    break;
                case GTKPlatform.Windows:
                    _webViewWindows.GoBack();
                    break;
            }
        }

        public bool CanGoForward()
        {
            switch (_platform)
            {
                case GTKPlatform.Linux:
                    return _webViewLinux.CanGoForward();
                case GTKPlatform.MacOS:
                    return _webViewMac.CanGoForward();
                case GTKPlatform.Windows:
                    return _webViewWindows.CanGoForward();
                default:
                    return false;
            }
        }

        public void GoForward()
        {
            switch (_platform)
            {
                case GTKPlatform.Linux:
                    _webViewLinux.GoForward();
                    break;
                case GTKPlatform.MacOS:
                    _webViewMac.GoForward();
                    break;
                case GTKPlatform.Windows:
                    _webViewWindows.GoForward();
                    break;
            }
        }

        public void ExecuteScript(string script)
        {
            switch (_platform)
            {
                case GTKPlatform.Linux:
                    _webViewLinux.ExecuteScript(script);
                    break;
                case GTKPlatform.MacOS:
                    _webViewMac.ExecuteScript(script);
                    break;
                case GTKPlatform.Windows:
                    _webViewWindows.ExecuteScript(script);
                    break;
            }
        }
    }
}