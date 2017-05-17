using NativeView = Gtk.Widget;
using System.ComponentModel;

namespace Xamarin.Forms.Platform.GTK
{
    public abstract class ViewRenderer : ViewRenderer<View, NativeView>
    {

    }

    public abstract class ViewRenderer<TView, TNativeView> : VisualElementRenderer<TView, TNativeView> 
        where TView : View where TNativeView : NativeView
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && Control != null)
            {
                Control.Dispose();
                Control = null;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                UpdateBackgroundColor();
            }
        }

        protected override void SetNativeControl(TNativeView view)
        {
            base.SetNativeControl(view);

            Add(view);
        }
    }
}