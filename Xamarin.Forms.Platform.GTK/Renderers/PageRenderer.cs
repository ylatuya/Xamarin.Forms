using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.GTK.Extensions;
using Xamarin.Forms.Platform.GTK.Packagers;
using Container = Gtk.EventBox;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class PageRenderer : Container, IVisualElementRenderer, IEffectControlProvider
    {
        private bool _disposed;
        private bool _appeared;
        private PageElementPackager _packager;
        private readonly PropertyChangedEventHandler _propertyChangedHandler;

        public PageRenderer()
        {
            _propertyChangedHandler = OnElementPropertyChanged;
        }

        public Controls.Page Control { get; private set; }

        public bool Disposed { get { return _disposed; } }

        Page Page => Element as Page;

        public VisualElement Element { get; private set; }

        public Container Container => this;

        IElementController ElementController => Element as IElementController;

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        void IEffectControlProvider.RegisterEffect(Effect effect)
        {
            var platformEffect = effect as PlatformEffect;
            if (platformEffect != null)
                platformEffect.SetContainer(Container);
        }

        public void SetElement(VisualElement element)
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
            Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
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

                if (_packager != null)
                {
                    _packager.Dispose();
                    _packager = null;
                }

                Element = null;
                _disposed = true;

                Dispose(true);
            }
        }

        protected override void OnShown()
        {
            base.OnShown();

            if (_appeared || _disposed)
                return;

            _packager = new PageElementPackager(this);
            _packager.Load();

            UpdateBackgroundColor();

            Page.SendAppearing();
            _appeared = true;
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();

            if (!_appeared || _disposed)
                return;

            Page.SendDisappearing();
            _appeared = false;
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);

            var toolbarSize = Platform.NativeToolbarTracker.GetCurrentToolbarSize();
            var pageContentSize = new Gdk.Rectangle(0, 0, allocation.Width, allocation.Height - toolbarSize.Height);

            SetElementSize(pageContentSize.ToSize());
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

                if (_packager != null)
                {
                    _packager.Dispose();
                    _packager = null;
                }

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

        private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                UpdateBackgroundColor();
            else if (e.PropertyName == Page.BackgroundImageProperty.PropertyName)
                UpdateBackgroundImage();
        }

        private void UpdateBackgroundImage()
        {
            Control.SetBackgroundImage(Page.BackgroundImage);
        }
    }
}