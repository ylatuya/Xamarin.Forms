using Gtk;
using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.GTK.Extensions;
using Container = Gtk.EventBox;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public abstract class AbstractPageRenderer<TWidget, TPage> : Container, IPageControl, IVisualElementRenderer, IEffectControlProvider
        where TWidget : Widget
        where TPage : Page
    {
        private Gdk.Rectangle _lastAllocation;
        protected bool _disposed;
        protected bool _appeared;
        protected readonly PropertyChangedEventHandler _propertyChangedHandler;

        protected AbstractPageRenderer()
        {
            _propertyChangedHandler = OnElementPropertyChanged;
        }

        public Controls.Page Control { get; protected set; }

        public TWidget Widget { get; protected set; }

        public VisualElement Element { get; protected set; }

        public TPage Page => Element as TPage;

        public bool Disposed { get { return _disposed; } }

        public Container Container => this;

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        protected IElementController ElementController => Element as IElementController;

        protected IPageController PageController => Element as IPageController;

        void IEffectControlProvider.RegisterEffect(Effect effect)
        {
            var platformEffect = effect as PlatformEffect;
            if (platformEffect != null)
                platformEffect.SetContainer(Container);
        }

        public virtual void SetElement(VisualElement element)
        {
            VisualElement oldElement = Element;
            Element = element;

            if (element != null)
            {
                element.PropertyChanged += _propertyChangedHandler;
            }

            OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

            EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);
        }

        public void SetElementSize(Size size)
        {
            var bounds = new Rectangle(Element.X, Element.Y, size.Width, size.Height);

            if (Element.Bounds != bounds)
            {
                Element.Layout(bounds);
            }
        }

        public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return Container.GetDesiredSize(widthConstraint, heightConstraint);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (!_disposed)
            {
                if (_appeared)
                    Page.SendDisappearing();

                _appeared = false;

                Dispose(true);
                _disposed = true;
            }
        }

        protected override void OnShown()
        {
            base.OnShown();

            if (_appeared || _disposed)
                return;

            UpdateBackgroundColor();

            PageController.SendAppearing();
            _appeared = true;
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();

            if (!_appeared || _disposed)
                return;

            PageController.SendDisappearing();
            _appeared = false;
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);

            if (_lastAllocation != allocation)
            {
                _lastAllocation = allocation;
                SetPageSize(_lastAllocation.Width, _lastAllocation.Height);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Element != null)
                {
                    Element.PropertyChanged -= OnElementPropertyChanged;
                }

                Platform.SetRenderer(Element, null);

                this.RemoveFromContainer(Control);
                Control = null;
                Element = null;
            }
        }

        protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    Control = new Controls.Page();
                    Add(Control);
                }

                VisibleWindow = Page.ShouldDisplayNativeWindow();
                UpdateBackgroundImage();
            }

            ElementChanged?.Invoke(this, e);
        }

        protected virtual void UpdateBackgroundColor()
        {
            Color backgroundColor = Element.BackgroundColor;

            if (backgroundColor.IsDefaultOrTransparent())
            {
                Control.SetBackgroundColor(null);
            }
            else
            {
                Control.SetBackgroundColor(backgroundColor.ToGtkColor());
            }
        }

        protected virtual void UpdateBackgroundImage()
        {
            Control.SetBackgroundImage(Page.BackgroundImage);
        }

        protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                UpdateBackgroundColor();
            else if (e.PropertyName == Xamarin.Forms.Page.BackgroundImageProperty.PropertyName)
                UpdateBackgroundImage();
        }

        private void SetPageSize(int width, int height)
        {
            var toolbarSize = Platform.NativeToolbarTracker.GetCurrentToolbarSize();
            var pageContentSize = new Gdk.Rectangle(0, 0, width, height - toolbarSize.Height);
            SetElementSize(pageContentSize.ToSize());
        }
    }
}
