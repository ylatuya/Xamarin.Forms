using Gtk;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.GTK.Controls;
using Xamarin.Forms.Platform.GTK.Extensions;
using Xamarin.Forms.PlatformConfiguration.GTKSpecific;
using Container = Gtk.EventBox;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class TabbedPageRenderer : Container, IVisualElementRenderer, IEffectControlProvider
    {
        private bool _disposed;
        private VisualElementTracker<Page, Container> _tracker;

        public NotebookWrapper Control { get; private set; }

        public TabbedPage Element { get; private set; }

        IElementController ElementController => Element as IElementController;

        IPageController PageController => Element as IPageController;

        VisualElement IVisualElementRenderer.Element => Element;

        public Container Container => this;

        public bool Disposed { get { return _disposed; } }

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        void IEffectControlProvider.RegisterEffect(Effect effect)
        {
            var platformEffect = effect as PlatformEffect;
            if (platformEffect != null)
                platformEffect.SetContainer(Container);
        }

        protected VisualElementTracker<Page, Container> Tracker
        {
            get { return _tracker; }
            set
            {
                if (_tracker == value)
                    return;

                if (_tracker != null)
                    _tracker.Dispose();

                _tracker = value;
            }
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                PageController.SendDisappearing();
                Element.PagesChanged -= OnPagesChanged;

                if (Control?.NoteBook != null)
                {
                    Control.NoteBook.SwitchPage -= OnNotebookPageSwitched;
                }
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
                    Control = new NotebookWrapper();

                    Add(Control);
                }
            }

            OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

            OnPagesChanged(Element.Children,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            Element.PropertyChanged += OnElementPropertyChanged;
            Element.PagesChanged += OnPagesChanged;

            UpdateCurrentPage();
            UpdateBarBackgroundColor();
            UpdateBarTextColor();
            UpdateTabPos();
            UpdateBackgroundImage();

            EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);
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
                UpdateBarTextColor();
                UpdateBarBackgroundColor();
            }
            else if (e.PropertyName == TabbedPage.BarTextColorProperty.PropertyName)
                UpdateBarTextColor();
            else if (e.PropertyName == TabbedPage.BarBackgroundColorProperty.PropertyName)
                UpdateBarBackgroundColor();
            else if (e.PropertyName ==
                  PlatformConfiguration.GTKSpecific.TabbedPage.TabPositionProperty.PropertyName)
                UpdateTabPos();
            else if (e.PropertyName == Page.BackgroundImageProperty.PropertyName)
                UpdateBackgroundImage();
        }

        private void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Control.NoteBook.SwitchPage -= OnNotebookPageSwitched;

            e.Apply((o, i, c) => SetupPage((Page)o, i), (o, i) => TeardownPage((Page)o), Reset);
            ResetPages();
            SetPages();
            UpdateChildrenOrderIndex();
            UpdateCurrentPage();

            Control.NoteBook.SwitchPage += OnNotebookPageSwitched;
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

        private void ResetPages()
        {
            Control.RemoveAllPages();
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
                        page.Title,
                        page.Icon?.ToPixbuf(),
                        i);
                }
            }

            Control.ShowAll();
        }

        private void TeardownPage(Page page)
        {
            page.PropertyChanged -= OnPagePropertyChanged;

            var pageRenderer = Platform.GetRenderer(page);

            if (pageRenderer != null)
            {
                Control.RemovePage(pageRenderer.Container);
            }

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

                Control.SetTabLabelText(index, page.Title);
            }
            else if (e.PropertyName == Page.IconProperty.PropertyName)
            {
                var page = (Page)sender;
                var index = TabbedPage.GetIndex(page);
                var icon = page.Icon;

                Control.SetTabIcon(index, icon.ToPixbuf());
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
                    if (Element.Children[i].BindingContext.Equals(Element.SelectedItem))
                    {
                        break;
                    }

                    selectedIndex++;
                }
            }

            Control.NoteBook.CurrentPage = selectedIndex;
            Control.NoteBook.ShowAll();
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
            if (Element == null || Element.BarBackgroundColor.IsDefault)
                return;

            var barBackgroundColor = Element.BarBackgroundColor.ToGtkColor();

            for (var i = 0; i < Element.Children.Count; i++)
            {
                Control.SetTabBackgroundColor(i, barBackgroundColor);
            }
        }

        private void UpdateBarTextColor()
        {
            if (Element == null || Element.BarTextColor.IsDefault)
                return;

            var barTextColor = Element.BarTextColor.ToGtkColor();

            for (var i = 0; i < Element.Children.Count; i++)
            {
                Control.SetTabTextColor(i, barTextColor);
            }
        }

        private void UpdateTabPos()
        {
            var tabposition = Element.OnThisPlatform().GetTabPosition();

            switch(tabposition)
            {
                case TabPosition.Top:
                    Control.NoteBook.TabPos = PositionType.Top;
                    break;
                case TabPosition.Bottom:
                    Control.NoteBook.TabPos = PositionType.Bottom;
                    break;
            }
        }

        private void UpdateBackgroundImage()
        {
            Control.SetBackgroundImage(Element.BackgroundImage);
        }

        private void OnNotebookPageSwitched(object o, SwitchPageArgs args)
        {
            var currentPageIndex = (int)args.PageNum;
            Element currentSelectedChild = Element.Children.Count > currentPageIndex
                ? Element.Children[currentPageIndex]
                : null;

            if (currentSelectedChild != null)
            {
                ElementController.SetValueFromRenderer(TabbedPage.SelectedItemProperty, currentSelectedChild.BindingContext);
            }
        }
    }
}