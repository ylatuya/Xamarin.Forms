using Gtk;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class ScrolledTextView : ScrolledWindow
    {
        private TextView _textView;
        private bool _editable;

        public ScrolledTextView()
        {
            ShadowType = ShadowType.In;
            HscrollbarPolicy = PolicyType.Never;
            VscrollbarPolicy = PolicyType.Always;

            _textView = new TextView
            {
                AcceptsTab = true,
                WrapMode = Gtk.WrapMode.WordChar
            };

            Add(_textView);
        }

        public bool Editable
        {
            get
            {
                return _editable;
            }

            set
            {
                _editable = value;
                UpdateEditable();
            }
        }

        public TextView TextView => _textView;

        private void UpdateEditable()
        {
            TextView.Editable = _editable;
            TextView.Sensitive = _editable;
            TextView.CanFocus = _editable;
        }
    }
}
