using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class ProgressBarRenderer : ViewRenderer<ProgressBar, Gtk.ProgressBar>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
        {
            if (e.NewElement == null)
                return;

            if (Control == null)
            {
                var progressBar = new Gtk.ProgressBar();
                progressBar.Adjustment = new Gtk.Adjustment(0, 0, 1, 0.1, 1, 1);
                SetNativeControl(progressBar);
            }

            UpdateProgress();
            UpdateBackgroundColor();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ProgressBar.ProgressProperty.PropertyName)
                UpdateProgress();
            else if (e.PropertyName == ProgressBar.BackgroundColorProperty.PropertyName)
                UpdateBackgroundColor();
        }

        private void UpdateProgress()
        {
            Control.Adjustment.Value = Element.Progress;
            Control.TooltipText = string.Format("{0}%", (Element.Progress * 100));
        }

        protected override void UpdateBackgroundColor()
        {
            var backgroundColor = Element.BackgroundColor;

            if (backgroundColor == null || backgroundColor.IsDefault)
                return;

            Control.ModifyBg(Gtk.StateType.Normal, backgroundColor.ToGtkColor());

            base.UpdateBackgroundColor();
        }
    }
}