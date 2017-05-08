using Gdk;
using Gtk;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class TextCell : CellRenderer
    {
        private const int TextCellSpacing = 6;

        [GLib.Property("text")]
        public string Text { get; set; }

        [GLib.Property("textcolor")]
        public Gdk.Color TextColor { get; set; }

        [GLib.Property("detail")]
        public string Detail { get; set; }

        [GLib.Property("detailcolor")]
        public Gdk.Color DetailColor { get; set; }

        public override void GetSize(Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
        {
            base.GetSize(widget, ref cell_area, out x_offset, out y_offset, out width, out height);
        }

        protected override void Render(Drawable window, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, CellRendererState flags)
        {
            using (var ctx = CairoHelper.Create(window))
            {
                using (var layout = new Pango.Layout(widget.PangoContext))
                {
                    DrawText(Text, window, widget, cell_area, flags);

                    if (!string.IsNullOrEmpty(Detail))
                    {
                        int height, width;
                        layout.GetPixelSize(out width, out height);
                        DrawDetail(Detail, window, widget, cell_area, flags, height + TextCellSpacing);
                    }
                }
            }

            base.Render(window, widget, background_area, cell_area, expose_area, flags);
        }

        private void DrawText(string text, Drawable window, Widget widget, Gdk.Rectangle cell_area, CellRendererState flags)
        {
            using (var layout = new Pango.Layout(widget.PangoContext))
            {
                layout.Alignment = Pango.Alignment.Left;
                layout.SetText(text);

                Pango.FontDescription desc = Pango.FontDescription.FromString("12");
                layout.FontDescription = desc;

                StateType state = flags.HasFlag(CellRendererState.Selected) ?
                    widget.IsFocus ? StateType.Selected : StateType.Active : StateType.Normal;

                window.DrawLayout(
                    widget.Style.TextGC(state),
                    cell_area.X + TextCellSpacing, 
                    cell_area.Y + TextCellSpacing, layout);
            }
        }

        private void DrawDetail(string text, Drawable window, Widget widget, Gdk.Rectangle cell_area, CellRendererState flags, int topMargin)
        {
            using (var layout = new Pango.Layout(widget.PangoContext))
            {
                layout.Alignment = Pango.Alignment.Left;
                layout.SetText(text);

                StateType state = flags.HasFlag(CellRendererState.Selected) ?
                    widget.IsFocus ? StateType.Selected : StateType.Active : StateType.Normal;

                window.DrawLayout(
                    widget.Style.TextGC(state),
                    cell_area.X + TextCellSpacing, 
                    cell_area.Y + TextCellSpacing + topMargin, layout);
            }
        }
    }
}
