using Gtk;

namespace Xamarin.Forms.Platform.GTK
{
    internal class PlatformRenderer : EventBox
    {
        private bool _disposed;

        public PlatformRenderer(Platform platform)
        {
            Platform = platform;
        }

        public Platform Platform { get; set; }

        public override void Dispose()
        {
            if (!_disposed)
            {

            }

            _disposed = true;

            base.Dispose();
        }
    }
}