using System;
using Xamarin.Forms;
using Xamarin.Forms.Control.Core.Controls;
using Xamarin.Forms.Controls.Core.GTK.Renderers;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(Separator), typeof(SeparatorRenderer))]
namespace Xamarin.Forms.Controls.Core.GTK.Renderers
{
    public class SeparatorRenderer : ViewRenderer<Separator, Gtk.Separator>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Separator> e)
        {
            if (Control == null)
            {
                SetNativeControl(new Gtk.Separator(IntPtr.Zero));
            }

            base.OnElementChanged(e);
        }
    }
}