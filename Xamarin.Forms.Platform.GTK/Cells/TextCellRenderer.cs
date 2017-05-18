using Gtk;
using Xamarin.Forms.Platform.GTK.Extensions;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class TextCellRenderer : CellRenderer
    {
        public override Container GetCell(Cell item, Container reusableView, Controls.ListView listView)
        {
            var textCell = (Xamarin.Forms.TextCell)item;

            var text = textCell.Text ?? string.Empty;
            var textColor = textCell.TextColor.ToGtkColor();
            var detail = textCell.Detail ?? string.Empty;
            var detailColor = textCell.DetailColor.ToGtkColor();

            var gtkTextCell =
                reusableView as TextCell ??
                new TextCell(
                    text,
                    textColor,
                    detail,
                    detailColor);

            if (gtkTextCell.Cell != null)
                gtkTextCell.Cell.PropertyChanged -= gtkTextCell.HandlePropertyChanged;

            gtkTextCell.Cell = textCell;
            gtkTextCell.IsGroupHeader = textCell.GetIsGroupHeader<ItemsView<Cell>, Cell>();

            textCell.PropertyChanged += gtkTextCell.HandlePropertyChanged;
            gtkTextCell.PropertyChanged = HandlePropertyChanged;

            WireUpForceUpdateSizeRequested(item, gtkTextCell, listView);
            UpdateBackground(gtkTextCell, item);

            return gtkTextCell;
        }

        private void HandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            var gtkTextCell = (TextCell)sender;
            var textCell = (Xamarin.Forms.TextCell)gtkTextCell.Cell;

            if (args.PropertyName == Xamarin.Forms.TextCell.TextProperty.PropertyName)
            {
                gtkTextCell.Text = textCell.Text ?? string.Empty;
            }
            else if (args.PropertyName == Xamarin.Forms.TextCell.DetailProperty.PropertyName)
            {
                gtkTextCell.Detail = textCell.Detail ?? string.Empty;
            }
            else if (args.PropertyName == Xamarin.Forms.TextCell.TextColorProperty.PropertyName)
                gtkTextCell.TextColor = textCell.TextColor.ToGtkColor();
            else if (args.PropertyName == Xamarin.Forms.TextCell.DetailColorProperty.PropertyName)
                gtkTextCell.DetailColor = textCell.DetailColor.ToGtkColor();
        }

        internal override void UpdateBackgroundChild(Cell cell, Gdk.Color backgroundColor)
        {
            base.UpdateBackgroundChild(cell, backgroundColor);
        }
    }
}