using System;
using Container = Gtk.EventBox;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using Xamarin.Forms.Platform.GTK.Controls;
using Xamarin.Forms.Platform.GTK.Extensions;
using Xamarin.Forms.Internals;
using System.Linq;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class CarouselPageRenderer : Container, IVisualElementRenderer
    {
        private Carousel _carousel;
        private List<PageContainer> _pages;
        private int _selectedIndex;
        private bool _appeared;
        private bool _disposed;

        public Container Container => this;

        public VisualElement Element { get; private set; }

        public Page Page => (Page)Element;

        CarouselPage Carousel => Element as CarouselPage;

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex == value)
                    return;

                _selectedIndex = value;

                if (Carousel != null)
                    Carousel.CurrentPage = (ContentPage)Element.LogicalChildren[(int)SelectedIndex];
            }
        }

        public bool Disposed { get { return _disposed; } }

        protected override void OnShown()
        {
            base.OnShown();

            if (_appeared || _disposed)
                return;

            _appeared = true;
            Page.SendAppearing();
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();

            if (!_appeared || _disposed)
                return;

            _pages = null;
            _appeared = false;
            Page.SendDisappearing();
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                if (Carousel != null)
                {
                    Carousel.PropertyChanged -= OnPropertyChanged;
                    Carousel.PagesChanged -= OnPagesChanged;
                }

                Platform.SetRenderer(Element, null);

                Element = null;
                _disposed = true;
                Page.SendDisappearing();
            }

            base.Dispose();
        }

        public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            var result = new Size(
              widthConstraint,
              heightConstraint);

            return new SizeRequest(result);
        }

        public void SetElement(VisualElement element)
        {
            var newPage = element as CarouselPage;
            if (element != null && newPage == null)
                throw new ArgumentException("element must be a CarouselPage");

            VisualElement oldElement = Element;
            Element = element;

            Init();

            if (newPage != null)
            {
                UpdateCurrentPage();
                newPage.SendAppearing();
            }

            OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));
        }

        public void SetElementSize(Size size)
        {
            Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
        }

        private void Init()
        {
            InitializeCarousel();
            UpdateBackground();
            UpdateSource();

            Carousel.PropertyChanged += OnPropertyChanged;
            Carousel.PagesChanged += OnPagesChanged;
        }

        private void InitializeCarousel()
        {
            _carousel = new Carousel();
            _carousel.Animated = true;

            Add(_carousel);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TabbedPage.CurrentPage))
                UpdateCurrentPage();
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                UpdateBackground();
            else if (e.PropertyName == Page.BackgroundImageProperty.PropertyName)
                UpdateBackground();
        }

        private void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    int index = e.NewStartingIndex;
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        var page = e.NewItems[i] as Page;
                        _carousel.AddPage(index, page);
                        _pages.Add(new PageContainer(page, i));
                        index++;
                    }

                    var newPages = new List<object>();
                    foreach(var pc in _pages)
                    {
                        newPages.Add(pc.Page);
                    }

                    e.Apply(Carousel.Children, newPages);

                    break;
                case NotifyCollectionChangedAction.Remove:

                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        var page = e.OldItems[i];
                        _carousel.RemovePage(page);
                        var pageContainer = _pages.FirstOrDefault(p => p.Page == page);
                        _pages.Remove(pageContainer);
                    }

                    var oldPages = new List<object>();
                    foreach (var pc in _pages)
                    {
                        oldPages.Add(pc.Page);
                    }

                    e.Apply(Carousel.Children, oldPages);
                    UpdateCurrentPage();
                    break;
                case NotifyCollectionChangedAction.Reset:

                    if (_carousel != null)
                    {
                        _carousel.Reset();
                        UpdateSource();
                    }

                    break;
            }
        }

        private void UpdateCurrentPage()
        {
            ContentPage current = Carousel.CurrentPage;

            if (current != null)
            {
                int index = Carousel.CurrentPage != null ? CarouselPage.GetIndex(Carousel.CurrentPage) : 0;

                if (index < 0)
                    index = 0;

                SelectedIndex = index;

                if (_carousel != null)
                {
                    _carousel.SetCurrentPage(SelectedIndex);
                }
            }
        }

        private void UpdateBackground()
        {
            if(Element.BackgroundColor.IsDefault)
            {
                return;
            }

            var backgroundColor = Element.BackgroundColor.ToGtkColor();

            if(_carousel != null)
            {
                _carousel.SetBackground(backgroundColor);
            }
        }

        private void UpdateSource()
        {
            _pages = new List<PageContainer>();

            for (var i = 0; i < Element.LogicalChildren.Count; i++)
            {
                Element element = Element.LogicalChildren[i];
                var child = element as ContentPage;

                if (child != null)
                {
                    _pages.Add(new PageContainer(child, i));
                }
            }

            if (_carousel != null)
            {
                _carousel.ItemsSource = _pages;
            }

            UpdateCurrentPage();
        }

        private void OnElementChanged(VisualElementChangedEventArgs e)
        {
            ElementChanged?.Invoke(this, e);
        }
    }
}