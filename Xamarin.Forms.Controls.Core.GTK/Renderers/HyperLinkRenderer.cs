using System.ComponentModel;
using GtkToolkit.Controls;
using GtkToolkit.GTK.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(HyperLink), typeof(HyperLinkRenderer))]
namespace GtkToolkit.GTK.Renderers
{
    public class HyperLinkRenderer : ViewRenderer<HyperLink, Gtk.LinkButton>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<HyperLink> e)
        {
            if (Control == null)
            {
                Gtk.LinkButton linkButton = new Gtk.LinkButton(string.Empty);
                linkButton.BorderWidth = 0;
        
                SetNativeControl(linkButton);
            }

            if (e.NewElement != null)
            {
                UpdateText();
                UpdateUri();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == HyperLink.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == HyperLink.UriProperty.PropertyName)
                UpdateUri();

            base.OnElementPropertyChanged(sender, e);
        }

        private void UpdateText()
        {
            Control.Label = Element.Text;
        }

        private void UpdateUri()
        {
            Control.Uri = Element.Uri;
        }
    }
}