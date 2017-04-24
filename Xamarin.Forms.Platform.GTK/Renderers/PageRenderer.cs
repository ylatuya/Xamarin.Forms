using Gtk;
using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;
using Container = Gtk.EventBox;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class PageRenderer : Container, IVisualElementRenderer
    {
        private VisualElementPackager _packager;
        private readonly PropertyChangedEventHandler _propertyChangedHandler;

        public PageRenderer()
        {
            _propertyChangedHandler = OnElementPropertyChanged;
        }

        public VisualElement Element { get; private set; }

        public Container Container => this;

        IElementController ElementController => Element as IElementController;

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        public void SetElement(VisualElement element)
        {
            VisualElement oldElement = Element;
            Element = element;

            if (element != null)
            {
                element.PropertyChanged += _propertyChangedHandler;
            }

            OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));
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

            Dispose(true);
        }

        protected override void OnShown()
        {
            base.OnShown();

            _packager = new VisualElementPackager(this);
            _packager.Load();

            UpdateBackgroundColor();
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
                Element.PropertyChanged -= OnElementPropertyChanged;
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
            ElementChanged?.Invoke(this, e);
        }

        private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                UpdateBackgroundColor();
        }

        protected virtual void UpdateBackgroundColor()
        {
            Color backgroundColor = Element.BackgroundColor;

            if (backgroundColor != Color.Default)
            {
                ModifyBg(StateType.Normal, backgroundColor.ToGtkColor());
            }
        }
    }
}