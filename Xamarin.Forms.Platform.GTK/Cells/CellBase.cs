using Gtk;
using System;
using System.ComponentModel;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class CellBase : EventBox
    {
        private Cell _cell;

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

                if (_cell != null)
                    Device.BeginInvokeOnMainThread(_cell.SendAppearing);
            }
        }

        public void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
