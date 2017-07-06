using Gtk;
using System;
using System.Collections.Generic;
using Xamarin.Forms.Platform.GTK.Cells;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class ItemTappedEventArgs : EventArgs
    {
        private object _item;

        public object Item
        {
            get
            {
                return _item;
            }
        }

        public ItemTappedEventArgs(object item)
        {
            _item = item;
        }
    }

    public class SelectedItemEventArgs : EventArgs
    {
        private object _selectedItem;

        public object SelectedItem
        {
            get
            {
                return _selectedItem;
            }
        }

        public SelectedItemEventArgs(object selectedItem)
        {
            _selectedItem = selectedItem;
        }
    }

    public class ListViewSeparator : EventBox
    {
        public ListViewSeparator()
        {
            HeightRequest = 1;
            ModifyBg(StateType.Normal, Color.Gray.ToGtkColor());    // Default Color: Gray
            VisibleWindow = false;
        }
    }

    public class ListView : ScrolledWindow
    {
        private const int RefreshHeight = 48;

        private VBox _root;
        private EventBox _headerContainer;
        private Widget _header;
        private VBox _list;
        private EventBox _footerContainer;
        private Widget _footer;
        private Viewport _viewPort;
        private IEnumerable<Widget> _cells;
        private List<ListViewSeparator> _separators;
        private object _selectedItem;
        private Table _refreshHeader;
        private ImageButton _refreshButton;
        private Gtk.Label _refreshLabel;
        private bool _isPullToRequestEnabled;
        private bool _refreshing;

        public delegate void ItemTappedEventHandler(object sender, ItemTappedEventArgs args);
        public event ItemTappedEventHandler OnItemTapped = null;

        public delegate void SelectedItemEventHandler(object sender, SelectedItemEventArgs args);
        public event SelectedItemEventHandler OnSelectedItemChanged = null;

        public delegate void RefreshEventHandler(object sender, EventArgs args);
        public event RefreshEventHandler OnRefresh = null;

        public ListView()
        {
            BuildListView();
        }

        public Widget Header
        {
            get
            {
                return _header;
            }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    RefreshHeader(_header);
                }
            }
        }

        public IEnumerable<Widget> Items
        {
            get
            {
                return _cells;
            }
            set
            {
                _cells = value;
                RefreshItems(_cells);
            }
        }

        public Widget Footer
        {
            get
            {
                return _footer;
            }
            set
            {
                if (_footer != value)
                {
                    _footer = value;
                    RefreshFooter(_footer);
                }
            }
        }

        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (value != _selectedItem)
                {
                    _selectedItem = value;
                    OnSelectedItemChanged?.Invoke(this, new SelectedItemEventArgs(_selectedItem));
                }
            }
        }

        public bool IsPullToRequestEnabled
        {
            get { return _isPullToRequestEnabled; }
            set { _isPullToRequestEnabled = value; }
        }

        public bool Refreshing
        {
            get { return _refreshing; }
            set { _refreshing = value; }
        }

        public void SetBackgroundColor(Gdk.Color backgroundColor)
        {
            if (_root != null)
            {
                _root.ModifyBg(StateType.Normal, backgroundColor);
                _viewPort.ModifyBg(StateType.Normal, backgroundColor);
            }
        }

        public void SetSeparatorColor(Gdk.Color separatorColor)
        {
            foreach (var separator in _separators)
            {
                separator.ModifyBg(StateType.Normal, separatorColor);
                separator.VisibleWindow = true;
            }
        }

        public void SetSeparatorVisibility(bool visible)
        {
            foreach (var separator in _separators)
            {
                separator.HeightRequest = visible ? 1 : 0;
            }
        }

        public void UpdatePullToRefreshEnabled(bool isPullToRequestEnabled)
        {
            IsPullToRequestEnabled = isPullToRequestEnabled;

            if(_refreshHeader == null)
            {
                return;
            }

            if (IsPullToRequestEnabled)
            {
                _root.RemoveFromContainer(_refreshHeader);
                _root.PackStart(_refreshHeader, false, false, 0);
                _root.ReorderChild(_refreshHeader, 0);
            }
            else
            {
                _root.RemoveFromContainer(_refreshHeader);
            }
        }

        public void UpdateIsRefreshing(bool refreshing)
        {
            Refreshing = refreshing;

            if(Refreshing)
            {
                _refreshHeader.Attach(_refreshLabel, 0, 1, 0, 1);
            }
            else
            {
                _refreshHeader.RemoveFromContainer(_refreshLabel);
            }

            _refreshButton.Visible = !Refreshing;
            _refreshLabel.Visible = Refreshing;
        }

        public void SetSeletedItem(object selectedItem)
        {
            if(selectedItem == null)
            {
                return;
            }

            SelectedItem = selectedItem;
        }

        private void BuildListView()
        {
            CanFocus = true;
            ShadowType = ShadowType.None;
            BorderWidth = 0;
            HscrollbarPolicy = PolicyType.Never;
            VscrollbarPolicy = PolicyType.Automatic;

            _root = new VBox();
            _refreshHeader = new Table(1, 1, true);
            _refreshHeader.HeightRequest = RefreshHeight;

            // Refresh Loading
            _refreshLabel = new Gtk.Label();
            _refreshLabel.Text = "Loading";

            // Refresh Button
            _refreshButton = new ImageButton();
            _refreshButton.LabelWidget.SetTextFromSpan(
                new Span()
                {
                    Text = "Refresh"
                });
            _refreshButton.ImageWidget.Stock = Stock.Refresh;
            _refreshButton.SetImagePosition(PositionType.Left);
            _refreshButton.Clicked += (sender, args) =>  
            {
                OnRefresh?.Invoke(this, new EventArgs());
            };

            _refreshHeader.Attach(_refreshButton, 0, 1, 0, 1);

            _root.PackStart(_refreshHeader, false, false, 0);

            // Header
            _headerContainer = new EventBox();
            _root.PackStart(_headerContainer, false, false, 0);

            // List
            _list = new VBox();
            _separators = new List<ListViewSeparator>();
            _root.PackStart(_list, true, true, 0);

            // Footer
            _footerContainer = new EventBox();
            _root.PackStart(_footerContainer, false, false, 0);

            _viewPort = new Viewport();
            _viewPort.ShadowType = ShadowType.None;
            _viewPort.BorderWidth = 0;
            _viewPort.Add(_root);

            Add(_viewPort);

            ShowAll(); 
        }
       
        private void RefreshHeader(Widget newHeader)
        {
            if (_headerContainer != null)
            {
                foreach(var child in _headerContainer.Children)
                {
                    _headerContainer.RemoveFromContainer(child);
                    child.Destroy();
                }
            }

            _header = newHeader;
            _headerContainer.Add(_header);
            _header.ShowAll();
        }

        private void RefreshFooter(Widget newFooter)
        {
            if (_footerContainer != null)
            {
                foreach (var child in _footerContainer.Children)
                {
                    _footerContainer.RemoveFromContainer(child);
                    child.Destroy();
                }
            }

            _footer = newFooter;
            _footerContainer.Add(_footer);
            _footer.ShowAll();
        }

        // TODO: Improve cell creation for the highest performance
        private void RefreshItems(IEnumerable<Widget> items)
        {
            ClearList();

            foreach (var item in items)
            {
                if (item != null)
                {
                    item.ButtonPressEvent += (sender, args) =>
                    {
                        var gtkCell = sender as CellBase;

                        if (gtkCell != null && gtkCell.Cell != null)
                        {
                            var selectedItem = gtkCell.Cell.BindingContext;
                            SelectedItem = selectedItem;
                            OnItemTapped?.Invoke(this, new ItemTappedEventArgs(SelectedItem));
                        }
                    };

                    var itemContainer = item as EventBox;

                    if (itemContainer != null)
                    {
                        itemContainer.VisibleWindow = false;
                    }

                    _list.PackStart(item, false, false, 0);

                    var separator = new ListViewSeparator();
                    _separators.Add(separator);
                    _list.PackStart(separator, false, false, 0);
                }
            }

            _list.ShowAll();
        }

        private void ClearList()
        {
            if (_list != null)
            {
                foreach (var child in _list.Children)
                {
                    _list.RemoveFromContainer(child);
                    child.Destroy();
                }
            }

            if (_separators != null)
            {
                _separators.Clear();
            }
        }
    }
}