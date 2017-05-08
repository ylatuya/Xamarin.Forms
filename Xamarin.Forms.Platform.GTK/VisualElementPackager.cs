using System;

namespace Xamarin.Forms.Platform.GTK
{
    public class VisualElementPackager : IDisposable
    {
        private bool _isDisposed;
        private VisualElement _element;

        private IElementController ElementController => Renderer.Element as IElementController;

        protected IVisualElementRenderer Renderer { get; set; }

        public VisualElementPackager(IVisualElementRenderer renderer)
        {
            if (renderer == null)
                throw new ArgumentNullException(nameof(renderer));

            Renderer = renderer;
            renderer.ElementChanged += OnRendererElementChanged;

            SetElement(null, Renderer.Element);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Load()
        {
            for (var i = 0; i < ElementController.LogicalChildren.Count; i++)
            {
                var child = ElementController.LogicalChildren[i] as VisualElement;
                if (child != null)
                    OnChildAdded(child);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                SetElement(_element, null);
                if (Renderer != null)
                {
                    Renderer.ElementChanged -= OnRendererElementChanged;
                    Renderer = null;
                }
            }

            _isDisposed = true;
        }

        protected virtual void OnChildAdded(VisualElement view)
        {
            var viewRenderer = Platform.CreateRenderer(view);
            Platform.SetRenderer(view, viewRenderer);

            Gtk.Container container = Renderer.Container;
            Gtk.Fixed fixedControl = null;

            if (Renderer is Renderers.LayoutRenderer)
            {
                fixedControl = (Renderer as Renderers.LayoutRenderer).Control;
                container = fixedControl;
            }

            container.Add(viewRenderer.Container);
        }

        private void SetElement(VisualElement oldElement, VisualElement newElement)
        {
            _element = newElement;
        }

        private void OnRendererElementChanged(object sender, VisualElementChangedEventArgs args)
        {
            if (args.NewElement == _element)
                return;

            SetElement(_element, args.NewElement);
        }
    }
}