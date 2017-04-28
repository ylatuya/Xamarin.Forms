using Gtk;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class TextCellRenderer : CellRenderer
    {
        public override Gtk.CellRenderer GetCell(Cell item, Gtk.CellRenderer reusableView, TreeView tv)
        {
            var textCell = (TextCell)item;

            var tvc = reusableView as GtkTextCell ?? new GtkTextCell();

            tvc.Text = textCell.Text ?? string.Empty;
            tvc.TextColor = textCell.TextColor.ToGtkColor();
            tvc.Detail = textCell.Detail ?? string.Empty;
            tvc.DetailColor = textCell.DetailColor.ToGtkColor();

            WireUpForceUpdateSizeRequested(item, tvc, tv);
            UpdateBackground(tvc, item);

            return tvc;
        }

        internal override void UpdateBackgroundChild(Cell cell, Gdk.Color backgroundColor)
        {
            base.UpdateBackgroundChild(cell, backgroundColor);
        }
    }
}