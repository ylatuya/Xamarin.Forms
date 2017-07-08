using GtkToolkit.Controls;
using GtkToolkit.GTK.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(FontButton), typeof(FontButtonRenderer))]
namespace GtkToolkit.GTK.Renderers
{
    public class FontButtonRenderer : ViewRenderer<FontButton, Gtk.FontButton>
    {
        private Gtk.FontButton _fontButton;

        protected override void OnElementChanged(ElementChangedEventArgs<FontButton> e)
        {
            if (Control == null)
            {
                _fontButton = new Gtk.FontButton();

                Add(_fontButton);
                _fontButton.ShowAll();

                SetNativeControl(_fontButton);
            }

            base.OnElementChanged(e);
        }
    }
}