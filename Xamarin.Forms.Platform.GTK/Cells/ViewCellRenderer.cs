using Gtk;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class ViewCellRenderer : CellRenderer
    {
        public override Container GetCell(Cell item, Container reusableView, Controls.ListView listView)
        {
            var viewCell = (Xamarin.Forms.ViewCell)item;

            var cell = reusableView as ViewCell;

            if (cell == null)
                cell = new ViewCell();

            cell.Cell = viewCell;

            SetRealCell(item, cell);
            WireUpForceUpdateSizeRequested(item, cell, listView);
            UpdateBackground(cell, item);
            UpdateIsEnabled(cell, viewCell);

            return cell;
        }

        static void UpdateIsEnabled(ViewCell cell, Xamarin.Forms.ViewCell viewCell)
        {
            //TODO: Implement IsEnabled on ViewCell
        }
    }
}
