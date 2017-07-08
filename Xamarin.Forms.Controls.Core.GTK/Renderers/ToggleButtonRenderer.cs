using GtkToolkit.Controls;
using GtkToolkit.GTK.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(ToggleButton), typeof(ToggleButtonRenderer))]
namespace GtkToolkit.GTK.Renderers
{
    public class ToggleButtonRenderer : ViewRenderer<ToggleButton, Gtk.ToggleButton>
    {
        private Gtk.ToggleButton _toggleButton;

        protected override void OnElementChanged(ElementChangedEventArgs<ToggleButton> e)
        {
            if (Control == null)
            {
                _toggleButton = new Gtk.ToggleButton();

                Add(_toggleButton);
                _toggleButton.ShowAll();

                SetNativeControl(_toggleButton);
            }

            base.OnElementChanged(e);
        }
    }
}