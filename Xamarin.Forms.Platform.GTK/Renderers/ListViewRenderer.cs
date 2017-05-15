using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Platform.GTK.Extensions;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;

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

        IListViewController Controller => Element;

        ITemplatedItemsView<Cell> TemplatedItemsView => Element;

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            if (e.OldElement != null)
            {
                var templatedItems = ((ITemplatedItemsView<Cell>)e.OldElement).TemplatedItems;
                templatedItems.CollectionChanged -= OnCollectionChanged;
            }

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    _listView = new Controls.ListView();
                    _listView.OnSelectedItemChanged += OnSelectedItemChanged;
                    SetNativeControl(_listView);
                }

                var templatedItems = ((ITemplatedItemsView<Cell>)e.NewElement).TemplatedItems;
                templatedItems.CollectionChanged += OnCollectionChanged;

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
            else if (e.PropertyName.Equals("HeaderElement", StringComparison.InvariantCultureIgnoreCase))
                UpdateHeader();
            else if (e.PropertyName.Equals("FooterElement", StringComparison.InvariantCultureIgnoreCase))
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
                    _listView.OnSelectedItemChanged -= OnSelectedItemChanged;
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

            if (Element.BackgroundColor.IsDefault)
            {
                return;
            }

            var backgroundColor = Element.BackgroundColor.ToGtkColor();

            _listView.SetBackgroundColor(backgroundColor);
        }

        private void UpdateItems()
        {
            _cells.Clear();

            var items = TemplatedItemsView.TemplatedItems;

            if (!items.Any())
            {
                return;
            }

            foreach (var item in items)
            {
                var renderer =
                    (Cells.CellRenderer)Internals.Registrar.Registered.GetHandler<IRegisterable>(item.GetType());
                var nativeCell = renderer.GetCell(item, null, _listView);

                _cells.Add(nativeCell);
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

        //TODO: Implement RowHeight
        private void UpdateRowHeight()
        {
            var rowHeight = Element.RowHeight;

            foreach (var cell in _cells)
            {
                cell.HeightRequest = rowHeight > 0 ? rowHeight : DefaultRowHeight;
            }
        }

        //TODO: Implement SeparatorColor
        private void UpdateSeparatorColor()
        {

        }

        //TODO: Implement SeparatorVisibility
        private void UpdateSeparatorVisibility()
        {
            if (Element.SeparatorVisibility == SeparatorVisibility.Default)
            {

            }
            else
            {

            }
        }

        //TODO: Implement UpdateIsRefreshing
        private void UpdateIsRefreshing()
        {

        }

        //TODO: Implement PullToRefresh
        private void UpdatePullToRefreshEnabled()
        {

        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateItems();
        }

        private void OnSelectedItemChanged(object sender, Controls.SelectedItemEventArgs args)
        {
            if (_listView != null && _listView.SelectedItem != null)
            {
                ((IElementController)Element).SetValueFromRenderer(
                    ListView.SelectedItemProperty,
                    _listView.SelectedItem);
            }
        }
    }
}