using Gtk;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class ImageCellRenderer : CellRenderer
    {
        public override Gtk.CellRenderer GetCell(Cell item, Gtk.CellRenderer reusableView, TreeView tv)
        {
            var imageCell = (ImageCell)item;

            var tvc = reusableView as GtkImageCell ?? new GtkImageCell();

            tvc.Text = imageCell.Text ?? string.Empty;
            tvc.TextColor = imageCell.TextColor.ToGtkColor();
            tvc.Detail = imageCell.Detail ?? string.Empty;
            tvc.DetailColor = imageCell.DetailColor.ToGtkColor();

            WireUpForceUpdateSizeRequested(item, tvc, tv);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            SetImage(imageCell, tvc);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return tvc;
        }

        internal override void UpdateBackgroundChild(Cell cell, Gdk.Color backgroundColor)
        {
            base.UpdateBackgroundChild(cell, backgroundColor);
        }

        private static async System.Threading.Tasks.Task SetImage(ImageCell cell, GtkImageCell target)
        {
            var source = cell.ImageSource;

            target.Image = null;

            Renderers.IImageSourceHandler handler;

            if (source != null && (handler =
                Internals.Registrar.Registered.GetHandler<Renderers.IImageSourceHandler>(source.GetType())) != null)
            {
                Gdk.Pixbuf image;
                try
                {
                    image = await handler.LoadImageAsync(source).ConfigureAwait(false);
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                {
                    image = null;
                }

                target.Image = image;

            }
            else
                target.Image = null;
        }
    }
}