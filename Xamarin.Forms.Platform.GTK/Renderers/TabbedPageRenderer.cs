using Gtk;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.GTK.Controls;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class TabbedPageRenderer : EventBox, IVisualElementRenderer
    {
        private bool _disposed;

        public Notebook Control { get; private set; }

        public TabbedPage Element { get; private set; }

        IElementController ElementController => Element as IElementController;

        IPageController PageController => Element as IPageController;

        VisualElement IVisualElementRenderer.Element => Element;

        public EventBox Container => this;

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                PageController.SendDisappearing();
                Element.PagesChanged -= OnPagesChanged;
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
            if (element != null && !(element is TabbedPage))
                throw new ArgumentException("Element must be a TabbedPage", "element");

            TabbedPage oldElement = Element;

            Element = (TabbedPage)element;

            if (oldElement != null)
            {
                oldElement.PropertyChanged -= OnElementPropertyChanged;
                ((INotifyCollectionChanged)oldElement.Children).CollectionChanged -= OnPagesChanged;
            }

            if (element != null)
            {
                if (Control == null)
                {
                    Control = new Notebook
                    {
                        CanFocus = true,
                        Scrollable = true,
                        ShowTabs = true,
                        TabPos = PositionType.Top
                    };

                    Add(Control);
                }
            }

            OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

            OnPagesChanged(Element.Children,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            UpdateCurrentPage();
            UpdateBarBackgroundColor();
            UpdateBarTextColor();
        }

        public void SetElementSize(Size size)
        {
            Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
        }

        protected override void OnShown()
        {
            PageController.SendAppearing();

            base.OnShown();
        }

        protected override void OnDestroyed()
        {
            PageController.SendDisappearing();

            base.OnDestroyed();
        }

        protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
        {
            ElementChanged?.Invoke(this, e);
        }

        private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TabbedPage.CurrentPage))
            {
                UpdateCurrentPage();
            }
            else if (e.PropertyName == TabbedPage.BarBackgroundColorProperty.PropertyName)
                UpdateBarBackgroundColor();
        }

        private void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            e.Apply((o, i, c) => SetupPage((Page)o, i), (o, i) => TeardownPage((Page)o), Reset);
            SetPages();
            UpdateChildrenOrderIndex();
        }

        private void SetupPage(Page page, int index)
        {
            var renderer = Platform.GetRenderer(page);

            if (renderer == null)
            {
                renderer = Platform.CreateRenderer(page);
                Platform.SetRenderer(page, renderer);
            }

            page.PropertyChanged += OnPagePropertyChanged;
        }

        private void SetPages()
        {
            for (var i = 0; i < Element.Children.Count; i++)
            {
                var child = Element.Children[i];
                var page = child as Page;

                if (page == null)
                    continue;

                var pageRenderer = Platform.GetRenderer(page);

                if (pageRenderer != null)
                {
                    Control.InsertPage(
                        pageRenderer.Container,
                        new TabbedPageHeader(page.Title, page.Icon?.ToPixbuf()),
                        i);
                }
            }

            Control.CurrentPage = 0;
        }

        private void TeardownPage(Page page)
        {
            page.PropertyChanged -= OnPagePropertyChanged;

            Platform.SetRenderer(page, null);
        }

        private void Reset()
        {
            var i = 0;
            foreach (var page in Element.Children)
                SetupPage(page, i++);
        }

        private void OnPagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Page.TitleProperty.PropertyName)
            {
                var page = (Page)sender;
                var index = TabbedPage.GetIndex(page);
                var title = page.Title;

                var pageRenderer = Platform.GetRenderer(page);
                if (pageRenderer != null)
                {
                    var tabbedPageHeader = Control.GetTabLabel(pageRenderer.Container) as TabbedPageHeader;
                    tabbedPageHeader.GetTabbedPageTitle().Text = title;
                }
            }
            else if (e.PropertyName == Page.IconProperty.PropertyName)
            {
                var page = (Page)sender;
                var index = TabbedPage.GetIndex(page);
                var icon = page.Icon;

                var pageRenderer = Platform.GetRenderer(page);
                if (pageRenderer != null)
                {
                    var tabbedPageHeader = Control.GetTabLabel(pageRenderer.Container) as TabbedPageHeader;
                    tabbedPageHeader.GetTabbedPageIcon().Pixbuf = icon.ToPixbuf();
                }
            }
            else if (e.PropertyName == TabbedPage.BarBackgroundColorProperty.PropertyName)
                UpdateBarBackgroundColor();
            else if (e.PropertyName == TabbedPage.BarTextColorProperty.PropertyName)
                UpdateBarTextColor();
        }

        private void UpdateCurrentPage()
        {
            Page page = Element.CurrentPage;

            if (page == null)
                return;

            int selectedIndex = 0;
            if (Element.SelectedItem != null)
            {
                for (var i = 0; i < Element.Children.Count; i++)
                {
                    if (Element.SelectedItem == Element.Children[i])
                    {
                        break;
                    }

                    selectedIndex++;
                }
            }

            Control.CurrentPage = selectedIndex;
        }

        private void UpdateChildrenOrderIndex()
        {
            for (var i = 0; i < Element.Children.Count; i++)
            {
                var page = PageController.InternalChildren[i];

                TabbedPage.SetIndex(page as Page, i);
            }
        }

        private void UpdateBarBackgroundColor()
        {
            if (Element == null)
                return;

            var barBackgroundColor = Element.BarBackgroundColor;
            var isDefaultColor = barBackgroundColor.IsDefault;

            if (isDefaultColor)
                return;

            for (var i = 0; i < Element.Children.Count; i++)
            {
                var child = Element.Children[i];
                var page = child as Page;
                var pageRenderer = Platform.GetRenderer(page);

                if (pageRenderer != null)
                {
                    var tabbedPageHeader = Control.GetTabLabel(pageRenderer.Container);

                    tabbedPageHeader.ModifyBg(StateType.Normal, barBackgroundColor.ToGtkColor());
                    tabbedPageHeader.ModifyBg(StateType.Active, barBackgroundColor.ToGtkColor());
                }
            }
        }

        private void UpdateBarTextColor()
        {
            if (Element == null)
                return;

            var barTextColor = Element.BarTextColor;

            var isDefaultColor = barTextColor.IsDefault;

            if (isDefaultColor)
                return;

            for (var i = 0; i < Element.Children.Count; i++)
            {
                var child = Element.Children[i];
                var page = child as Page;
                var pageRenderer = Platform.GetRenderer(page);

                if (pageRenderer != null)
                {
                    var tabbedPageHeader = Control.GetTabLabel(pageRenderer.Container) as TabbedPageHeader;

                    if (tabbedPageHeader != null)
                    {
                        tabbedPageHeader.GetTabbedPageTitle().ModifyFg(StateType.Normal, barTextColor.ToGtkColor());
                        tabbedPageHeader.GetTabbedPageTitle().ModifyFg(StateType.Active, barTextColor.ToGtkColor());
                    }
                }
            }
        }
    }
}