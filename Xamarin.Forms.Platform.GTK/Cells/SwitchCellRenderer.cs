using System.ComponentModel;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class SwitchCellRenderer : CellRenderer
    {
        protected override Gtk.Container GetCellWidgetInstance(Cell item)
        {
            var switchCell = (Xamarin.Forms.SwitchCell)item;

            var text = switchCell.Text ?? string.Empty;
            var on = switchCell.On;

            return new SwitchCell(
                    text,
                    on);
        }

        protected override void CellPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.CellPropertyChanged(sender, args);

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
    }
}