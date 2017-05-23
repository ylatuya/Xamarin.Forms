using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class TableViewRenderer : ViewRenderer<TableView, Controls.TableView>
    {
        private const int DefaultRowHeight = 44;

        protected override void UpdateBackgroundColor()
        {
            base.UpdateBackgroundColor();

            UpdateBackgroundView();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var tableView = new Controls.TableView();
                    SetNativeControl(tableView);
                }

                SetSource();
                UpdateRowHeight();
                UpdateBackgroundView();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == TableView.RowHeightProperty.PropertyName)
                UpdateRowHeight();
            else if (e.PropertyName == TableView.HasUnevenRowsProperty.PropertyName)
                SetSource();
        }

        private void SetSource()
        {
            Control.Root = Element.Root;
        }

        private void UpdateRowHeight()
        {
            var rowHeight = Element.RowHeight;

            Control.SetRowHeight(rowHeight > 0 ? rowHeight : DefaultRowHeight);
        }

        private void UpdateBackgroundView()
        {
            if (Element.BackgroundColor.IsDefault)
            {
                return;
            }

            var backgroundColor = Element.BackgroundColor.ToGtkColor();
            Control.SetBackgroundColor(backgroundColor);
        }
    }
}