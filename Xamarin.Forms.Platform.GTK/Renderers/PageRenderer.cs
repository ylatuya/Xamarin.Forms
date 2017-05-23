using Gtk;
using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.GTK.Extensions;
using Container = Gtk.EventBox;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class PageRenderer : Container, IVisualElementRenderer, IEffectControlProvider
    {
        private bool _disposed;
        private bool _appeared;
        private VisualElementPackager _packager;
        private readonly PropertyChangedEventHandler _propertyChangedHandler;

        public PageRenderer()
        {
            _propertyChangedHandler = OnElementPropertyChanged;
        }

        public Controls.Page Control { get; private set; }

        //public Table Table { get; private set; }

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

                //if (Table == null)
                //{
                //    Table = new Table(1, 1, true);

                //    Add(Table);
                //}
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
            return Container.GetSizeRequest(widthConstraint, heightConstraint);
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

            _packager = new VisualElementPackager(this);
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

            SetElementSize(new Size(allocation.Width, allocation.Height));
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
                    //Table.Attach(Control, 0, 1, 0, 1);
                    Add(Control);
                }
            }

            ElementChanged?.Invoke(this, e);
        }

        protected virtual void UpdateBackgroundColor()
        {
            Color backgroundColor = Element.BackgroundColor;

            if (backgroundColor != Color.Default)
            {
                Control.SetBackgroundColor(backgroundColor.ToGtkColor());
            }
        }

        private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                UpdateBackgroundColor();
        }
    }
}