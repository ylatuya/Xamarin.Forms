using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gdk;
using Gtk;
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

        protected override bool OnExposeEvent(EventExpose evnt)
        {
            base.OnExposeEvent(evnt);

            Rectangle bounds = Element.Bounds;
            Container.MoveTo(bounds.X, bounds.Y);

            var width = (int)bounds.Width;
            var height = (int)bounds.Height;

            Container.SetSize(width, height);

            for (var i = 0; i < ElementController.LogicalChildren.Count; i++)
            {
                var child = ElementController.LogicalChildren[i] as VisualElement;
                if (child != null)
                {
                    var renderer = Platform.GetRenderer(child);
                    renderer?.Container.MoveTo(child.Bounds.X, child.Bounds.Y);
                }
            }

            return true;
        }

        public virtual SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            if (Children.Length == 0)
                return new SizeRequest();

            return Control.GetSizeRequest(widthConstraint, heightConstraint);
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
                UpdateEnabled();
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                UpdateBackgroundColor();
        }

        protected virtual void UpdateBackgroundColor()
        {
            Color backgroundColor = Element.BackgroundColor;

            bool isDefault = backgroundColor.IsDefaultOrTransparent();

            if (!isDefault)
            {
                Container.ModifyBg(StateType.Normal, backgroundColor.ToGtkColor());
            }

            Container.VisibleWindow = !isDefault;
        }

        private void UpdateEnabled()
        {
            var control = Control as Control;

            if (control != null)
                control.Visible = Element.IsEnabled;
        }
    }
}