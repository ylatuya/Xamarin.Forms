using GtkToolkit.Controls;
using GtkToolkit.GTK.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(DataGrid), typeof(DataGridRenderer))]
namespace GtkToolkit.GTK.Renderers
{
    public class DataGridRenderer : ViewRenderer<DataGrid, Gtk.TreeView>
    {

    }
}