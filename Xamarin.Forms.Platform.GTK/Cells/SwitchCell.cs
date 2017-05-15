using Gtk;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class SwitchCell : CellBase
    {
        private string _text;
        private bool _on;
        private Gtk.Label _textLabel;
        private CheckButton _checkButton;

        public SwitchCell(
            string text,
            bool on)
        {
            _textLabel = new Gtk.Label();
            _textLabel.SetAlignment(0, 0);
            _textLabel.Text = text;

            Add(_textLabel);

            _checkButton = new CheckButton();
            _checkButton.SetAlignment(0, 0);
            _checkButton.Active = on;

            Add(_checkButton);
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; UpdateText(_text); }
        }

        public bool On
        {
            get { return _on; }
            set { _on = value; UpdateOn(_on); }
        }

        private void UpdateText(string text)
        {
            if (_textLabel != null)
            {
                _textLabel.Text = text;
            }
        }

        private void UpdateOn(bool on)
        {
            if (_checkButton != null)
            {
                _checkButton.Active = on;
            }
        }
    }
}