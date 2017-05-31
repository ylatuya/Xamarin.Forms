using GtkToolkit.Controls;
using GtkToolkit.GTK.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(HyperLink), typeof(HyperLinkRenderer))]
namespace GtkToolkit.GTK.Renderers
{
    public class HyperLinkRenderer : ViewRenderer<HyperLink, Gtk.LinkButton>
    {

    }
}
