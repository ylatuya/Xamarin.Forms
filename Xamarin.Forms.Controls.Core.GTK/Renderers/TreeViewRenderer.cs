using System.ComponentModel;
using GtkToolkit.Controls;
using GtkToolkit.GTK.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;
using System.Linq;

[assembly: ExportRenderer(typeof(TreeView), typeof(TreeViewRenderer))]
namespace GtkToolkit.GTK.Renderers
{
    public class TreeViewRenderer : ViewRenderer<TreeView, Gtk.TreeView>
    {
        private const int DefaultRowHeight = 24;

        private bool _disposed;
        private Gtk.TreeView _treeView;

        protected override void OnElementChanged(ElementChangedEventArgs<TreeView> e)
        {
            if (Control == null)
            {
                _treeView = new Gtk.TreeView();
                _treeView.HeadersVisible = false;

                Gtk.TreeViewColumn column = new Gtk.TreeViewColumn();
                Gtk.CellRendererText cell = new Gtk.CellRendererText();
                column.PackStart(cell, true);
                column.AddAttribute(cell, "text", 0);
                _treeView.AppendColumn(column);

                Add(_treeView);
                _treeView.ShowAll();

                SetNativeControl(_treeView);
            }

            if (e.NewElement != null)
            {
                UpdateItems();
                UpdateRowHeight();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TreeView.ItemsSourceProperty.PropertyName)
                UpdateItems();
            else if (e.PropertyName == TreeView.RowHeightProperty.PropertyName)
                UpdateRowHeight();

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

        private void UpdateItems()
        {
            if (_treeView != null)
            {
                var items = Element.ItemsSource;

                if(items == null)
                {
                    return;
                }

                Gtk.TreeStore treeStore = new Gtk.TreeStore(typeof(string));

                foreach (var item in items)
                {
                    AddNode(item, treeStore);
                }

                _treeView.Model = treeStore;
                _treeView.ShowAll();
            }
        }

        private void AddNode(Node node, Gtk.TreeStore treeStore)
        {
            Gtk.TreeIter iter = treeStore.AppendValues(node.Name);

            if (node.Children.Any())
            {
                foreach (var child in node.Children)
                {
                    treeStore.AppendValues(iter, child.Name);
                }
            }
        }

        private void UpdateRowHeight()
        {
            var rowHeight = Element.RowHeight;

            var column = _treeView.Columns.FirstOrDefault();

            if (column != null)
            {
                var cell = column.Cells.FirstOrDefault();

                if (cell != null)
                {
                    cell.Height = rowHeight > 0 ? rowHeight : DefaultRowHeight;
                }
            }
        }
    }
}