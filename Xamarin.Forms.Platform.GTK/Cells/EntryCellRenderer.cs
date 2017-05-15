using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class EntryCellRenderer : CellRenderer
    {
        public override Gtk.Container GetCell(Cell item, Gtk.Container reusableView, Controls.ListView listView)
        {
            var entryCell = (Xamarin.Forms.EntryCell)item;

            var label = entryCell.Label ?? string.Empty;
            var labelColor = entryCell.LabelColor.ToGtkColor();
            var text = entryCell.Text;
            var placeholder = entryCell.Placeholder;

            var gtkEntryCell =
                reusableView as EntryCell ??
                new EntryCell(
                    label,
                    labelColor, 
                    text,
                    placeholder);

            if (gtkEntryCell.Cell != null)
                gtkEntryCell.Cell.PropertyChanged -= gtkEntryCell.HandlePropertyChanged;

            gtkEntryCell.Cell = entryCell;

            entryCell.PropertyChanged += gtkEntryCell.HandlePropertyChanged;
            gtkEntryCell.PropertyChanged = HandlePropertyChanged;

            WireUpForceUpdateSizeRequested(item, gtkEntryCell, listView);
            UpdateBackground(gtkEntryCell, item);

            return gtkEntryCell;
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
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

        internal override void UpdateBackgroundChild(Cell cell, Gdk.Color backgroundColor)
        {
            base.UpdateBackgroundChild(cell, backgroundColor);
        }
    }
}