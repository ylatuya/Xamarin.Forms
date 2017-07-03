using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.GTK.Cells;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class ListViewRenderer : ViewRenderer<ListView, Controls.ListView>
    {
        public const int DefaultRowHeight = 44;

        private bool _disposed;
        private Controls.ListView _listView;
        private IVisualElementRenderer _headerRenderer;
        private IVisualElementRenderer _footerRenderer;
        private List<Gtk.Container> _cells;

        public ListViewRenderer()
        {
            _cells = new List<Gtk.Container>();
        }

        ListView ListView => Element;

        IListViewController Controller => Element;

        ITemplatedItemsView<Cell> TemplatedItemsView => Element;

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            if (e.OldElement != null)
            {
                e.OldElement.ScrollToRequested -= OnElementScrollToRequested;

                var templatedItems = ((ITemplatedItemsView<Cell>)e.OldElement).TemplatedItems;
                templatedItems.CollectionChanged -= OnCollectionChanged;
            }

            if (e.NewElement != null)
            {
                e.NewElement.ScrollToRequested += OnElementScrollToRequested;

                if (Control == null)
                {
                    _listView = new Controls.ListView();
                    _listView.OnItemTapped += OnItemTapped;
                    _listView.OnRefresh += OnRefresh;

                    SetNativeControl(_listView);
                }

                var templatedItems = ((ITemplatedItemsView<Cell>)e.NewElement).TemplatedItems;
                templatedItems.CollectionChanged += OnCollectionChanged;

                UpdateItems();
                UpdateGrouping();
                UpdateBackgroundColor();
                UpdateHeader();
                UpdateFooter();
                UpdateRowHeight();
                UpdateHasUnevenRows();
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
            else if (e.PropertyName == ListView.IsGroupingEnabledProperty.PropertyName)
                UpdateGrouping();
            else if (e.PropertyName == nameof(ListView.HeaderElement))
                UpdateHeader();
            else if (e.PropertyName == nameof(ListView.FooterElement))
                UpdateFooter();
            else if (e.PropertyName == ListView.RowHeightProperty.PropertyName)
                UpdateRowHeight();
            else if (e.PropertyName == ListView.HasUnevenRowsProperty.PropertyName)
                UpdateHasUnevenRows();
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

                _cells = null;

                if (Element != null)
                {
                    var templatedItems = TemplatedItemsView.TemplatedItems;
                    templatedItems.CollectionChanged -= OnCollectionChanged;
                }

                if (_headerRenderer != null)
                {
                    Platform.DisposeModelAndChildrenRenderers(_headerRenderer.Element);
                    _headerRenderer = null;
                }

                if (_listView != null)
                {
                    _listView.OnItemTapped -= OnItemTapped;
                    _listView.OnRefresh -= OnRefresh;
                }

                if (_footerRenderer != null)
                {
                    Platform.DisposeModelAndChildrenRenderers(_footerRenderer.Element);
                    _footerRenderer = null;
                }
            }
        }

        protected override void UpdateBackgroundColor()
        {
            base.UpdateBackgroundColor();

            if (_listView == null)
            {
                return;
            }

            if (Element.BackgroundColor.IsDefaultOrTransparent())
            {
                return;
            }

            var backgroundColor = Element.BackgroundColor.ToGtkColor();

            _listView.SetBackgroundColor(backgroundColor);
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);

            foreach (var cell in _cells)
            {
                cell.WidthRequest = allocation.Width;
                cell.QueueDraw();
            }
        }

        private void UpdateItems()
        {
            _cells.Clear();

            var items = TemplatedItemsView.TemplatedItems;

            if (!items.Any())
            {
                return;
            }

            bool grouping = Element.IsGroupingEnabled;

            if (grouping)
            {
                return;
            }

            foreach (var item in items)
            {
                var cell = GetCell(item);

                _cells.Add(cell);
            }

            _listView.Items = _cells;
        }

        private void UpdateHeader()
        {
            var header = Controller.HeaderElement;
            var headerView = (View)header;

            if (headerView != null)
            {
                _headerRenderer = Platform.CreateRenderer(headerView);
                Platform.SetRenderer(headerView, _headerRenderer);

                _listView.Header = _headerRenderer.Container;
            }
            else
            {
                ClearHeader();
            }
        }

        private void ClearHeader()
        {
            _listView.Header = null;
            if (_headerRenderer == null)
                return;
            Platform.DisposeModelAndChildrenRenderers(_headerRenderer.Element);
            _headerRenderer = null;
        }

        private void UpdateFooter()
        {
            var footer = Controller.FooterElement;
            var footerView = (View)footer;

            if (footerView != null)
            {
                _footerRenderer = Platform.CreateRenderer(footerView);
                Platform.SetRenderer(footerView, _footerRenderer);

                _listView.Footer = _footerRenderer.Container;
            }
            else
            {
                ClearFooter();
            }
        }

        private void ClearFooter()
        {
            _listView.Footer = null;
            if (_footerRenderer == null)
                return;
            Platform.DisposeModelAndChildrenRenderers(_footerRenderer.Element);
            _footerRenderer = null;
        }

        private void UpdateRowHeight()
        {
            var hasUnevenRows = Element.HasUnevenRows;

            if (hasUnevenRows)
            {
                return;
            }

            var rowHeight = Element.RowHeight;

            foreach (var cell in _cells)
            {
                var formsCell = GetXamarinFormsCell(cell);

                if (formsCell != null)
                {
                    var isGroupHeader = formsCell.GetIsGroupHeader<ItemsView<Cell>, Cell>();

                    if (isGroupHeader)
                        cell.HeightRequest = DefaultRowHeight;
                    else
                        cell.HeightRequest = rowHeight > 0 ? rowHeight : DefaultRowHeight;
                }
            }
        }

        private void UpdateHasUnevenRows()
        {
            var hasUnevenRows = Element.HasUnevenRows;

            if (hasUnevenRows)
            {
                foreach (var cell in _cells)
                {
                    var cellHeight = GetUnevenRowCellHeight(cell);

                    cell.HeightRequest = cellHeight > 0 ? cellHeight : DefaultRowHeight;
                }
            }
            else
            {
                UpdateRowHeight();
            }
        }

        private int GetUnevenRowCellHeight(Gtk.Container cell)
        {
            int height = -1;

            var formsCell = GetXamarinFormsCell(cell);

            if (formsCell != null)
            {
                height = Convert.ToInt32(formsCell.RenderHeight);
            }

            return height;
        }

        private Cell GetXamarinFormsCell(Gtk.Container cell)
        {
            try
            {
                var formsCell = cell
                   .GetType()
                   .GetProperty("Cell")
                   .GetValue(cell, null) as Cell;

                return formsCell;
            }
            catch
            {
                return null;
            }
        }

        private void UpdateSeparatorColor()
        {
            if (Element.SeparatorColor.IsDefaultOrTransparent())
            {
                if (_listView != null)
                {
                    _listView.SetSeparatorVisibility(false);
                }

                return;
            }

            var separatorColor = Element.SeparatorColor.ToGtkColor();

            if (_listView != null)
            {
                _listView.SetSeparatorVisibility(true);
                _listView.SetSeparatorColor(separatorColor);
            }
        }

        private void UpdateSeparatorVisibility()
        {
            if (_listView == null)
            {
                return;
            }

            var visibility = Element.SeparatorVisibility;

            switch (visibility)
            {
                case SeparatorVisibility.Default:
                    _listView.SetSeparatorVisibility(true);
                    break;
                case SeparatorVisibility.None:
                    _listView.SetSeparatorVisibility(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateIsRefreshing()
        {
            var refreshing = Element.IsRefreshing;

            if (_listView != null)
            {
                _listView.UpdateIsRefreshing(refreshing);
            }
        }

        private void UpdatePullToRefreshEnabled()
        {
            var isPullToRefreshEnabled = Element.IsPullToRefreshEnabled;

            if (_listView != null)
            {
                _listView.UpdatePullToRefreshEnabled(isPullToRefreshEnabled);
            }
        }

        private void UpdateGrouping()
        {
            var templatedItems = TemplatedItemsView.TemplatedItems;

            if (!templatedItems.Any())
            {
                return;
            }

            bool grouping = Element.IsGroupingEnabled;

            if (grouping)
            {
                _cells.Clear();

                int index = 0;
                foreach (var groupItem in templatedItems)
                {
                    var group = templatedItems.GetGroup(index);

                    if (group.Count != 0)
                    {
                        if (group.HeaderContent != null)
                            _cells.Add(GetCell(group.HeaderContent));
                        else
                            _cells.Add(CreateEmptyHeader());

                        foreach (var item in group.ToList())
                        {
                            _cells.Add(GetCell(item as Cell));
                        }
                    }

                    index++;
                }

                _listView.Items = _cells;
            }
        }

        private Cells.TextCell CreateEmptyHeader()
        {
            return new Cells.TextCell(
                string.Empty,
                Color.Black.ToGtkColor(),
                string.Empty,
                Color.Black.ToGtkColor());
        }

        private Gtk.Container GetCell(Cell cell)
        {
            var renderer =
                (CellRenderer)Registrar.Registered.GetHandler<IRegisterable>(cell.GetType());

            var realCell = renderer.GetCell(cell, null, _listView);

            return realCell;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                return;
            }

            bool grouping = Element.IsGroupingEnabled;

            if (grouping)
                UpdateGrouping();
            else
                UpdateItems();
        }

        private void OnRefresh(object sender, EventArgs args)
        {
            if(Element == null)
            {
                return;
            }

            var refeshCommand = Element.RefreshCommand;

            if (refeshCommand != null)
            {
                refeshCommand.Execute(null);
            }
        }

        private void OnElementScrollToRequested(object sender, ScrollToRequestedEventArgs e)
        {
            Cell cell;
            int position = 0;
            int height = 0;
            var scrollArgs = (ITemplatedItemsListScrollToRequestedEventArgs)e;

            var templatedItems = TemplatedItemsView.TemplatedItems;

            if (Element.IsGroupingEnabled)
            {
                var results = templatedItems.GetGroupAndIndexOfItem(scrollArgs.Group, scrollArgs.Item);

                if (results.Item1 == -1 || results.Item2 == -1)
                    return;

                var group = templatedItems.GetGroup(results.Item1);
                cell = group[results.Item2];
                position = templatedItems.GetGlobalIndexForGroup(group);
            }
            else
            {
                position = templatedItems.GetGlobalIndexOfItem(scrollArgs.Item);

                if (position == -1)
                    return;

                cell = templatedItems[position];
            }

            foreach (var item in Control.Items)
            {
                height += item.Allocation.Height;

                if (((CellBase)item).Cell == cell)
                {
                    break;
                }
            }

            var cellHeight = (int)cell.RenderHeight;
            var y = 0;

            if (e.Position == ScrollToPosition.MakeVisible)
            {
                Control.Vadjustment.Value = height;
                return;
            }

            var listHeight = _listView.Allocation.Height; 

            if (e.Position == ScrollToPosition.Start)
                y = height;
            if (e.Position == ScrollToPosition.Center)
                y = height - listHeight / 2;
            else if (e.Position == ScrollToPosition.End)
                y = height - listHeight + cellHeight;

            Control.Vadjustment.Value = y;
        }

        private void OnItemTapped(object sender, Controls.ItemTappedEventArgs args)
        {
            if (Element == null)
                return;

            var templatedItems = TemplatedItemsView.TemplatedItems;
            var index = -1;

            if (Element.IsGroupingEnabled)
            {
                int selectedItemIndex = templatedItems.GetGlobalIndexOfItem(args.Item);
                var leftOver = 0;
                int groupIndex = templatedItems.GetGroupIndexFromGlobal(selectedItemIndex, out leftOver);

                index = selectedItemIndex - (groupIndex + 1);
            }
            else
            {
                index = templatedItems.GetGlobalIndexOfItem(args.Item);
            }

            if (index > -1)
            {
                Element.NotifyRowTapped(index, cell: null);
            }
        }
    }
}