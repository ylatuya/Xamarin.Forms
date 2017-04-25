using System.ComponentModel;

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
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ProgressBar.ProgressProperty.PropertyName)
                UpdateProgress();
        }

        private void UpdateProgress()
        {
            Control.Adjustment.Value = Element.Progress;
            Control.TooltipText = string.Format("{0}%", (Element.Progress * 100));
        }
    }
}