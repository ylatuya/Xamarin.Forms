using System;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class CellRenderer : IRegisterable
    {
        static readonly BindableProperty s_realCellProperty =
            BindableProperty.CreateAttached("RealCell", typeof(Gtk.CellRenderer),
                typeof(Cell), null);

        EventHandler _onForceUpdateSizeRequested;

        public virtual Gtk.CellRenderer GetCell(Cell item, Gtk.CellRenderer reusableView, Gtk.TreeView tv)
        {
            var tvc = reusableView as Gtk.CellRenderer ?? new Gtk.CellRenderer(IntPtr.Zero);

            WireUpForceUpdateSizeRequested(item, tvc, tv);

            UpdateBackground(tvc, item);

            return tvc;
        }

        protected void UpdateBackground(Gtk.CellRenderer tableViewCell, Cell cell)
        {
            var bgColor = Color.White.ToGtkColor();
            var element = cell.RealParent as VisualElement;

            if (element != null)
                bgColor = element.BackgroundColor == Color.Default ? bgColor : element.BackgroundColor.ToGtkColor();

            UpdateBackgroundChild(cell, bgColor);

            tableViewCell.CellBackgroundGdk = bgColor;
        }

        protected void WireUpForceUpdateSizeRequested(ICellController cell, Gtk.CellRenderer nativeCell, Gtk.TreeView tableView)
        {
            cell.ForceUpdateSizeRequested -= _onForceUpdateSizeRequested;

            _onForceUpdateSizeRequested = (sender, e) =>
            {
                //TODO: Implement ForceUpdateSize
            };

            cell.ForceUpdateSizeRequested += _onForceUpdateSizeRequested;
        }

        internal virtual void UpdateBackgroundChild(Cell cell, Gdk.Color backgroundColor)
        {

        }

        internal static Gtk.CellRenderer GetRealCell(BindableObject cell)
        {
            return (Gtk.CellRenderer)cell.GetValue(s_realCellProperty);
        }

        internal static void SetRealCell(BindableObject cell, Gtk.CellRenderer renderer)
        {
            cell.SetValue(s_realCellProperty, renderer);
        }
    }
}