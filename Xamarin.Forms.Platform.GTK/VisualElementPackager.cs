using System;

namespace Xamarin.Forms.Platform.GTK
{
    public class VisualElementPackager : IDisposable
    {
        private readonly IVisualElementRenderer _renderer;

        private VisualElement _element;

        private IElementController ElementController => _renderer.Element as IElementController;

        public VisualElementPackager(IVisualElementRenderer renderer)
        {
            if (renderer == null)
                throw new ArgumentNullException(nameof(renderer));

            _renderer = renderer;
            SetElement(null, _renderer.Element);
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
            if (disposing)
            {
                // TODO:
            }
        }

        protected virtual void OnChildAdded(VisualElement view)
        {
            var viewRenderer = Platform.CreateRenderer(view);
            Platform.SetRenderer(view, viewRenderer);

            Gtk.Container container = _renderer.Container;
            Gtk.Fixed fixedControl = null;

            if (_renderer is Renderers.LayoutRenderer)
            {
                fixedControl = (_renderer as Renderers.LayoutRenderer).Control;
                container = fixedControl;
            }

            container.Add(viewRenderer.Container);
        }

        private void SetElement(VisualElement oldElement, VisualElement newElement)
        {
            _element = newElement;
        }
    }
}