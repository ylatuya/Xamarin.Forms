using System.ComponentModel;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class ViewCellRenderer : CellRenderer
    {
        public override Gtk.Container GetCell(Cell item, Gtk.Container reusableView, Controls.ListView listView)
        {
            var viewCell = (Xamarin.Forms.ViewCell)item;

            var cell = reusableView as ViewCell;

            if (cell == null)
                cell = new ViewCell();
            else
                cell.Cell.PropertyChanged -= ViewCellPropertyChanged;

            viewCell.PropertyChanged += ViewCellPropertyChanged;
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

        static void ViewCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewCell = (Xamarin.Forms.ViewCell)sender;
            var realCell = (ViewCell)GetRealCell(viewCell);

            if (e.PropertyName == Cell.IsEnabledProperty.PropertyName)
                UpdateIsEnabled(realCell, viewCell);
        }
    }
}