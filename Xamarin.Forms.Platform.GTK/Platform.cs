using Gtk;
using System;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.GTK
{
    public class Platform : BindableObject, IPlatform, IDisposable
    {
        private bool _disposed;
        private readonly PlatformRenderer _renderer;

        internal static readonly BindableProperty RendererProperty =
            BindableProperty.CreateAttached("Renderer", typeof(IVisualElementRenderer), 
                typeof(Platform), default(IVisualElementRenderer),
            propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                var view = bindable as VisualElement;
                if (view != null)
                    view.IsPlatformEnabled = newvalue != null;
            });

        internal PlatformRenderer PlatformRenderer => _renderer;

        internal static NativeToolbarTracker NativeToolbarTracker = new NativeToolbarTracker();

        Page Page { get; set; }

        internal Platform()
        {
            _renderer = new PlatformRenderer(this);

            MessagingCenter.Subscribe(this, Page.AlertSignalName, (Page sender, AlertArguments arguments) =>
            {
                MessageDialog messageDialog = new MessageDialog(
                    PlatformRenderer.Toplevel as Window,
                    DialogFlags.DestroyWithParent,
                    MessageType.Other,
                    ButtonsType.Ok,
                    arguments.Message);

                messageDialog.Title = arguments.Title;

                ResponseType result = (ResponseType)messageDialog.Run();

                if(result == ResponseType.Ok)
                {
                    messageDialog.Destroy();
                    arguments.SetResult(true);
                }

                arguments.SetResult(false);
            });

            MessagingCenter.Subscribe(this, Page.ActionSheetSignalName, (Page sender, ActionSheetArguments arguments) =>
            {
                MessageDialog messageDialog = new MessageDialog(
                   PlatformRenderer.Toplevel as Window,
                   DialogFlags.DestroyWithParent,
                   MessageType.Other,
                   ButtonsType.Ok,
                   arguments.Title);

                ResponseType result = (ResponseType)messageDialog.Run();

                if (result == ResponseType.Ok)
                {
                    messageDialog.Destroy();
                    arguments.SetResult(string.Empty);
                }

                arguments.SetResult(string.Empty);
            });
        }

        internal static void DisposeModelAndChildrenRenderers(Element view)
        {
            IVisualElementRenderer renderer;

            foreach (VisualElement child in view.Descendants())
                DisposeModelAndChildrenRenderers(child);

            renderer = GetRenderer((VisualElement)view);

            renderer?.Dispose();

            view.ClearValue(RendererProperty);
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

            MessagingCenter.Unsubscribe<Page, ActionSheetArguments>(this, Page.ActionSheetSignalName);
            MessagingCenter.Unsubscribe<Page, AlertArguments>(this, Page.AlertSignalName);
            MessagingCenter.Unsubscribe<Page, bool>(this, Page.BusySetSignalName);

            PlatformRenderer.Dispose();
        }

        internal void SetPage(Page newRoot)
        {
            if (newRoot == null)
                return;

            if (Page != null)
                throw new NotImplementedException();

            Page = newRoot;
            Page.Platform = this;
            AddChild(Page);
        }

        private void AddChild(Page mainPage)
        {
            var viewRenderer = GetRenderer(mainPage);

            if (viewRenderer == null)
            {
                viewRenderer = CreateRenderer(mainPage);
                SetRenderer(mainPage, viewRenderer);

                PlatformRenderer.Add(viewRenderer.Container);
                PlatformRenderer.ShowAll();
            }
        }

        internal class DefaultRenderer : VisualElementRenderer<VisualElement, Gtk.Widget>
        {

        }
    }
}