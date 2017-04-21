using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class TimePickerRenderer : ViewRenderer<TimePicker, Controls.TimePicker>
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var timePicker = new Controls.TimePicker();
                    SetNativeControl(timePicker);
                }

                UpdateTime();
                UpdateTextColor();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
                UpdateTime();
            if (e.PropertyName == TimePicker.TextColorProperty.PropertyName)
                UpdateTextColor();
        }

        private void UpdateTime()
        {
            if (Control == null || Element == null)
                return;

            if (Element.Time.Equals(default(TimeSpan)))
                Control.CurrentTime = DateTime.Now.TimeOfDay;
            else
                Control.CurrentTime = Element.Time;
        }

        private void UpdateTextColor()
        {
            var textColor = Element.TextColor.ToGtkColor();
            Control.Color = textColor;
        }
    }
}