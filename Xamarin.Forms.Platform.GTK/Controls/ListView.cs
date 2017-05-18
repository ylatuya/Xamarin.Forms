using Gtk;
using System;
using System.Collections.Generic;
using Xamarin.Forms.Platform.GTK.Cells;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Controls
{
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
        private VBox _root;
        private EventBox _header;
        private VBox _list;
        private EventBox _footer;
        private IEnumerable<Widget> _cells;
        private List<ListViewSeparator> _separators;
        private object _selectedItem;

        public delegate void SelectedItemEventHandler(object sender, SelectedItemEventArgs args);

        public event SelectedItemEventHandler OnSelectedItemChanged = null;

        public ListView()
        {
            BuildListView();
        }

        public EventBox Header
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

        public EventBox Footer
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
            set { _selectedItem = value; }
        }

        public void SetBackgroundColor(Gdk.Color backgroundColor)
        {
            if (_root != null)
            {
                _root.ModifyBg(StateType.Normal, backgroundColor);
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

        private void BuildListView()
        {
            CanFocus = true;
            ShadowType = ShadowType.None;
            BorderWidth = 0;
            HscrollbarPolicy = PolicyType.Never;
            VscrollbarPolicy = PolicyType.Automatic;
            
            _root = new VBox();

            // Header
            _header = new EventBox();
            _root.PackStart(_header, false, false, 0);

            // List
            _list = new VBox();
            _separators = new List<ListViewSeparator>();
            _root.PackStart(_list, true, true, 0);

            // Footer
            _footer = new EventBox();
            _root.PackStart(_footer, false, false, 0);

            Viewport viewPort = new Viewport();
            viewPort.ShadowType = ShadowType.None;
            viewPort.BorderWidth = 0;
            viewPort.Add(_root);

            Add(viewPort);

            ShowAll();
        }

        private void RefreshHeader(EventBox header)
        {
            _header = header;

            if (_header != null)
            {
                _header.ShowAll();
            }
        }

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
                            OnSelectedItemChanged?.Invoke(this, new SelectedItemEventArgs(SelectedItem));
                        }
                    };

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
                foreach (var cell in _list.Children)
                {
                    cell.Destroy();
                }
            }

            if (_separators != null)
            {
                _separators.Clear();
            }
        }

        private void RefreshFooter(EventBox footer)
        {
            _footer = footer;

            if (_footer != null)
            {
                _footer.ShowAll();
            }
        }
    }
}