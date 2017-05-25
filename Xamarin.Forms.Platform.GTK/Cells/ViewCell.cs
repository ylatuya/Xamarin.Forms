using System;
using static Xamarin.Forms.Platform.GTK.Platform;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public class ViewCell : CellBase
    {
        private WeakReference<IVisualElementRenderer> _rendererRef;

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            var viewCell = Cell as Xamarin.Forms.ViewCell;
            var view = viewCell.View;

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

        protected override void UpdateCell()
        {
            var viewCell = Cell as Xamarin.Forms.ViewCell;

            if (viewCell != null)
                Device.BeginInvokeOnMainThread(viewCell.SendDisappearing);

            Device.BeginInvokeOnMainThread(viewCell.SendAppearing);

            IVisualElementRenderer renderer;
            if (_rendererRef == null || !_rendererRef.TryGetTarget(out renderer))
                renderer = GetNewRenderer();
            else
            {
                if (renderer.Element != null && renderer == Platform.GetRenderer(renderer.Element))
                    renderer.Element.ClearValue(Platform.RendererProperty);

                var type = Internals.Registrar.Registered.GetHandlerType(viewCell.View.GetType());
                if (renderer.GetType() == type || (renderer is DefaultRenderer && type == null))
                    renderer.SetElement(viewCell.View);
                else
                {
                    renderer = GetNewRenderer();
                }
            }

            Platform.SetRenderer(viewCell.View, renderer);
        }

        private IVisualElementRenderer GetNewRenderer()
        {
            var viewCell = Cell as Xamarin.Forms.ViewCell;
            var newRenderer = Platform.CreateRenderer(viewCell.View);

            _rendererRef = new WeakReference<IVisualElementRenderer>(newRenderer);
            Add(newRenderer.Container);

            return newRenderer;
        }
    }
}