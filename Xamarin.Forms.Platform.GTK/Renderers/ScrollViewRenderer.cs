using Gtk;
using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class ScrollViewRenderer : ViewRenderer<ScrollView, ScrolledWindow>
    {
        private VisualElement _currentView;

        protected IScrollViewController Controller
        {
            get { return Element; }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ScrollView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.ScrollToRequested -= OnScrollToRequested;
            }

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new ScrolledWindow
                    {
                        CanFocus = true,
                        ShadowType = ShadowType.None,
                        BorderWidth = 0,
                        HscrollbarPolicy = PolicyType.Automatic,
                        VscrollbarPolicy = PolicyType.Automatic
                    });

                    Control.Hadjustment.ValueChanged += OnScrollEvent;
                    Control.Vadjustment.ValueChanged += OnScrollEvent;
                }

                Element.ScrollToRequested += OnScrollToRequested;

                UpdateOrientation();
                LoadContent();
                UpdateContentSize();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ScrollView.ContentSizeProperty.PropertyName)
                UpdateContentSize();
            else if(e.PropertyName == nameof(ScrollView.Content))
                LoadContent();
            else if (e.PropertyName == ScrollView.OrientationProperty.PropertyName)
                UpdateOrientation();
        }

        protected override void Dispose(bool disposing)
        {
            if (Control != null)
            {
                Control.ScrollEvent -= OnScrollEvent;
                Control.Hadjustment.ValueChanged -= OnScrollEvent;
                Control.Vadjustment.ValueChanged -= OnScrollEvent;
            }

            base.Dispose(disposing);
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            SizeRequest result = base.GetDesiredSize(widthConstraint, heightConstraint);
            result.Minimum = new Size(40, 40);

            return result;
        }

        private void OnScrollEvent(object o, EventArgs args)
        {
            Controller.SetScrolledPosition(Control.Hadjustment.Value, Control.Vadjustment.Value);
        }

        private void LoadContent()
        {
            if (_currentView != null)
            {
                _currentView.Cleanup();
            }

            _currentView = Element.Content;

            IVisualElementRenderer renderer = null;
            if (_currentView != null)
            {
                renderer = _currentView.GetOrCreateRenderer();
            }

            Viewport viewPort = new Viewport();
            viewPort.Add(renderer != null ? renderer.Container : null);
            Control.Add(viewPort);
        }

        private void UpdateOrientation()
        {
            switch (Element.Orientation)
            {
                case ScrollOrientation.Vertical:
                    Control.HscrollbarPolicy = PolicyType.Never;
                    Control.VscrollbarPolicy = PolicyType.Automatic;
                    break;
                case ScrollOrientation.Horizontal:
                    Control.HscrollbarPolicy = PolicyType.Automatic;
                    Control.VscrollbarPolicy = PolicyType.Never;
                    break;
                case ScrollOrientation.Both:
                    Control.HscrollbarPolicy = PolicyType.Automatic;
                    Control.VscrollbarPolicy = PolicyType.Automatic;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Element.Orientation));
            }
        }

        private void OnScrollToRequested(object sender, ScrollToRequestedEventArgs e)
        {
            double x = e.ScrollX, y = e.ScrollY;

            ScrollToMode mode = e.Mode;

            if (mode == ScrollToMode.Element)
            {
                Point pos = Controller.GetScrollPositionForElement((VisualElement)e.Element, e.Position);
                x = pos.X;
                y = pos.Y;
                mode = ScrollToMode.Position;
            }

            if (mode == ScrollToMode.Position)
            {
                Control.Hadjustment.Value = x;
                Control.Vadjustment.Value = y;
            }

            Controller.SendScrollFinished();
        }

        private void UpdateContentSize()
        {
            var contentSize = Element.ContentSize;

            // TODO: 
            /*
            var height = Convert.ToInt32(contentSize.Height);
            var width = Convert.ToInt32(contentSize.Width);

            Control.SetSizeRequest(width, height);
            */
        }
    }
}