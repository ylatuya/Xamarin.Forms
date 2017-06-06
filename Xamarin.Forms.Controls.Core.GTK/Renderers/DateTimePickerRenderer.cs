using GtkToolkit.Controls;
using GtkToolkit.GTK.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(DateTimePicker), typeof(DateTimePickerRenderer))]
namespace GtkToolkit.GTK.Renderers
{
    public class DateTimePickerRenderer : ViewRenderer<DateTimePicker, Controls.DateTimePicker>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DateTimePicker> e)
        {
            if (Control == null)
            {
                Controls.DateTimePicker dateTimePicker = new Controls.DateTimePicker();
                
                SetNativeControl(dateTimePicker);
            }

            base.OnElementChanged(e);
        }
    }
}