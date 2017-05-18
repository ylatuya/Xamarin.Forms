using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class TimePickerRenderer : ViewRenderer<TimePicker, Controls.TimePicker>
    {
        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (Control != null)
                    Control.TimeChanged -= OnTimeChanged;

                _disposed = true;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var timePicker = new Controls.TimePicker();
                    timePicker.TimeChanged += OnTimeChanged;
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

            if (e.PropertyName == TimePicker.TimeProperty.PropertyName ||
                e.PropertyName == TimePicker.FormatProperty.PropertyName)
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
            Control.TextColor = textColor;
        }

        private void OnTimeChanged(object sender, EventArgs e)
        {
            var currentTime = (DateTime.Today + Control.CurrentTime);

            ElementController?.SetValueFromRenderer(TimePicker.TimeProperty, currentTime);
        }
    }
}