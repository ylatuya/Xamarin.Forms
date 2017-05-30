using Xamarin.Forms;
using Xamarin.Forms.Control.Core.Controls;
using Xamarin.Forms.Controls.Core.GTK.Renderers;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(DataGrid), typeof(DataGridRenderer))]
namespace Xamarin.Forms.Controls.Core.GTK.Renderers
{
    public class DataGridRenderer : ViewRenderer<DataGrid, Gtk.TreeView>
    {

    }
}