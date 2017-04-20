using Gtk;

namespace Xamarin.Forms.Platform.GTK
{
    internal class PlatformRenderer : EventBox
    {
        public PlatformRenderer(Platform platform)
        {
            Platform = platform;
        }

        public Platform Platform { get; set; }

        protected override void OnRealized()
        {
            base.OnRealized();

            var viewRenderer = Platform.GetRenderer(Application.Current.MainPage);
            viewRenderer?.UpdateLayout();
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);

            var viewRenderer = Platform.GetRenderer(Application.Current.MainPage);
            viewRenderer?.SetElementSize(new Size(
                allocation.Width,
                allocation.Height));
        }
    }
}
