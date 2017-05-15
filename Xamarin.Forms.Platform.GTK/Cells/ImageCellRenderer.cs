using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class ImageCellRenderer : CellRenderer
    {
        public override Gtk.Container GetCell(Cell item, Gtk.Container reusableView, Controls.ListView listView)
        {
            var imageCell = (Xamarin.Forms.ImageCell)item;

            var text = imageCell.Text ?? string.Empty;
            var textColor = imageCell.TextColor.ToGtkColor();
            var detail = imageCell.Detail ?? string.Empty;
            var detailColor = imageCell.DetailColor.ToGtkColor();

            var gtkImageCell = 
                reusableView as ImageCell ?? 
                new ImageCell(
                    null, 
                    text,
                    textColor,
                    detail, 
                    detailColor);

            if (gtkImageCell.Cell != null)
                gtkImageCell.Cell.PropertyChanged -= gtkImageCell.HandlePropertyChanged;

            gtkImageCell.Cell = imageCell;

            imageCell.PropertyChanged += gtkImageCell.HandlePropertyChanged;
            gtkImageCell.PropertyChanged = HandlePropertyChanged;

            WireUpForceUpdateSizeRequested(item, gtkImageCell, listView);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            SetImage(imageCell, gtkImageCell);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return gtkImageCell;
        }

        private async void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var gtkImageCell = (ImageCell)sender;
            var imageCell = (Xamarin.Forms.ImageCell)gtkImageCell.Cell;

            if (args.PropertyName == Xamarin.Forms.TextCell.TextProperty.PropertyName)
            {
                gtkImageCell.Text = imageCell.Text ?? string.Empty;
            }
            else if (args.PropertyName == Xamarin.Forms.TextCell.DetailProperty.PropertyName)
            {
                gtkImageCell.Detail = imageCell.Detail ?? string.Empty;
            }
            else if (args.PropertyName == Xamarin.Forms.ImageCell.ImageSourceProperty.PropertyName)
            {
                await SetImage(imageCell, gtkImageCell);
            }
        }

        internal override void UpdateBackgroundChild(Cell cell, Gdk.Color backgroundColor)
        {
            base.UpdateBackgroundChild(cell, backgroundColor);
        }

        private static async System.Threading.Tasks.Task SetImage(Xamarin.Forms.ImageCell cell, ImageCell target)
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