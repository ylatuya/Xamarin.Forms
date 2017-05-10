using Gtk;
using System.Linq;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class Page : EventBox
    {
        private EventBox _headerContainer;
        private EventBox _contentContainer;
        private HBox _toolbar;
        private EventBox _content;

        public HBox Toolbar
        {
            get
            {
                return _toolbar;
            }
            set
            {
                if (_toolbar != value)
                {
                    _toolbar = value;
                    RefreshToolbar();
                }
            }
        }

        public EventBox Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    RefreshContent();
                }
            }
        }


        public Page()
        {
            BuildPage();
        }

        public void SetBackgroundColor(Gdk.Color backgroundColor)
        {
            if (_contentContainer != null)
            {
                _contentContainer.ModifyBg(StateType.Normal, backgroundColor);
            }
        }

        private void BuildPage()
        {
            _toolbar = new HBox();
            _content = new EventBox();

            var root = new VBox(false, 0);

            _headerContainer = new EventBox();
            root.PackStart(_headerContainer, true, true, 0);

            _contentContainer = new EventBox();
            root.PackStart(_contentContainer, true, true, 0);

            Add(root);

            ShowAll();
        }

        private void RefreshToolbar()
        {
            if (_headerContainer.Children.Length > 0)
            {
                _headerContainer.Remove(_headerContainer.Children.First());
            }

            _headerContainer.Add(_toolbar);
            _toolbar.ShowAll();
        }

        private void RefreshContent()
        {
            if (_contentContainer.Children.Length > 0)
            {
                _contentContainer.Remove(_contentContainer.Children.First());
            }

            _contentContainer.Add(_content);
            _content.ShowAll(); 
        }
    }
}