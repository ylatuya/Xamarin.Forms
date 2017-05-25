using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class EntryCellRenderer : CellRenderer
    {
        protected override Gtk.Container GetCellWidgetInstance(Cell item)
        {
            var entryCell = (Xamarin.Forms.EntryCell)item;

            var label = entryCell.Label ?? string.Empty;
            var labelColor = entryCell.LabelColor.ToGtkColor();
            var text = entryCell.Text;
            var placeholder = entryCell.Placeholder;

            return new EntryCell(
                    label,
                    labelColor,
                    text,
                    placeholder);
        }

        protected override void CellPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.CellPropertyChanged(sender, args);

            var gtkEntryCell = (EntryCell)sender;
            var entryCell = (Xamarin.Forms.EntryCell)gtkEntryCell.Cell;

            if (args.PropertyName == Xamarin.Forms.EntryCell.LabelProperty.PropertyName)
            {
                gtkEntryCell.Label = entryCell.Label ?? string.Empty;
            }
            else if (args.PropertyName == Xamarin.Forms.EntryCell.LabelColorProperty.PropertyName)
            {
                gtkEntryCell.LabelColor = entryCell.LabelColor.ToGtkColor();
            }
            else if (args.PropertyName == Xamarin.Forms.EntryCell.TextProperty.PropertyName)
            {
                gtkEntryCell.Text = entryCell.Text;
            }
            else if (args.PropertyName == Xamarin.Forms.EntryCell.PlaceholderProperty.PropertyName)
            {
                gtkEntryCell.Placeholder = entryCell.Placeholder;
            }
        }
    }
}