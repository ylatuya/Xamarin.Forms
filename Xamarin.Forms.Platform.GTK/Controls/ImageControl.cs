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
        private bool _resized;

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

        public ImageControl()
        {
            BuildImageControl();
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
            if ((_image.Pixbuf != null) && (!_resized))
            {
                var srcWidth = _original.Width;
                var srcHeight = _original.Height;
                int resultWidth, resultHeight;
                ScaleRatio(srcWidth, srcHeight, allocation.Width, allocation.Height, out resultWidth, out resultHeight);
                _image.Pixbuf = _original.ScaleSimple(resultWidth, resultHeight, InterpType.Bilinear);
                _resized = true;
            }
            else
            {
                _resized = false;
                base.OnSizeAllocated(allocation);
            }
        }

        private void BuildImageControl()
        {
            CanFocus = true;

            _image = new Gtk.Image();

            PackStart(_image, true, true, 0);
        }

        private static void ScaleRatio(int srcWidth, int srcHeight, int destWidth, int destHeight, out int resultWidth, out int resultHeight)
        {
            var widthRatio = (float)destWidth / srcWidth;
            var heigthRatio = (float)destHeight / srcHeight;

            var ratio = Math.Min(widthRatio, heigthRatio);
            resultHeight = (int)(srcHeight * ratio);
            resultWidth = (int)(srcWidth * ratio);
        }
    }
}