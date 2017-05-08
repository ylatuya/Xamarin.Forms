using Gdk;
using Gtk;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class ImageCell : CellRenderer
    {
        private const int ImageCellSpacing = 6;

        [GLib.Property("image")]
        public Pixbuf Image { get; set; }

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

            width = 250;
        }

        protected override void Render(Drawable window, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, CellRendererState flags)
        {
            using (var ctx = CairoHelper.Create(window))
            {
                using (var layout = new Pango.Layout(widget.PangoContext))
                {
                    DrawImage(Image, window, widget, cell_area, flags);
                    DrawText(Text, window, widget, cell_area, flags);

                    if (!string.IsNullOrEmpty(Detail))
                    {
                        int height, width;
                        layout.GetPixelSize(out width, out height);
                        DrawDetail(Detail, window, widget, cell_area, flags, height + ImageCellSpacing);
                    }
                }
            }

            base.Render(window, widget, background_area, cell_area, expose_area, flags);
        }

        private void DrawImage(Pixbuf image, Drawable window, Widget widget, Gdk.Rectangle cell_area, CellRendererState flags)
        {
            using (var layout = new Pango.Layout(widget.PangoContext))
            {
                layout.Alignment = Pango.Alignment.Left;
   
                StateType state = flags.HasFlag(CellRendererState.Selected) ?
                    widget.IsFocus ? StateType.Selected : StateType.Active : StateType.Normal;

                window.DrawPixbuf(
                    widget.Style.ForegroundGC(state), 
                    image, 
                    0, 
                    0, 
                    cell_area.X, 
                    cell_area.Y,
                    cell_area.Height,
                    cell_area.Height, 
                    RgbDither.None,
                    0,
                    0);
            }
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
                    cell_area.X + ImageCellSpacing + ((Image != null) ? cell_area.Height : 0), 
                    cell_area.Y + ImageCellSpacing, 
                    layout);
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
                    cell_area.X + ImageCellSpacing +((Image != null) ? cell_area.Height : 0), 
                    cell_area.Y + ImageCellSpacing +topMargin, 
                    layout);
            }
        }
    }
}
