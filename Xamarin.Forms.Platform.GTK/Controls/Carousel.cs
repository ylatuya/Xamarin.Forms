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
        private Container _root;
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

            _root = new Container();
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

            int counter = 0;
            foreach (var item in items)
            {
                var pageContainer = item as PageContainer;

                if (pageContainer != null)
                {
                    var page = pageContainer.Page;
                    var gtkPage = Platform.CreateRenderer(page);

                    _pages.Add(gtkPage.Container);

                    if (counter == 0)
                    {
                        _root.Add(gtkPage.Container);
                    }
                }
            }

            SelectedIndex = 0;
        }

        private void LayoutChildren(Widget children)
        {
            if (_root == null) return;

            if (children == null) return;

            children.WidthRequest = _root.WidthRequest;
        }

        private void MoveLeft(bool animate = false)
        {
            _root.Remove(_pages[SelectedIndex]);

            if (SelectedIndex <= 0)
                SelectedIndex = _pages.Count - 1;
            else
                SelectedIndex--;

            _root.Add(_pages[SelectedIndex]);

            ShowAll();

            if (animate)
            {
                // TODO:
            }
        }

        private void MoveRight(bool animate = false)
        {
            _root.Remove(_pages[SelectedIndex]);

            if (SelectedIndex >= (ItemsSource.Count - 1))
                SelectedIndex = 0;
            else
                SelectedIndex++;

            _root.Add(_pages[SelectedIndex]);

            ShowAll();

            if (animate)
            {
                // TODO:
            }
        }
    }
}