using System.ComponentModel;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class SwitchCellRenderer : CellRenderer
    {
        public override Gtk.Container GetCell(Cell item, Gtk.Container reusableView, Controls.ListView listView)
        {
            var switchCell = (Xamarin.Forms.SwitchCell)item;

            var text = switchCell.Text ?? string.Empty;
            var on = switchCell.On;

            var gtkSwitchCell = 
                reusableView as SwitchCell ??
                new SwitchCell(
                    text,
                    on);

            if (gtkSwitchCell.Cell != null)
                gtkSwitchCell.Cell.PropertyChanged -= gtkSwitchCell.HandlePropertyChanged;

            gtkSwitchCell.Cell = switchCell;

            switchCell.PropertyChanged += gtkSwitchCell.HandlePropertyChanged;
            gtkSwitchCell.PropertyChanged = HandlePropertyChanged;

            WireUpForceUpdateSizeRequested(item, gtkSwitchCell, listView);
            UpdateBackground(gtkSwitchCell, item);

            return gtkSwitchCell;
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var gtkSwitchCell = (SwitchCell)sender;
            var switchCell = (Xamarin.Forms.SwitchCell)gtkSwitchCell.Cell;

            if (args.PropertyName == Xamarin.Forms.SwitchCell.TextProperty.PropertyName)
            {
                gtkSwitchCell.Text = switchCell.Text ?? string.Empty;
            }
            else if (args.PropertyName == Xamarin.Forms.SwitchCell.OnProperty.PropertyName)
            {
                gtkSwitchCell.On = switchCell.On;
            }
        }

        internal override void UpdateBackgroundChild(Cell cell, Gdk.Color backgroundColor)
        {
            base.UpdateBackgroundChild(cell, backgroundColor);
        }
    }
}