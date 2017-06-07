using System;

namespace Xamarin.Forms.Platform.GTK.Extensions
{
    public static class PageExtensions
    {
        public static Gtk.EventBox CreateContainer(this VisualElement view)
        {
            if (!Forms.IsInitialized)
                throw new InvalidOperationException("call Forms.Init() before this");

            throw new NotImplementedException();
        }
    }
}