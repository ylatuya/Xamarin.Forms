using Gtk;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class SwitchCell : EventBox
    {
        public SwitchCell(
            string text,
            bool on)
        {
            Gtk.Label textLabel = new Gtk.Label();
            textLabel.SetAlignment(0, 0);
            textLabel.Text = text;

            Add(textLabel);
        }
    }
}