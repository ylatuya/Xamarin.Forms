using System.ComponentModel;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class EntryCellRenderer : CellRenderer
    {
        public override Gtk.Container GetCell(Cell item, Gtk.Container reusableView, Controls.ListView listView)
        {
            var entryCell = (Xamarin.Forms.EntryCell)item;

            var text = entryCell.Text ?? string.Empty;
            var placeholder = entryCell.Placeholder;

            var gtkEntryCell =
                reusableView as EntryCell ??
                new EntryCell(
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
            var switchCell = (Xamarin.Forms.EntryCell)gtkEntryCell.Cell;

            if (args.PropertyName == Xamarin.Forms.EntryCell.TextProperty.PropertyName)
            {
                gtkEntryCell.Text = switchCell.Text ?? string.Empty;
            }
            else if (args.PropertyName == Xamarin.Forms.EntryCell.PlaceholderProperty.PropertyName)
            {
                gtkEntryCell.Placeholder = switchCell.Placeholder;
            }
        }

        internal override void UpdateBackgroundChild(Cell cell, Gdk.Color backgroundColor)
        {
            base.UpdateBackgroundChild(cell, backgroundColor);
        }
    }
}