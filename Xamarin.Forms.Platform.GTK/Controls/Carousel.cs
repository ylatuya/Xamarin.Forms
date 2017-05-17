using Gtk;
using System.Collections;
using Container = Gtk.EventBox;
using System.Collections.Generic;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class Carousel : Container
    {
        private IList _itemsSource;
        private int _selectedIndex;
        private Table _root;
        private List<Container> _pages;
        private double _initialPos;
        private bool _animated;

        public Carousel()
        {
            BuildCarousel();
        }

        public IList ItemsSource
        {
            get
            {
                return _itemsSource;
            }
            set
            {
                if (_itemsSource != value)
                {
                    _itemsSource = value;
                    RefreshItemsSource(_itemsSource);
                }
            }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            private set { _selectedIndex = value; }
        }

        public bool Animated
        {
            get { return _animated; }
            set { _animated = value; }
        }

        public void SetBackground(Gdk.Color backgroundColor)
        {
            if (_root != null)
            {
                _root.ModifyBg(StateType.Normal, backgroundColor);
            }
        }

        private void BuildCarousel()
        {
            _pages = new List<Container>();

            _root = new Table(1, 1, true);
            Add(_root);

            ButtonPressEvent += OnCarouselButtonPressEvent;
            ButtonReleaseEvent += OnCarouselButtonReleaseEvent;
        }

        private void OnCarouselButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            _initialPos = args.Event.X;
        }

        private void OnCarouselButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
        {
            var lastPos = args.Event.X;

            if(lastPos == _initialPos)
            {
                return;
            }

            if (lastPos > _initialPos)
            {
                MoveLeft(Animated);
            }
            else
            {
                MoveRight(Animated);
            }
        }

        private void RefreshItemsSource(IList items)
        {
            if (items.Count == 0)
            {
                return;
            }

            _pages.Clear();

            for(int i = 0; i < items.Count; i++)
            {
                var pageContainer = items[i] as PageContainer;

                if (pageContainer != null)
                {
                    var page = pageContainer.Page;
                    var gtkPage = Platform.CreateRenderer(page);

                    _pages.Add(gtkPage.Container);
                    _root.Attach(gtkPage.Container, 0, 1, 0, 1);
                }
            }

            SelectedIndex = 0;
        }

        private void MoveLeft(bool animate = false)
        {
            if (SelectedIndex <= 0)
            {
                return;
            }

            SelectedIndex--;

            foreach(var page in _pages)
            {
                page.Visible = false;
            }

            _pages[SelectedIndex].Visible = true;

        }

        private void MoveRight(bool animate = false)
        {
            if (SelectedIndex >= (ItemsSource.Count - 1))
            {
                return;
            }
            
            SelectedIndex++;

            foreach (var page in _pages)
            {
                page.Visible = false;
            }

            _pages[SelectedIndex].Visible = true;
        }
    }
}