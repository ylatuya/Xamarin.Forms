using System;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class CellRenderer : IRegisterable
    {
        static readonly BindableProperty RealCellProperty =
            BindableProperty.CreateAttached("RealCell", typeof(Gtk.Container),
                typeof(Cell), null);

        EventHandler _onForceUpdateSizeRequested;

        public virtual Gtk.Container GetCell(Cell item, Gtk.Container reusableView, Controls.ListView listView)
        {
            var tvc = reusableView as Gtk.Container ?? new Gtk.Container(IntPtr.Zero);

            WireUpForceUpdateSizeRequested(item, tvc, listView);

            UpdateBackground(tvc, item);

            return tvc;
        }

        protected void UpdateBackground(Gtk.Container tableViewCell, Cell cell)
        {
            var bgColor = Color.White.ToGtkColor();
            var element = cell.RealParent as VisualElement;

            if (element != null)
                bgColor = element.BackgroundColor == Color.Default ? bgColor : element.BackgroundColor.ToGtkColor();

            UpdateBackgroundChild(cell, bgColor);

            tableViewCell.ModifyBg(Gtk.StateType.Normal, bgColor);
        }

        protected void WireUpForceUpdateSizeRequested(ICellController cell, Gtk.Container nativeCell, Controls.ListView listView)
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

        internal static Gtk.Container GetRealCell(BindableObject cell)
        {
            return (Gtk.Container)cell.GetValue(RealCellProperty);
        }

        internal static void SetRealCell(BindableObject cell, Gtk.Container renderer)
        {
            cell.SetValue(RealCellProperty, renderer);
        }
    }
}