using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;
using Container = Gtk.EventBox;
using Control = Gtk.Widget;

namespace Xamarin.Forms.Platform.GTK
{
    public class VisualElementRenderer<TElement, TNativeElement> : Container, IVisualElementRenderer 
        where TElement : VisualElement
        where TNativeElement : Control
    {
        private readonly PropertyChangedEventHandler _propertyChangedHandler;
        private readonly List<EventHandler<VisualElementChangedEventArgs>> _elementChangedHandlers = new List<EventHandler<VisualElementChangedEventArgs>>();

        protected VisualElementRenderer()
        {
            _propertyChangedHandler = OnElementPropertyChanged;
        }

        public TNativeElement Control { get; set; }

        public TElement Element { get; set; }

        public Container Container => this;

        VisualElement IVisualElementRenderer.Element
        {
            get
            {
                return Element;
            }
        }

        protected IElementController ElementController => Element as IElementController;

        event EventHandler<VisualElementChangedEventArgs> IVisualElementRenderer.ElementChanged
        {
            add { _elementChangedHandlers.Add(value); }
            remove { _elementChangedHandlers.Remove(value); }
        }

        public event EventHandler<ElementChangedEventArgs<TElement>> ElementChanged;

        void IVisualElementRenderer.SetElement(VisualElement element)
        {
            SetElement((TElement)element);
        }

        public void SetElement(TElement element)
        {
            var oldElement = Element;
            Element = element;

            if (oldElement != null)
            {
                oldElement.PropertyChanged -= _propertyChangedHandler;
            }

            if (element != null)
            {
                element.PropertyChanged += _propertyChangedHandler;
            }

            OnElementChanged(new ElementChangedEventArgs<TElement>(oldElement, element));
        }

        public void SetElementSize(Size size)
        {
            Layout.LayoutChildIntoBoundingRegion(Element, new Rectangle(Element.X, Element.Y, size.Width, size.Height));
        }

        public virtual void UpdateLayout()
        {
            Rectangle bounds = Element.Bounds;
            Container.MoveTo(bounds.X, bounds.Y);
            Container.SetSizeRequest((int)Element.Bounds.Width, (int)Element.Bounds.Height);
        }

        public virtual SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            if (Children.Length == 0)
                return new SizeRequest();

            var desiredSize = Control.SizeRequest();
            var width =  desiredSize.Width;
            var height = desiredSize.Height;

            View view = Element as View;

            if (width == 0) width = -1;

            if (height == 0) height = -1;

            return new SizeRequest(
                new Size(width, height),
                Size.Zero);
        }

        public sealed override void Dispose()
        {
            base.Dispose();
            Dispose(true);
        }

        protected virtual void OnElementChanged(ElementChangedEventArgs<TElement> e)
        {
            var args = new VisualElementChangedEventArgs(e.OldElement, e.NewElement);
            for (var i = 0; i < _elementChangedHandlers.Count; i++)
                _elementChangedHandlers[i](this, args);

            ElementChanged?.Invoke(this, e);
        }

        protected virtual void SetNativeControl(TNativeElement view)
        {
            Control = view;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            Element = null;
        }

        protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
            {
                UpdateEnabled();
            }
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            {
                UpdateBackgroundColor();
            }
            else if (e.PropertyName == VisualElement.WidthProperty.PropertyName ||
                     e.PropertyName == VisualElement.HeightProperty.PropertyName)
            {
                Platform.InvalidateParentLayout(Element);
            }
        }

        protected virtual void UpdateBackgroundColor()
        {
            Color backgroundColor = Element.BackgroundColor;

            if (backgroundColor != Color.Default)
            {
                var control = Control as Control;
                var parent = control?.Parent as Container;

                if (parent == null) return;

                parent.VisibleWindow = backgroundColor != Color.Transparent;
                parent.ModifyBg(Gtk.StateType.Normal, backgroundColor.ToGtkColor());
            }
        }

        private void UpdateEnabled()
        {
            var control = Control as Control;

            if (control != null)
                control.Visible = Element.IsEnabled;
        }
    }
}