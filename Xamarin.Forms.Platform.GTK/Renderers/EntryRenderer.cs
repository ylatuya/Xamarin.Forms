using System;
using System.ComponentModel;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class EntryRenderer : ViewRenderer<Entry, Gtk.Entry>
    {
        private bool _disposed;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                Gtk.Entry entry = new Gtk.Entry();
                SetNativeControl(entry);
            }

            if (e.NewElement != null)
            {
                UpdateText();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Entry.TextProperty.PropertyName)
                UpdateText();

            base.OnElementPropertyChanged(sender, e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }

            base.Dispose(disposing);
        }

        private void UpdateText()
        {
            if (Control.Text != Element.Text)
                Control.Text = Element.Text ?? string.Empty;
        }
    }
}