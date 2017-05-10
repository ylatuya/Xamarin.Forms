using Gdk;
using System;
using System.IO;

namespace Xamarin.Forms.Platform.GTK.Extensions
{
    public static class ImageExtensions
    {
        public static Pixbuf ToPixbuf(this ImageSource imagesource)
        {
            try
            {
                Pixbuf image = null;

                var filesource = imagesource as FileImageSource;

                if (filesource != null)
                {
                    var file = filesource.File;

                    if (!string.IsNullOrEmpty(file))
                    {
                        var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
                        image = new Pixbuf(imagePath);
                    }
                }

                return image;
            }
            catch
            {
                return null;
            }
        }
    }
}