using System.ComponentModel;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class TableViewRenderer : ViewRenderer<TableView, Controls.TableView>
    {
        private const int DefaultRowHeight = 44;

        private Controls.TableView _tableView;

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
                    _tableView = new Controls.TableView();
                    SetNativeControl(_tableView);
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
            if (_tableView != null)
            {
                var model = Element.Root;
                _tableView.Source = model;
            }
        }

        private void UpdateRowHeight()
        {

        }

        private void UpdateBackgroundView()
        {

        }
    }
}
