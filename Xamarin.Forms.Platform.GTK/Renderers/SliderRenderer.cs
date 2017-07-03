using System;
using System.ComponentModel;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class SliderRenderer : ViewRenderer<Slider, Gtk.HScale>
    {
        private double _minimum;
        private double _maximum;
        private bool _disposed;

        protected override bool PreventGestureBubbling { get; set; } = true;

        protected override void Dispose(bool disposing)
        {
            if (Control != null)
                Control.ValueChanged -= OnControlValueChanged;

            if (disposing && !_disposed)
            {
                _disposed = true;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    _minimum = 0;
                    _maximum = 100;
                    SetNativeControl(new Gtk.HScale(_minimum, _maximum, 0.1));
                    Control.ValueChanged += OnControlValueChanged;
                }

                UpdateMaximum();
                UpdateMinimum();
                UpdateValue();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Slider.MaximumProperty.PropertyName)
                UpdateMaximum();
            else if (e.PropertyName == Slider.MinimumProperty.PropertyName)
                UpdateMinimum();
            else if (e.PropertyName == Slider.ValueProperty.PropertyName)
                UpdateValue();
        }

        private void OnControlValueChanged(object sender, EventArgs eventArgs)
        {
            ElementController.SetValueFromRenderer(Slider.ValueProperty, Control.Value);
        }

        private void UpdateMaximum()
        {
            _maximum = (float)Element.Maximum;

            Control.SetRange(_minimum, _maximum);
        }

        private void UpdateMinimum()
        {
            _minimum = (float)Element.Minimum;

            Control.SetRange(_minimum, _maximum);
        }

        private void UpdateValue()
        {
            Control.Value = (float)Element.Value;
        }
    }
}