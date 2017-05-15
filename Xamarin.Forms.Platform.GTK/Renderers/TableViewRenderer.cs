using System.ComponentModel;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class TableViewRenderer : ViewRenderer<TableView, Controls.TableView>
    {
        private const int DefaultRowHeight = 44;

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
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                UpdateBackgroundView();
        }

        private void SetSource()
        {

        }

        private void UpdateRowHeight()
        {

        }

        private void UpdateBackgroundView()
        {

        }
    }
}
