namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class ViewCellRenderer : CellRenderer
    {
        protected override Gtk.Container GetCellWidgetInstance(Cell item)
        {
            return new ViewCell();
        }
    }
}