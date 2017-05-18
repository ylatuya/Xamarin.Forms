using Gdk;
using System;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class ImageControl : Gtk.HBox
    {
        public const int MinWidth = 1;
        public const int MinHeight = 1;

        private Gtk.Image _image;
        private Pixbuf _original;
        private Gdk.Size _imageSize;
        private ImageAspect _aspect;

        private Gdk.Rectangle _lastAllocation = Gdk.Rectangle.Zero;

        public ImageControl()
        {
            _aspect = ImageAspect.AspectFill;
            BuildImageControl();
        }

        public ImageAspect Aspect
        {
            get
            {
                return _aspect;
            }

            set
            {
                _aspect = value;
                QueueDraw();
            }
        }

        public Pixbuf Pixbuf
        {
            get
            {
                return _image.Pixbuf;
            }
            set
            {
                _original = value;
                _image.Pixbuf = value;
                _imageSize = _image.Allocation.Size;
            }
        }

        public void SetScale(int width, int height)
        {
            if (_image == null || _original == null)
                return;

            if (width <= 0 || height <= 0)
                return;

            if (width < MinWidth)
                width = MinWidth;

            if (height < MinHeight)
                height = MinHeight;

            if (width == _original.Width && height == _original.Height)
                _image.Pixbuf = _original;
            else
                _image.Pixbuf = _original.ScaleSimple(width, height, InterpType.Bilinear);

            _image.SetSizeRequest(width, height);
            _imageSize.Width = width;
            _imageSize.Height = height;

            _image.ShowAll();
        }

        public void SetAlpha(double opacity)
        {
            if (_image != null && _original != null)
            {
                _image.Pixbuf = Pixbuf.AddAlpha(
                    true,
                    ((byte)(255 * opacity)),
                    ((byte)(255 * opacity)), 
                    ((byte)(255 * opacity)));
            }
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);

            if (_image.Pixbuf != null && !_lastAllocation.Equals(allocation))
            {
                _lastAllocation = allocation;

                var srcWidth = _original.Width;
                var srcHeight = _original.Height;

                Pixbuf newPixBuf = null;

                switch (Aspect)
                {
                    case ImageAspect.AspectFit:
                        newPixBuf = GetAspectFitPixBuf(_original, allocation);
                        break;
                    case ImageAspect.AspectFill:
                        newPixBuf = GetAspectFillPixBuf(_original, allocation);
                        break;
                    case ImageAspect.Fill:
                        newPixBuf = GetFillPixBuf(_original, allocation);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(Aspect));
                }

                _image.Pixbuf = newPixBuf;
            }
        }

        private void BuildImageControl()
        {
            CanFocus = true;

            _image = new Gtk.Image();

            PackStart(_image, true, true, 0);
        }

        private static Pixbuf GetAspectFitPixBuf(Pixbuf original, Gdk.Rectangle allocation)
        {
            var widthRatio = (float)allocation.Width / original.Width;
            var heigthRatio = (float)allocation.Height / original.Height;

            var fitRatio = Math.Min(widthRatio, heigthRatio);
            var finalWidth = (int)(original.Width * fitRatio);
            var finalHeight = (int)(original.Height * fitRatio);

            return original.ScaleSimple(finalWidth, finalHeight, InterpType.Bilinear);
        }

        private static Pixbuf GetAspectFillPixBuf(Pixbuf original, Gdk.Rectangle allocation)
        {
            var widthRatio = (float)allocation.Width / original.Width;
            var heigthRatio = (float)allocation.Height / original.Height;

            var fitRatio = Math.Max(widthRatio, heigthRatio);
            var finalWidth = (int)(original.Width * fitRatio);
            var finalHeight = (int)(original.Height * fitRatio);

            return original.ScaleSimple(finalWidth, finalHeight, InterpType.Bilinear);
        }

        private static Pixbuf GetFillPixBuf(Pixbuf original, Gdk.Rectangle allocation)
        {
            return original.ScaleSimple(allocation.Width, allocation.Height, InterpType.Bilinear);
        }
    }

    public enum ImageAspect
    {
        AspectFit,
        AspectFill,
        Fill
    }
}