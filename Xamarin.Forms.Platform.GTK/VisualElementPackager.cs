using System;
using Xamarin.Forms.Platform.GTK.Extensions;

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
            Controls.Page page = null;

            if (Renderer is Renderers.NavigationPageRenderer)
            {
                return;
            }

            if (Renderer is Renderers.LayoutRenderer)
            {
                var fixedControl = (Renderer as Renderers.LayoutRenderer).Control;
                container = fixedControl;
                container.Add(viewRenderer.Container);
            }
            else if (Renderer is Renderers.PageRenderer)
            {
                page = (Renderer as Renderers.PageRenderer).Control;

                page.Content = viewRenderer.Container;
            }
            else
                container.Add(viewRenderer.Container);
           
            viewRenderer.Container.ShowAll();
        }

        protected virtual void OnChildRemoved(VisualElement view)
        {
            var viewRenderer = Platform.GetRenderer(view);

            if (viewRenderer != null)
            {
                var parent = viewRenderer.Container.Parent;
                parent.RemoveFromContainer(viewRenderer.Container);
            }
        }

        private void SetElement(VisualElement oldElement, VisualElement newElement)
        {
            if (oldElement != null)
            {
                oldElement.ChildAdded -= OnChildAdded;
                oldElement.ChildRemoved -= OnChildRemoved;
                oldElement.ChildrenReordered -= OnChildReordered;
            }

            _element = newElement;

            if (newElement != null)
            {
                newElement.ChildAdded += OnChildAdded;
                newElement.ChildRemoved += OnChildRemoved;
                newElement.ChildrenReordered += OnChildReordered;
            }
        }

        private void OnRendererElementChanged(object sender, VisualElementChangedEventArgs args)
        {
            if (args.NewElement == _element)
                return;

            SetElement(_element, args.NewElement);
        }

        private void OnChildAdded(object sender, ElementEventArgs e)
        {
            var view = e.Element as VisualElement;
            if (view != null)
                OnChildAdded(view);
        }

        private void OnChildRemoved(object sender, ElementEventArgs e)
        {
            var view = e.Element as VisualElement;
            if (view != null)
                OnChildRemoved(view);
        }
        
        private void OnChildReordered(object sender, EventArgs e)
        {
            // TODO
        }
    }
}