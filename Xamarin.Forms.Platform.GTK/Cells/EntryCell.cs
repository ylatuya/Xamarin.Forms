using Gtk;
using Xamarin.Forms.Platform.GTK.Controls;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class EntryCell : CellBase
    {
        private string _label;
        private Gdk.Color _textColor;
        private string _text;
        private string _placeholder;
        private VBox _root;
        private Gtk.Label _textLabel;
        private EntryWrapper _entry;

        public EntryCell(
            string label,
            Gdk.Color labelColor,
            string text,
            string placeholder)
        {
            _root = new VBox();
            Add(_root);

            _textLabel = new Gtk.Label();
            _textLabel.SetAlignment(0, 0);
            _textLabel.Text = label;
            _textLabel.ModifyFg(StateType.Normal, labelColor);

            _root.PackStart(_textLabel, false, false, 0);

            _entry = new EntryWrapper();
            _entry.IsEnabled = true;
            _entry.Entry.Text = text;
            _entry.PlaceholderText = placeholder;

            _root.PackStart(_entry, false, false, 0);
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; UpdateLabel(_label); }
        }

        public Gdk.Color LabelColor
        {
            get { return _textColor; }
            set { _textColor = value; UpdateLabelColor(_textColor); }
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

        private void UpdateLabel(string label)
        {
            if (_textLabel != null)
            {
                _textLabel.Text = label;
            }
        }

        private void UpdateLabelColor(Gdk.Color textColor)
        {
            if (_textLabel != null)
            {
                _textLabel.ModifyFg(StateType.Normal, textColor);
            }
        }

        private void UpdateText(string text)
        {
            if (_entry != null)
            {
                _entry.Entry.Text = text;
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