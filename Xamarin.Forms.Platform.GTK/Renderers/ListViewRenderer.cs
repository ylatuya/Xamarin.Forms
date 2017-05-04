using Gtk;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class ListViewRenderer : ViewRenderer<ListView, VBox>
    {
        public const int DefaultRowHeight = 44;

        private bool _disposed;
        private TreeView _treeView;
        private EventBox _header;
        private EventBox _footer;
        private IVisualElementRenderer _headerRenderer;
        private IVisualElementRenderer _footerRenderer;

        IListViewController Controller => Element;

        ITemplatedItemsView<Cell> TemplatedItemsView => Element;

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var scroller = new VBox();

                    _header = new EventBox();
                    scroller.Add(_header);

                    _treeView = new TreeView();
                    _treeView.RulesHint = true;
                    _treeView.HeadersVisible = false;
                    scroller.Add(_treeView);

                    _footer = new EventBox();
                    scroller.Add(_footer);

                    SetNativeControl(scroller);
                }

                UpdateItems();
                UpdateHeader();
                UpdateFooter();
                UpdateRowHeight();
                UpdateSeparatorColor();
                UpdateSeparatorVisibility();
                UpdateIsRefreshing();
                UpdatePullToRefreshEnabled();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ListView.ItemsSourceProperty.PropertyName)
                UpdateItems();
            else if (e.PropertyName.Equals("HeaderElement", System.StringComparison.InvariantCultureIgnoreCase))
                UpdateHeader();
            else if (e.PropertyName.Equals("FooterElement", System.StringComparison.InvariantCultureIgnoreCase))
                UpdateFooter();
            else if (e.PropertyName == ListView.RowHeightProperty.PropertyName)
                UpdateRowHeight();
            else if (e.PropertyName == ListView.SeparatorColorProperty.PropertyName)
                UpdateSeparatorColor();
            else if (e.PropertyName == ListView.SeparatorVisibilityProperty.PropertyName)
                UpdateSeparatorVisibility();
            else if (e.PropertyName == ListView.IsRefreshingProperty.PropertyName)
                UpdateIsRefreshing();
            else if (e.PropertyName == ListView.IsPullToRefreshEnabledProperty.PropertyName)
                UpdatePullToRefreshEnabled();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && !_disposed)
            {
                _disposed = true;
            }
        }

        protected override void UpdateBackgroundColor()
        {
            base.UpdateBackgroundColor();

            if (_treeView == null)
            {
                return;
            }

            if (Element.BackgroundColor.IsDefault)
            {
                return;
            }

            var backgroundColor = Element.BackgroundColor.ToGtkColor();

            foreach (var column in _treeView.Columns)
            {
                foreach (var cell in column.Cells)
                {
                    cell.CellBackgroundGdk = backgroundColor;
                }
            }
        }

        private void UpdateItems()
        {
            var items = TemplatedItemsView.TemplatedItems;

            if (!items.Any())
            {
                return;
            }

            ListStore listStore = new ListStore(typeof(object));

            foreach (var cell in items)
            {
                var renderer =
                    (Cells.CellRenderer)Internals.Registrar.Registered.GetHandler<IRegisterable>(cell.GetType());
                var nativeCell = renderer.GetCell(cell, null, _treeView);

                if (!_treeView.Columns.Any())
                {
                    TreeViewColumn column = new TreeViewColumn();
                    column.FixedWidth = System.Convert.ToInt32(Element.WidthRequest);
                    column.PackStart(nativeCell, true);
                    _treeView.AppendColumn(column);
                    column.AddAttribute(nativeCell, "attribute", 0);
                }

                listStore.AppendValues(nativeCell);
            }

            _treeView.Model = listStore;
        }

        private void UpdateHeader()
        {
            var header = Controller.HeaderElement;
            var headerView = (View)header;

            if (headerView != null)
            {
                _headerRenderer = Platform.CreateRenderer(headerView);
                Platform.SetRenderer(headerView, _headerRenderer);

                _header.Add(_headerRenderer.Container);
            }
        }

        private void UpdateFooter()
        {
            var footer = Controller.FooterElement;
            var footerView = (View)footer;

            if (footerView != null)
            {
                _footerRenderer = Platform.CreateRenderer(footerView);
                Platform.SetRenderer(footerView, _footerRenderer);

                _footer.Add(_footerRenderer.Container);
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

        //TODO: Implement SeparatorColor
        private void UpdateSeparatorColor()
        {

        }

        private void UpdateSeparatorVisibility()
        {
            if (Element.SeparatorVisibility == SeparatorVisibility.Default)
                _treeView.EnableGridLines = TreeViewGridLines.Horizontal;
            else
                _treeView.EnableGridLines = TreeViewGridLines.None;
        }

        //TODO: Implement UpdateIsRefreshing
        private void UpdateIsRefreshing()
        {

        }

        //TODO: Implement PullToRefresh
        private void UpdatePullToRefreshEnabled()
        {

        }
    }
}