using Gtk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.GTK.Controls;
using Xamarin.Forms.Platform.GTK.Extensions;
using Container = Gtk.EventBox;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class NavigationPageRenderer : Container, IVisualElementRenderer, IEffectControlProvider
    {
        private bool _disposed;
        private bool _appeared;
        private Stack<NavigationChildPage> _currentStack;
        private VisualElementTracker<Page, Container> _tracker;

        IPageController PageController => Element as IPageController;

        INavigationPageController NavigationController => Element as INavigationPageController;

        public NavigationPageRenderer()
        {
            _currentStack = new Stack<NavigationChildPage>();
        }

        public Table Control { get; private set; }

        public Container Container => this;

        public NavigationPage Element { get; private set; }

        public bool Disposed { get { return _disposed; } }

        VisualElement IVisualElementRenderer.Element => Element;

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
                if (Element != null)
                {
                    PageController?.SendDisappearing();
                    ((Element as IPageContainer<Page>)?.CurrentPage as IPageController)?.SendDisappearing();
                    Element.PropertyChanged -= HandlePropertyChanged;
                }

                _disposed = true;
            }

            base.Dispose();
        }

        protected override void OnShown()
        {
            Platform.NativeToolbarTracker.Navigation = Element;

            if (_appeared)
                return;

            _appeared = true;

            PageController.SendAppearing();

            base.OnShown();
        }

        protected override void OnDestroyed()
        {
            if (!_appeared)
                return;

            Platform.NativeToolbarTracker.TryHide(Element as NavigationPage);
            _appeared = false;

            PageController.SendDisappearing();

            base.OnDestroyed();
        }

        protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= HandlePropertyChanged;

            if (e.NewElement != null)
                e.NewElement.PropertyChanged += HandlePropertyChanged;

            ElementChanged?.Invoke(this, e);
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
            var oldElement = Element;
            Element = element as NavigationPage;

            if (element != null)
            {
                if (Control == null)
                {
                    Control = new Table(1, 1, true);

                    Add(Control);
                }
            }

            Init();

            OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

            EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);
        }

        public void SetElementSize(Size size)
        {
            Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
        }

        public Task<bool> PopToRootAsync(Page page, bool animated = true)
        {
            return OnPopToRoot(page, animated);
        }

        public Task<bool> PopViewAsync(Page page, bool animated = true)
        {
            return OnPop(page, animated);
        }

        public Task<bool> PushPageAsync(Page page, bool animated = true)
        {
            return OnPush(page, animated);
        }

        protected virtual async Task<bool> OnPopToRoot(Page page, bool animated)
        {
            var removed = await PopToRootPageAsync(page, animated);
            Platform.NativeToolbarTracker.UpdateToolBar();
            return removed;
        }

        protected virtual async Task<bool> OnPop(Page page, bool animated)
        {
            var removed = await PopPageAsync(page, animated);
            Platform.NativeToolbarTracker.UpdateToolBar();
            return removed;
        }

        protected virtual async Task<bool> OnPush(Page page, bool animated)
        {
            var shown = await AddPage(page, animated);
            Platform.NativeToolbarTracker.UpdateToolBar();
            return shown;
        }

        protected virtual void ConfigurePageRenderer()
        {
            Container.IsFocus = true;
        }

        private void Init()
        {
            ConfigurePageRenderer();

            var navPage = (NavigationPage)Element;

            if (navPage.CurrentPage == null)
                throw new InvalidOperationException(
                    "NavigationPage must have a root Page before being used. Either call PushAsync with a valid Page, or pass a Page to the constructor before usage.");

            Platform.NativeToolbarTracker.Navigation = navPage;
            NavigationController.PushRequested += OnPushRequested;
            NavigationController.PopRequested += OnPopRequested;
            NavigationController.PopToRootRequested += OnPopToRootRequested;
            NavigationController.RemovePageRequested += OnRemovedPageRequested;
            NavigationController.InsertPageBeforeRequested += OnInsertPageBeforeRequested;

            navPage.Popped += (sender, e) => Platform.NativeToolbarTracker.UpdateToolBar();
            navPage.PoppedToRoot += (sender, e) => Platform.NativeToolbarTracker.UpdateToolBar();

            UpdateBarBackgroundColor();
            UpdateBarTextColor();

            ((INavigationPageController)navPage).Pages.ForEach(async p => await PushPageAsync(p, false));

            UpdateBackgroundColor();
        }

        private void OnPushRequested(object sender, NavigationRequestedEventArgs e)
        {
            e.Task = PushPageAsync(e.Page, e.Animated);
        }

        private void OnPopRequested(object sender, NavigationRequestedEventArgs e)
        {
            e.Task = PopViewAsync(e.Page, e.Animated);
        }

        private void OnPopToRootRequested(object sender, NavigationRequestedEventArgs e)
        {
            e.Task = PopToRootAsync(e.Page, e.Animated);
        }

        private void OnRemovedPageRequested(object sender, NavigationRequestedEventArgs e)
        {
            RemovePage(e.Page, true);
            Platform.NativeToolbarTracker.UpdateToolBar();
        }

        private void OnInsertPageBeforeRequested(object sender, NavigationRequestedEventArgs e)
        {
            InsertPageBefore(e.Page, e.BeforePage);
        }

        private Task<bool> AddPage(Page page, bool animated)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            Page oldPage = null;
            if (_currentStack.Count >= 1)
                oldPage = _currentStack.Peek().Page;

            _currentStack.Push(new NavigationChildPage(page));

            if (Platform.GetRenderer(page) == null)
                Platform.SetRenderer(page, Platform.CreateRenderer(page));

            var pageRenderer = Platform.GetRenderer(page);

            Control.Attach(pageRenderer.Container, 0, 1, 0, 1);
            pageRenderer.Container.ShowAll();

            (page as IPageController)?.SendAppearing();

            return Task.FromResult(true);
        }

        private void RemovePage(Page page, bool removeFromStack)
        {
            (page as IPageController)?.SendDisappearing();
            var target = Platform.GetRenderer(page);

            if (Control.Children.Length > 0)
            {
                Control.Remove(target.Container);
            }

            if (Control.Children != null)
            {
                foreach (var children in Control.Children)
                {
                    children.ShowAll();
                }

                Control.ShowAll();
            }

            target?.Dispose();

            if (removeFromStack)
            {
                var newStack = new Stack<NavigationChildPage>();
                foreach (var stack in _currentStack)
                {
                    if (stack.Page != page)
                    {
                        newStack.Push(stack);
                    }
                }
                _currentStack = newStack;
            }
        }

        private void InsertPageBefore(Page page, Page before)
        {
            if (before == null)
                throw new ArgumentNullException(nameof(before));
            if (page == null)
                throw new ArgumentNullException(nameof(page));
        }

        private Task<bool> PopPageAsync(Page page, bool animated)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            var wrapper = _currentStack.Peek();
            if (page != wrapper.Page)
                throw new NotSupportedException("Popped page does not appear on top of current navigation stack, please file a bug.");

            _currentStack.Pop();
            (page as IPageController)?.SendDisappearing();

            var target = Platform.GetRenderer(page);
            var previousPage = _currentStack.Peek().Page;

            RemovePage(page, false);

            return Task.FromResult(true);
        }

        private Task<bool> PopToRootPageAsync(Page page, bool animated)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            (page as IPageController)?.SendDisappearing();

            for (int i = _currentStack.Count; i > 1; i--)
            {
                var lastPage = _currentStack.Pop();

                RemovePage(lastPage.Page, false);
            }

            return Task.FromResult(true);
        }

        private void UpdateBackgroundColor()
        {
            if (Element == null)
                return;

            var backgroundColor = Element.BackgroundColor == Color.Default ? Color.White : Element.BackgroundColor;

            Container.ModifyBg(StateType.Normal, backgroundColor.ToGtkColor());
        }

        private void UpdateBarBackgroundColor()
        {
            Platform.NativeToolbarTracker.UpdateToolBar();
        }

        private void UpdateBarTextColor()
        {
            Platform.NativeToolbarTracker.UpdateToolBar();
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == NavigationPage.BarBackgroundColorProperty.PropertyName)
                UpdateBarBackgroundColor();
            else if (e.PropertyName == NavigationPage.BarTextColorProperty.PropertyName)
                UpdateBarTextColor();
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                UpdateBackgroundColor();
        }
    }
}