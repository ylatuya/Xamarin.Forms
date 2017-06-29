using System.ComponentModel;
using GtkToolkit.Controls;
using GtkToolkit.GTK.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(DataGrid), typeof(DataGridRenderer))]
namespace GtkToolkit.GTK.Renderers
{
    public class DataGridRenderer : ViewRenderer<DataGrid, Gtk.TreeView>
    {
        private bool _disposed;
        private Gtk.TreeView _treeView;

        protected override void OnElementChanged(ElementChangedEventArgs<DataGrid> e)
        {
            if (Control == null)
            {
                _treeView = new Gtk.TreeView();

                SetNativeControl(_treeView);
            }

            if (e.NewElement != null)
            {
                UpdateColumns();
                UpdateItems();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == DataGrid.ColumnsProperty.PropertyName)
                UpdateColumns();
            if (e.PropertyName == DataGrid.ItemsSourceProperty.PropertyName)
                UpdateItems();

            base.OnElementPropertyChanged(sender, e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }

            base.Dispose(disposing);
        }

        private void UpdateColumns()
        {
            if (_treeView != null)
            {
                var columns = Element.Columns;

                foreach (var col in columns)
                {
                    Gtk.TreeViewColumn column = new Gtk.TreeViewColumn();
                    column.Title = col.Title;

                    // Add the columns to the TreeView
                    _treeView.AppendColumn(column);
                }
            }
        }

        private void UpdateItems()
        {
            if (_treeView != null)
            {
                var items = Element.ItemsSource;
            }
        }
    }
}