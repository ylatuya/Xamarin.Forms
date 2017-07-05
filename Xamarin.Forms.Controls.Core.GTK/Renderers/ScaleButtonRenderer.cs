using System.ComponentModel;
using GtkToolkit.Controls;
using GtkToolkit.GTK.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(ScaleButton), typeof(ScaleButtonRenderer))]
namespace GtkToolkit.GTK.Renderers
{
    public class ScaleButtonRenderer : ViewRenderer<ScaleButton, Gtk.ScaleButton>
    {
        private bool _disposed;
        private Gtk.ScaleButton _scaleButton;
        private double _minimum;
        private double _maximum;
        private double _step;

        protected override void OnElementChanged(ElementChangedEventArgs<ScaleButton> e)
        {
            if (Control == null)
            {
                _minimum = Element.Minimum;
                _maximum = Element.Maximum;
                _step = Element.StepIncrement;

                _scaleButton = new Gtk.ScaleButton(
                    Gtk.IconSize.Button,
                    _minimum,
                    _maximum,
                    _step,
                    null);

                Add(_scaleButton);

                _scaleButton.ShowAll();

                SetNativeControl(_scaleButton);
            }

            if (e.NewElement != null)
            {
                UpdateMinimum();
                UpdateMaximum();
                UpdateStepIncrement();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ScaleButton.MaximumProperty.PropertyName)
                UpdateMaximum();
            else if (e.PropertyName == ScaleButton.MinimumProperty.PropertyName)
                UpdateMinimum();
            else if (e.PropertyName == ScaleButton.StepIncrementProperty.PropertyName)
                UpdateStepIncrement();

            base.OnElementPropertyChanged(sender, e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }

            base.Dispose(disposing);
        }

        private void UpdateMaximum()
        {
            _maximum = Element.Maximum;

            Control.Adjustment.Lower = _minimum;
        }

        private void UpdateMinimum()
        {
            _minimum = Element.Minimum;

            Control.Adjustment.Upper = _maximum;
        }

        private void UpdateStepIncrement()
        {
            _step = Element.StepIncrement;

            Control.Adjustment.StepIncrement = _step;
        }
    }
}