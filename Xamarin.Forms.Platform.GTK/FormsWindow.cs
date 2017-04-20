using System;
using System.ComponentModel;
using Gtk;
using GRectangle = Gdk.Rectangle;

namespace Xamarin.Forms.Platform.GTK
{
    public class FormsWindow : Window
    {
        private Application _application;
        private GRectangle _lastAllocation;

        public FormsWindow() 
            : base(WindowType.Toplevel)
        {
            SetSizeRequest(800, 600);
        }

        public void LoadApplication(Application application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            Application.SetCurrentApplication(application);
            _application = application;

            application.PropertyChanged += ApplicationOnPropertyChanged;
            UpdateMainPage();
        }

        protected override void OnSizeAllocated(GRectangle allocation)
        {
            base.OnSizeAllocated(allocation);

            if (!_lastAllocation.Equals(allocation))
            {
                _lastAllocation = allocation;

                var viewRenderer = Platform.GetRenderer(Application.Current.MainPage);
                viewRenderer?.UpdateLayout();
            }
        }

        public sealed override void Dispose()
        {
            base.Dispose();

            Dispose(true);
        }

        private void ApplicationOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(Application.MainPage))
            {
                UpdateMainPage();
            }
        }

        private void UpdateMainPage()
        {
            if (_application.MainPage == null)
                return;

            var platformRenderer = Child as PlatformRenderer;

            if (platformRenderer != null)
            {
                ((IDisposable)platformRenderer.Platform).Dispose();
            }

            var platform = new Platform();
            platform.Renderer.SetSizeRequest(WidthRequest, HeightRequest);

            Add(platform.Renderer);
            platform.SetPage(_application.MainPage);

            Child.ShowAll();
        }

        private void Dispose(bool disposing)
        {
            if (disposing && _application != null)
            {
                _application.PropertyChanged -= ApplicationOnPropertyChanged;
            }
        }
    }
}