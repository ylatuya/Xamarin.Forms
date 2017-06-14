using Gdk;
using Gtk;
using System.Diagnostics;
using Xamarin.Forms.Platform.GTK.Packagers;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class LayoutRenderer : ViewRenderer<Layout, Fixed>
    {
        private LayoutElementPackager _packager;

        protected override void OnElementChanged(ElementChangedEventArgs<Layout> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Fixed());
                }

                _packager = new LayoutElementPackager(this);
                _packager.Load();
            }

            base.OnElementChanged(e);
        }
    }
}