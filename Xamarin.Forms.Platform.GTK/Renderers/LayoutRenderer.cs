using Gtk;
using Xamarin.Forms.Platform.GTK.Extensions;
using Xamarin.Forms.Platform.GTK.Packagers;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class LayoutRenderer : ViewRenderer<Layout, Fixed>
    {
        private Fixed _fixed;
        private LayoutElementPackager _packager;
        private Gdk.Rectangle _lastAllocation;

        protected override void OnElementChanged(ElementChangedEventArgs<Layout> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    if(_fixed == null)
                    {
                        _fixed = new Fixed();
                    }

                    SetNativeControl(_fixed);
                }

                _packager = new LayoutElementPackager(this);
                _packager.Load();
            }

            base.OnElementChanged(e);
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);

            if (IsAnimationRunning(Element))
            {
                return;
            }

            if (_lastAllocation != allocation)
            {
                _lastAllocation = allocation;

                Rectangle bounds = Element.Bounds;
                Container.MoveTo((int)bounds.X, (int)bounds.Y);

                for (var i = 0; i < ElementController.LogicalChildren.Count; i++)
                {
                    var child = ElementController.LogicalChildren[i] as VisualElement;

                    if (child != null)
                    {
                        var renderer = Platform.GetRenderer(child);
                        renderer?.Container.SetSize(child.Bounds.Width, child.Bounds.Height);

                        if (!IsAnimationRunning(renderer.Element))
                        {
                            renderer?.Container.MoveTo(child.Bounds.X, child.Bounds.Y);
                        }
                    }
                }
            }
        }
    }
}