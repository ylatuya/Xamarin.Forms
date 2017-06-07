using Gdk;
using Gtk;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class TabbedPageHeader : EventBox
    {
        private Gtk.Label _label;
        private Gtk.Image _image;

        public TabbedPageHeader(string title, Pixbuf icon = null)
        {
            HBox hbox = new HBox(false, 0);
            hbox.Spacing = 0;

            _image = new Gtk.Image();
            _image.Pixbuf = icon;
            hbox.Add(_image);

            _label = new Gtk.Label();
            _label.Text = title;
            hbox.Add(_label);

            Add(hbox);

            if ((Child != null))
            {
                Child.ShowAll();
            }

            Show();
        }

        public Gtk.Label Label => _label;

        public Gtk.Image Icon => _image;
    }
}