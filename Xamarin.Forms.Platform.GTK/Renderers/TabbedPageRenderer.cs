using Gtk;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.GTK.Controls;
using Xamarin.Forms.Platform.GTK.Extensions;
using Xamarin.Forms.PlatformConfiguration.GTKSpecific;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class TabbedPageRenderer : AbstractPageRenderer<NotebookWrapper, TabbedPage>
    {
        private const int DefaultIconWidth = 24;
        private const int DefaultIconHeight = 24;

        public override void SetElement(VisualElement element)
        {
            if (element != null && !(element is TabbedPage))
                throw new ArgumentException("Element must be a TabbedPage", "element");

            TabbedPage oldElement = Page;

            Element = element;

            if (oldElement != null)
            {
                oldElement.PropertyChanged -= OnElementPropertyChanged;
                ((INotifyCollectionChanged)oldElement.Children).CollectionChanged -= OnPagesChanged;
            }

            if (element != null)
            {
                if (Control == null)
                {
                    Control = new Controls.Page();
                    Add(Control);
                }

                if (Widget == null)
                {
                    Widget = new NotebookWrapper();
                    Control.Content = Widget;
                }
            }

            OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

            OnPagesChanged(Page.Children,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            Page.PropertyChanged += OnElementPropertyChanged;
            Page.PagesChanged += OnPagesChanged;

            UpdateCurrentPage();
            UpdateBarBackgroundColor();
            UpdateBarTextColor();
            UpdateTabPos();
            UpdateBackgroundImage();

            EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
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
        }

        protected override void UpdateBackgroundImage()
        {
            Widget.SetBackgroundImage(Page.BackgroundImage);
        }

        protected override void Dispose(bool disposing)
        {
            Page.PagesChanged -= OnPagesChanged;

            if (Widget != null)
            {
                Widget.NoteBook.SwitchPage -= OnNotebookPageSwitched;
            }

            base.Dispose(disposing);
        }

        private void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Widget.NoteBook.SwitchPage -= OnNotebookPageSwitched;

            e.Apply((o, i, c) => SetupPage((Page)o, i), (o, i) => TeardownPage((Page)o), Reset);
            ResetPages();
            SetPages();
            UpdateChildrenOrderIndex();
            UpdateCurrentPage();

            Widget.NoteBook.SwitchPage += OnNotebookPageSwitched;
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
            Widget.RemoveAllPages();
        }

        private void SetPages()
        {
            for (var i = 0; i < Page.Children.Count; i++)
            {
                var child = Page.Children[i];
                var page = child as Page;

                if (page == null)
                    continue;

                var pageRenderer = Platform.GetRenderer(page);

                if (pageRenderer != null)
                {
                    Widget.InsertPage(
                        pageRenderer.Container,
                        page.Title,
                        page.Icon?.ToPixbuf(new Size(DefaultIconWidth, DefaultIconHeight)),
                        i);
                }
            }

            Widget.ShowAll();
        }

        private void TeardownPage(Page page)
        {
            page.PropertyChanged -= OnPagePropertyChanged;

            var pageRenderer = Platform.GetRenderer(page);

            if (pageRenderer != null)
            {
                Widget.RemovePage(pageRenderer.Container);
            }

            Platform.SetRenderer(page, null);
        }

        private void Reset()
        {
            var i = 0;
            foreach (var page in Page.Children)
                SetupPage(page, i++);
        }

        private void OnPagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Xamarin.Forms.Page.TitleProperty.PropertyName)
            {
                var page = (Page)sender;
                var index = TabbedPage.GetIndex(page);

                Widget.SetTabLabelText(index, page.Title);
            }
            else if (e.PropertyName == Xamarin.Forms.Page.IconProperty.PropertyName)
            {
                var page = (Page)sender;
                var index = TabbedPage.GetIndex(page);
                var icon = page.Icon;

                Widget.SetTabIcon(index, icon.ToPixbuf());
            }
            else if (e.PropertyName == TabbedPage.BarBackgroundColorProperty.PropertyName)
                UpdateBarBackgroundColor();
            else if (e.PropertyName == TabbedPage.BarTextColorProperty.PropertyName)
                UpdateBarTextColor();
        }

        private void UpdateCurrentPage()
        {
            Page page = Page.CurrentPage;

            if (page == null)
                return;

            int selectedIndex = 0;
            if (Page.SelectedItem != null)
            {
                for (var i = 0; i < Page.Children.Count; i++)
                {
                    if (Page.Children[i].BindingContext.Equals(Page.SelectedItem))
                    {
                        break;
                    }

                    selectedIndex++;
                }
            }

            Widget.NoteBook.CurrentPage = selectedIndex;
            Widget.NoteBook.ShowAll();
        }

        private void UpdateChildrenOrderIndex()
        {
            for (var i = 0; i < Page.Children.Count; i++)
            {
                var page = PageController.InternalChildren[i];

                TabbedPage.SetIndex(page as Page, i);
            }
        }

        private void UpdateBarBackgroundColor()
        {
            if (Element == null || Page.BarBackgroundColor.IsDefault)
                return;

            var barBackgroundColor = Page.BarBackgroundColor.ToGtkColor();

            for (var i = 0; i < Page.Children.Count; i++)
            {
                Widget.SetTabBackgroundColor(i, barBackgroundColor);
            }
        }

        private void UpdateBarTextColor()
        {
            if (Element == null || Page.BarTextColor.IsDefault)
                return;

            var barTextColor = Page.BarTextColor.ToGtkColor();

            for (var i = 0; i < Page.Children.Count; i++)
            {
                Widget.SetTabTextColor(i, barTextColor);
            }
        }

        private void UpdateTabPos()
        {
            var tabposition = Page.OnThisPlatform().GetTabPosition();

            switch(tabposition)
            {
                case TabPosition.Top:
                    Widget.NoteBook.TabPos = PositionType.Top;
                    break;
                case TabPosition.Bottom:
                    Widget.NoteBook.TabPos = PositionType.Bottom;
                    break;
            }
        }

        private void OnNotebookPageSwitched(object o, SwitchPageArgs args)
        {
            var currentPageIndex = (int)args.PageNum;
            Element currentSelectedChild = Page.Children.Count > currentPageIndex
                ? Page.Children[currentPageIndex]
                : null;

            if (currentSelectedChild != null)
            {
                ElementController.SetValueFromRenderer(TabbedPage.SelectedItemProperty, currentSelectedChild.BindingContext);
            }
        }
    }
}