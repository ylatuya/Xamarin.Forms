using Gtk;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class LayoutRenderer : ViewRenderer<Layout, Fixed>
    {
        private VisualElementPackager _packager;

        protected override void OnElementChanged(ElementChangedEventArgs<Layout> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Fixed());
                }

                _packager = new VisualElementPackager(this);
                _packager.Load();
            }

            base.OnElementChanged(e);
        }
    }
}