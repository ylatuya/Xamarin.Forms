using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class SwitchRenderer : ViewRenderer<Switch, Gtk.CheckButton>
    {
        private bool _disposed;

        protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
        {
            if (e.OldElement != null)
                e.OldElement.Toggled -= OnElementToggled;

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Gtk.CheckButton());
                }

                UpdateState();
                UpdateBackgroundColor();
                e.NewElement.Toggled += OnElementToggled;
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Switch.IsToggledProperty.PropertyName)
            {
                Control.Active = Element.IsToggled;
            }
            else if (e.PropertyName == Switch.BackgroundColorProperty.PropertyName)
            {
                UpdateBackgroundColor();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }

            base.Dispose(disposing);
        }

        protected override void UpdateBackgroundColor()
        {
            Color backgroundColor = Element.BackgroundColor;

            Control.ModifyBg(Gtk.StateType.Normal, backgroundColor.ToGtkColor());

            base.UpdateBackgroundColor();
        }

        private void OnElementToggled(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void UpdateState()
        {
            Control.Active = Element.IsToggled ? true : false;
        }
    }
}