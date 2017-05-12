using Gtk;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class SwitchCellRenderer : CellRenderer
    {
        public override Container GetCell(Cell item, Container reusableView, Controls.ListView listView)
        {
            var switchCell = (Xamarin.Forms.SwitchCell)item;

            var text = switchCell.Text ?? string.Empty;
            var on = switchCell.On;

            var gtkSwitchCell = 
                reusableView as SwitchCell ??
                new SwitchCell(
                    text,
                    on);

            WireUpForceUpdateSizeRequested(item, gtkSwitchCell, listView);
            UpdateBackground(gtkSwitchCell, item);

            return gtkSwitchCell;
        }
    }
}