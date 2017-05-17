namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class CustomComboBox : Gtk.EventBox
    {
        private Gtk.HBox _box;
        private Gtk.Entry _entry;
        private Gtk.Button _button;
        private Gtk.Arrow _arrow;
        private Gdk.Color _color;

        public CustomComboBox()
        {
            BuildCustomComboBox();
        }

        public Gtk.Entry Entry
        {
            get
            {
                return _entry;
            }
        }

        public Gtk.Button PopupButton
        {
            get
            {
                return _button;
            }
        }

        public Gdk.Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                Entry.ModifyText(Gtk.StateType.Normal, _color);
            }
        }

        private void BuildCustomComboBox()
        {
            _box = new Gtk.HBox();

            _entry = new Gtk.Entry();
            _entry.CanFocus = true;
            _entry.IsEditable = true;
            _box.Add(_entry);

            Gtk.Box.BoxChild entryBoxChild = ((Gtk.Box.BoxChild)(_box[_entry]));
            entryBoxChild.Position = 0;

            _button = new Gtk.Button();
            _button.WidthRequest = 30;
            _button.CanFocus = true;
            _arrow = new Gtk.Arrow(Gtk.ArrowType.Down, Gtk.ShadowType.EtchedOut);
            _button.Add(_arrow);
            _box.Add(_button);

            Gtk.Box.BoxChild buttonBoxChild = ((Gtk.Box.BoxChild)(_box[_button]));
            buttonBoxChild.Position = 1;
            buttonBoxChild.Expand = false;

            Add(_box);

            if ((Child != null))
            {
                Child.ShowAll();
            }

            Show();
        }
    }
}
