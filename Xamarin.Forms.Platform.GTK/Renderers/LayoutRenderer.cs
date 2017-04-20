namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class LayoutRenderer : ViewRenderer<Layout, Gtk.Fixed>
    {
        private VisualElementPackager _packager;

        protected override void OnElementChanged(ElementChangedEventArgs<Layout> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Gtk.Fixed());
                }

                _packager = new VisualElementPackager(this);
                _packager.Load();
            }

            base.OnElementChanged(e);
        }

        public override void UpdateLayout()
        {
            base.UpdateLayout();

            for (var i = 0; i < ElementController.LogicalChildren.Count; i++)
            {
                var child = ElementController.LogicalChildren[i] as VisualElement;

                if (child == null)
                    continue;

                IVisualElementRenderer renderer = Platform.GetRenderer(child);

                if (renderer == null)
                    continue;

                renderer.UpdateLayout();
            }
        }
    }
}
