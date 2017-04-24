using System;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.GTK
{
    public class Platform : BindableObject, IPlatform, IDisposable
    {
        internal static readonly BindableProperty RendererProperty =
            BindableProperty.CreateAttached("Renderer", typeof(IVisualElementRenderer), typeof(Platform), default(IVisualElementRenderer),
            propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                var view = bindable as VisualElement;
                if (view != null)
                    view.IsPlatformEnabled = newvalue != null;
            });

        private bool _disposed;
        private readonly PlatformRenderer _renderer;

        internal PlatformRenderer Renderer => _renderer;

        internal Platform()
        {
            _renderer = new PlatformRenderer(this);
        }

        SizeRequest IPlatform.GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
        {
            var renderView = GetRenderer(view);

            if (renderView == null || renderView.Container == null)
                return new SizeRequest(Size.Zero);

            return renderView.GetDesiredSize(widthConstraint, heightConstraint);
        }

        public static IVisualElementRenderer GetRenderer(VisualElement element)
        {
            return (IVisualElementRenderer)element.GetValue(RendererProperty);
        }

        public static void SetRenderer(VisualElement element, IVisualElementRenderer value)
        {
            element.SetValue(RendererProperty, value);
        }

        public static IVisualElementRenderer CreateRenderer(VisualElement element)
        {
            var t = element.GetType();
            var renderer = Registrar.Registered.GetHandler<IVisualElementRenderer>(t) ?? new DefaultRenderer();
            renderer.SetElement(element);
            return renderer;
        }

        void IDisposable.Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            Renderer.Dispose();
        }

        internal void SetPage(Page mainPage)
        {
            mainPage.Platform = this;
            AddChild(mainPage);
        }

        private void AddChild(Page mainPage)
        {
            var viewRenderer = GetRenderer(mainPage);

            if (viewRenderer == null)
            {
                viewRenderer = CreateRenderer(mainPage);
                SetRenderer(mainPage, viewRenderer);
                Renderer.Add(viewRenderer.Container);

                _renderer.ShowAll();
            }
        }

        internal class DefaultRenderer : VisualElementRenderer<VisualElement, Gtk.Widget>
        {

        }
    }
}