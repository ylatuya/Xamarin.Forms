using Gtk;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class TextCell : CellBase
    {
        private VBox _root;
        private Gtk.Label _textLabel;
        private Gtk.Label _detailLabel;
        private string _text;
        private string _detail;
        private Gdk.Color _textColor;
        private Gdk.Color _detailColor;

        public TextCell(
            string text,
            Gdk.Color textColor,
            string detail,
            Gdk.Color detailColor)
        {
            _root = new VBox();

            _textLabel = new Gtk.Label();
            _textLabel.SetAlignment(0, 0);
            _textLabel.ModifyFg(StateType.Normal, textColor);
            _textLabel.Text = text;

            _root.PackStart(_textLabel, false, false, 0);

            _detailLabel = new Gtk.Label();
            _detailLabel.SetAlignment(0, 0);
            _detailLabel.ModifyFg(StateType.Normal, detailColor);
            _detailLabel.Text = detail;

            _root.PackStart(_detailLabel, true, true, 0);

            Add(_root);
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; UpdateText(_text); }
        }

        public string Detail
        {
            get { return _detail; }
            set { _detail = value; UpdateDetail(_detail); }
        }

        public Gdk.Color TextColor
        {
            get { return _textColor; }
            set { _textColor = value; UpdateTextColor(_textColor); }
        }

        public Gdk.Color DetailColor
        {
            get { return _detailColor; }
            set { _detailColor = value; UpdateDetailColor(_detailColor); }
        }

        private void UpdateText(string text)
        {
            if(_textLabel != null)
            {
                _textLabel.Text = text;
            }
        }

        private void UpdateDetail(string detail)
        {
            if (_detailLabel != null)
            {
                _detailLabel.Text = detail;
            }
        }

        private void UpdateTextColor(Gdk.Color textColor)
        {
            if (_textLabel != null)
            {
                _textLabel.ModifyFg(StateType.Normal, textColor);
            }
        }

        private void UpdateDetailColor(Gdk.Color detailColor)
        {
            if (_detailLabel != null)
            {
                _detailLabel.ModifyFg(StateType.Normal, detailColor);
            }
        }
    }
}