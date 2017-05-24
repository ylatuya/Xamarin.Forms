using System;
using Container = Gtk.EventBox;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using Xamarin.Forms.Platform.GTK.Controls;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class CarouselPageRenderer : Container, IVisualElementRenderer
    {
        private Carousel _carousel;
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
            VisualElement oldElement = Element;
            Element = element;

            Init();

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
            UpdateSource();
        }

        private void UpdateCurrentPage()
        {
            ContentPage current = Carousel.CurrentPage;

            if (current != null)
            {
                int index = Carousel.CurrentPage != null ? CarouselPage.GetIndex(Carousel.CurrentPage) : 0;

                if (index < 0)
                    index = 0;

                if (SelectedIndex == index)
                    return;

                SelectedIndex = index;
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
            var pages = new List<PageContainer>();

            for (var i = 0; i < Element.LogicalChildren.Count; i++)
            {
                Element element = Element.LogicalChildren[i];
                var child = element as ContentPage;

                if (child != null)
                {
                    pages.Add(new PageContainer(child, i));
                }
            }

            if (_carousel != null)
            {
                _carousel.ItemsSource = pages;
            }

            UpdateCurrentPage();
        }

        private void OnElementChanged(VisualElementChangedEventArgs e)
        {
            ElementChanged?.Invoke(this, e);
        }
    }
}