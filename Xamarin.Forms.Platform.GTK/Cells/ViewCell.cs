using Gtk;
using System;
using static Xamarin.Forms.Platform.GTK.Platform;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class ViewCell : EventBox
    {
        private WeakReference<IVisualElementRenderer> _rendererRef;
        private Xamarin.Forms.ViewCell _viewCell;

        public Xamarin.Forms.ViewCell Cell
        {
            get { return _viewCell; }
            set
            {
                if (_viewCell == value)
                    return;

                UpdateCell(value);
            }
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            var view = Cell.View;

            Layout.LayoutChildIntoBoundingRegion(
               view, 
               new Rectangle(0, 0, allocation.Width, allocation.Height));

            if (_rendererRef == null)
                return;

            IVisualElementRenderer renderer;
            if (_rendererRef.TryGetTarget(out renderer))
            {
                renderer.Container.SetSizeRequest(
                    Convert.ToInt32(view.Bounds.Width),
                    Convert.ToInt32(view.Bounds.Height));
            }

            base.OnSizeAllocated(allocation);
        }

        public override void Dispose()
        {
            IVisualElementRenderer renderer;
            if (_rendererRef != null && _rendererRef.TryGetTarget(out renderer) && renderer.Element != null)
            {
                _rendererRef = null;
            }

            base.Dispose();
        }

        private IVisualElementRenderer GetNewRenderer()
        {
            var newRenderer = Platform.CreateRenderer(_viewCell.View);
            _rendererRef = new WeakReference<IVisualElementRenderer>(newRenderer);
            Add(newRenderer.Container);

            return newRenderer;
        }

        private void UpdateCell(Xamarin.Forms.ViewCell cell)
        {
            if (_viewCell != null)
                Device.BeginInvokeOnMainThread(_viewCell.SendDisappearing);

            _viewCell = cell;

            Device.BeginInvokeOnMainThread(_viewCell.SendAppearing);

            IVisualElementRenderer renderer;
            if (_rendererRef == null || !_rendererRef.TryGetTarget(out renderer))
                renderer = GetNewRenderer();
            else
            {
                if (renderer.Element != null && renderer == Platform.GetRenderer(renderer.Element))
                    renderer.Element.ClearValue(Platform.RendererProperty);

                var type = Internals.Registrar.Registered.GetHandlerType(_viewCell.View.GetType());
                if (renderer.GetType() == type || (renderer is DefaultRenderer && type == null))
                    renderer.SetElement(_viewCell.View);
                else
                {
                    renderer = GetNewRenderer();
                }
            }

            Platform.SetRenderer(_viewCell.View, renderer);
        }
    }
}