using System;
using System.ComponentModel;
using Gtk;

namespace Xamarin.Forms.Platform.GTK
{
    public class FormsWindow : Window
    {
        private Application _application;

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

        public sealed override void Dispose()
        {
            base.Dispose();

            Dispose(true);
        }

        protected override bool OnConfigureEvent(Gdk.EventConfigure evnt)
        {
            Child?.QueueDraw();

            return base.OnConfigureEvent(evnt);
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
            platform.PlatformRenderer.SetSizeRequest(WidthRequest, HeightRequest);

            Add(platform.PlatformRenderer);
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