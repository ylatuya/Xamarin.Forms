using Gtk;
using System.Collections;
using Container = Gtk.EventBox;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class CarouselPage
    {
        public Container GtkPage { get; set; }
        public Xamarin.Forms.Page Page { get; set; }

        public CarouselPage(Container gtkPage, Xamarin.Forms.Page page)
        {
            GtkPage = gtkPage;
            Page = page;
        }
    }

    public class Carousel : Container
    {
        private IList _itemsSource;
        private int _selectedIndex;
        private Table _root;
        private List<CarouselPage> _pages;
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
        
        public void SetCurrentPage(int selectedIndex)
        {
            if(!_pages.Any())
            {
                return;
            }

            SelectedIndex = selectedIndex;

            foreach (var page in _pages)
            {
                page.GtkPage.Visible = false;
            }

            _pages[selectedIndex].GtkPage.Visible = true;
        }

        public void AddPage(int index, object element)
        {
            var page = element as Xamarin.Forms.Page;

            if (page != null)
            {
                var gtkPage = Platform.CreateRenderer(page);
                _pages.Insert(index, new CarouselPage(gtkPage.Container, page));
                _root.Attach(gtkPage.Container, 0, 1, 0, 1);
            }

            ItemsSource = _pages;
        }

        public void RemovePage(object element)
        {
            var page = element as Xamarin.Forms.Page;

            if (page != null)
            {
                var gtkPage = _pages.FirstOrDefault(p => p.Page == page);

                if (gtkPage != null)
                {
                    _pages.Remove(gtkPage);
                    _root.Remove(gtkPage.GtkPage);
                }
            }

            ItemsSource = _pages;
        }

        public void Reset()
        {
            _pages.Clear();

            do
            {
                foreach (var children in _root.Children)
                {
                    _root.RemoveFromContainer(children);
                }
            } while (_root.Children.Length > 0);
        }

        private void BuildCarousel()
        {
            _pages = new List<CarouselPage>();

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

            for (int i = 0; i < items.Count; i++)
            {
                var pageContainer = items[i] as PageContainer;

                if (pageContainer != null)
                {
                    var page = pageContainer.Page;
                    var gtkPage = Platform.CreateRenderer(page);

                    _pages.Add(new CarouselPage(gtkPage.Container, page));
                    _root.Attach(gtkPage.Container, 0, 1, 0, 1);
                }
            }

            SelectedIndex = 0;
            SetCurrentPage(SelectedIndex);
        }

        private void MoveLeft(bool animate = false)
        {
            if (SelectedIndex <= 0)
            {
                return;
            }

            SelectedIndex--;

            SetCurrentPage(SelectedIndex);
        }

        private void MoveRight(bool animate = false)
        {
            if (SelectedIndex >= (ItemsSource.Count - 1))
            {
                return;
            }
            
            SelectedIndex++;

            SetCurrentPage(SelectedIndex);
        }
    }
}