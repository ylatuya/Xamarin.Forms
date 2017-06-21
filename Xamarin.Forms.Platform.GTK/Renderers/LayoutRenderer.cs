using Gtk;
using Xamarin.Forms.Platform.GTK.Packagers;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class LayoutRenderer : ViewRenderer<Layout, Fixed>
    {
        private Fixed _fixed;
        private LayoutElementPackager _packager;

        protected override void OnElementChanged(ElementChangedEventArgs<Layout> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    if(_fixed == null)
                    {
                        _fixed = new Fixed();
                    }

                    SetNativeControl(_fixed);
                }

                _packager = new LayoutElementPackager(this);
                _packager.Load();
            }

            base.OnElementChanged(e);
        }
    }
}