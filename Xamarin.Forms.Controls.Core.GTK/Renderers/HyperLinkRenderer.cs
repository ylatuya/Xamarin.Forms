using Xamarin.Forms;
using Xamarin.Forms.Control.Core.Controls;
using Xamarin.Forms.Controls.Core.GTK.Renderers;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(HyperLink), typeof(HyperLinkRenderer))]
namespace Xamarin.Forms.Controls.Core.GTK.Renderers
{
    public class HyperLinkRenderer : ViewRenderer<HyperLink, Gtk.LinkButton>
    {

    }
}
