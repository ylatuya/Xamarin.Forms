using Gtk;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Control.Core.Controls;
using Xamarin.Forms.Controls.Core.GTK.Renderers;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(GridSplitter), typeof(GridSplitterRenderer))]
namespace Xamarin.Forms.Controls.Core.GTK.Renderers
{
    public class GridSplitterRenderer : ViewRenderer<GridSplitter, EventBox>
    {
        private bool _disposed;
        private Paned _paned;

        protected override void OnElementChanged(ElementChangedEventArgs<GridSplitter> e)
        {
            if (Control == null)
            {
                _paned = new HPaned();
                _paned.BorderWidth = 6;

                Add(_paned);
                SetNativeControl(this);
            }

            if (e.NewElement != null)
            {
                UpdateContent1();
                UpdateContent2();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GridSplitter.Content1Property.PropertyName)
                UpdateContent1();
            else if (e.PropertyName == GridSplitter.Content2Property.PropertyName)
                UpdateContent2();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }

            base.Dispose(disposing);
        }

        private void UpdateContent1()
        {
            if (_paned != null)
            {
                var content1 = Element.Content1;
                var nativeContent1 = Platform.GTK.Platform.GetRenderer(content1);
                _paned.Add1(nativeContent1.Container);
            }
        }

        private void UpdateContent2()
        {
            if (_paned != null)
            {
                var content2 = Element.Content1;
                var nativeContent2 = Platform.GTK.Platform.GetRenderer(content2);
                _paned.Add2(nativeContent2.Container);
            }
        }
    }
}