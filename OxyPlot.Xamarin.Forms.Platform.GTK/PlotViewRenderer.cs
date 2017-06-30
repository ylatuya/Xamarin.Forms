using OxyPlot.Xamarin.Forms.Platform.GTK;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(OxyPlot.Xamarin.Forms.PlotView), typeof(PlotViewRenderer))]
namespace OxyPlot.Xamarin.Forms.Platform.GTK
{
    public class PlotViewRenderer : ViewRenderer<PlotView, GtkSharp.PlotView>
    {
        static PlotViewRenderer()
        {
            Init();
        }

        public PlotViewRenderer()
        {
            // Do not delete
        }

        public static void Init()
        {
            PlotView.IsRendererInitialized = true;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<PlotView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            var plotView = new GtkSharp.PlotView
            {
                Model = Element.Model,
                Controller = Element.Controller
            };

            SetNativeControl(plotView);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
            {
                return;
            }

            if (e.PropertyName == PlotView.ModelProperty.PropertyName)
            {
                Control.Model = Element.Model;
            }

            if (e.PropertyName == PlotView.ControllerProperty.PropertyName)
            {
                Control.Controller = Element.Controller;
            }
        }
    }
}