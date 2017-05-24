using Gtk;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class CellBase : EventBox
    {
        private Cell _cell;
        private IList<MenuItem> _contextActions;
        private Menu _menu;

        public Action<object, PropertyChangedEventArgs> PropertyChanged;

        public Cell Cell
        {
            get { return _cell; }
            set
            {
                if (_cell == value)
                    return;

                if (_cell != null)
                    Device.BeginInvokeOnMainThread(_cell.SendDisappearing);

                _cell = value;
                _contextActions = Cell.ContextActions;

                if (_contextActions.Any())
                {
                    AddMenu();
                }

                if (_cell != null)
                    Device.BeginInvokeOnMainThread(_cell.SendAppearing);
            }
        }

        public void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void AddMenu()
        {
            _menu = new Menu();
            Add(_menu);
            SetupContextMenu();
        }

        private void SetupContextMenu()
        {
            if (Cell == null)
                return;

            if (!Cell.HasContextActions)
            {
                ButtonReleaseEvent -= OnClick;

                if (_contextActions != null)
                {
                    ((INotifyCollectionChanged)_contextActions).CollectionChanged -= OnContextActionsChanged;
                    _contextActions = null;
                }

                RemoveMenu();

                return;
            }

            ButtonReleaseEvent += OnClick;
        }

        private void RemoveMenu()
        {
            this.RemoveFromContainer(_menu);
        }

        private void OnClick(object o, ButtonReleaseEventArgs args)
        {
            if (args.Event.Button != 3)  // Right button
            {
                return;
            }

            OpenContextMenu();
        }

        private void OpenContextMenu()
        {
            if (_menu.Children.Count() == 0)
            {
                SetupMenuItems(_menu);

                ((INotifyCollectionChanged)Cell.ContextActions).CollectionChanged += OnContextActionsChanged;
            }

            _menu.ShowAll();
            _menu.Popup();
        }

        private void SetupMenuItems(Menu menu)
        {
            foreach (MenuItem item in Cell.ContextActions)
            {
                var menuItem = new Gtk.MenuItem(item.Text);

                menuItem.ButtonPressEvent += (sender, args) =>
                {
                    item.Command?.Execute(item.CommandParameter);
                };

                menu.Add(menuItem);
            }
        }

        private void OnContextActionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_menu != null)
            {
                foreach (var menuItem in _menu.Children)
                {
                    menuItem.RemoveFromContainer(_menu);
                }

                SetupMenuItems(_menu);
            }
        }
    }
}
