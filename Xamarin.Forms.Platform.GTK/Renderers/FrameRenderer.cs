using Gtk;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class FrameRenderer : ViewRenderer<Frame, Gtk.Frame>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var frame = new Gtk.Frame();
                    SetNativeControl(frame);
                }

                PackChild();
                SetupLayer();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals("Content", System.StringComparison.InvariantCultureIgnoreCase))
            {
                PackChild();
            }
            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
                e.PropertyName == Frame.OutlineColorProperty.PropertyName ||
                e.PropertyName == Frame.HasShadowProperty.PropertyName)
                SetupLayer();
        }

        private void SetupLayer()
        {
            if (Element.BackgroundColor == Color.Default)
                Control.ModifyBg(StateType.Normal, Color.White.ToGtkColor());
            else
                Control.ModifyBg(StateType.Normal, Element.BackgroundColor.ToGtkColor());

            if (Element.HasShadow)
            {
                Control.ShadowType = ShadowType.EtchedIn;
                Control.BorderWidth = 1;
            }
            else
            {
                Control.ShadowType = ShadowType.None;
            }
        }

        private void PackChild()
        {
            if (Element.Content == null)
                return;

            IVisualElementRenderer renderer = Element.Content.GetOrCreateRenderer();
            Control.Child = renderer.Container;
        }
    }
}