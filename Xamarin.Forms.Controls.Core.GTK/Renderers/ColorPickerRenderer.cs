using GtkToolkit.Controls;
using GtkToolkit.GTK.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(ColorPicker), typeof(ColorPickerRenderer))]
namespace GtkToolkit.GTK.Renderers
{
    public class ColorPickerRenderer : ViewRenderer<ColorPicker, Gtk.ColorSelection>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ColorPicker> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var colorPicker = new Gtk.ColorSelection();
                    SetNativeControl(colorPicker);
                }
            }

            base.OnElementChanged(e);
        }
    }
}