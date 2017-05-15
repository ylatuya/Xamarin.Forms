using Gtk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms.Platform.GTK.Cells;

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

    public class ListView : ScrolledWindow
    {
        private VBox _root;
        private EventBox _header;
        private VBox _list;
        private EventBox _footer;
        private IEnumerable<Widget> _cells;
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
                if (_cells != value)
                {
                    _cells = value;
                    RefreshItems(_cells);
                }
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

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            Debug.WriteLine($"{allocation.Height} x {allocation.Width}");
            base.OnSizeAllocated(allocation);
        }

        private void BuildListView()
        {
            CanFocus = true;
            ShadowType = ShadowType.None;
            HscrollbarPolicy = PolicyType.Never;
            VscrollbarPolicy = PolicyType.Automatic;
            
            _root = new VBox();

            // Header
            _header = new EventBox();
            _root.PackStart(_header, false, false, 0);

            // List
            _list = new VBox();
            _root.PackStart(_list, true, true, 0);

            // Footer
            _footer = new EventBox();
            _root.PackStart(_footer, false, false, 0);

            Viewport viewPort = new Viewport();
            viewPort.ShadowType = ShadowType.None;
            viewPort.Add(_root);

            Add(viewPort);
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
            foreach (var item in items)
            {
                if (item != null)
                {
                    item.ButtonPressEvent += (sender, args) =>
                    {
                        var gtkCell = sender as CellBase;

                        if (gtkCell != null)
                        {
                            var selectedItem = gtkCell.Cell.BindingContext;
                            SelectedItem = selectedItem;
                            OnSelectedItemChanged?.Invoke(this, new SelectedItemEventArgs(SelectedItem));
                        }
                    };

                    _list.PackStart(item, false, false, 0);
                }
            }

            _list.ShowAll();
        }

        private void RefreshFooter(EventBox footer)
        {
            _footer = footer;

            if (_footer != null)
            {
                _footer.ShowAll();
            }
        }

        public void SetBackgroundColor(Gdk.Color backgroundColor)
        {
            if (_root != null)
            {
                _root.ModifyBg(StateType.Normal, backgroundColor);
            }
        }
    }
}