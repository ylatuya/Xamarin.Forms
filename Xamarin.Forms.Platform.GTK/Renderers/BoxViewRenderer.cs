using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class BoxViewRenderer : ViewRenderer<BoxView, Controls.BoxView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Controls.BoxView());
                }

                SetColor(Element.Color);
                SetSize();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == BoxView.ColorProperty.PropertyName)
                SetColor(Element.Color);
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            SetSize();

            base.OnSizeAllocated(allocation);
        }

        protected override void UpdateBackgroundColor()
        {
            base.UpdateBackgroundColor();

            var backgroundColor = Element.BackgroundColor == Color.Default ? Color.Transparent.ToGtkColor() : Element.BackgroundColor.ToGtkColor();

            Control.UpdateBackgroundColor(backgroundColor);

            Container.VisibleWindow = true;
        }

        private void SetColor(Color color)
        {
            if (Element == null || Control == null)
                return;

            var backgroundColor = color != Color.Default ? color : Color.Transparent;

            Control.UpdateColor(backgroundColor.ToGtkColor());
        }

        private void SetSize()
        {
            int height = HeightRequest;
            int width = WidthRequest;

            Control.UpdateSize(height, width);
        }
    }
}