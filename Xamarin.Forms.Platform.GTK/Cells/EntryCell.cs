using Xamarin.Forms.Platform.GTK.Controls;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class EntryCell : CellBase
    {
        private string _text;
        private string _placeholder;
        private Gtk.Label _textLabel;
        private EntryWrapper _entry;

        public EntryCell(
            string text,
            string placeholder)
        {
            _textLabel = new Gtk.Label();
            _textLabel.SetAlignment(0, 0);
            _textLabel.Text = text;

            Add(_textLabel);

            _entry = new EntryWrapper();
            _entry.PlaceholderText = placeholder;

            Add(_entry);
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; UpdateText(_text); }
        }

        public string Placeholder
        {
            get { return _placeholder; }
            set { _placeholder = value; UpdatePlaceholder(_placeholder); }
        }

        private void UpdateText(string text)
        {
            if (_textLabel != null)
            {
                _textLabel.Text = text;
            }
        }

        private void UpdatePlaceholder(string placeholder)
        {
            if (_entry != null)
            {
                _entry.PlaceholderText = placeholder;
            }
        }
    }
}