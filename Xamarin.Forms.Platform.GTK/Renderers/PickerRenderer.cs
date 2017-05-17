using Gtk;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class PickerRenderer : ViewRenderer<Picker, ComboBox>
    {
        private bool _disposed;

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    ComboBox comboBox = new ComboBox();
                    CellRendererText text = new CellRendererText();
                    comboBox.PackStart(text, true);
                    comboBox.AddAttribute(text, "text", 0);

                    comboBox.Changed += OnChanged;

                    SetNativeControl(comboBox);
                }

                UpdateItemsSource();
                UpdateSelectedIndex();
                UpdateTextColor();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Picker.TitleProperty.PropertyName)
                UpdatePicker();
            if (e.PropertyName == Picker.SelectedIndexProperty.PropertyName)
                UpdateSelectedIndex();
            if (e.PropertyName == Picker.ItemsSourceProperty.PropertyName)
                UpdateItemsSource();
            if (e.PropertyName == Picker.TextColorProperty.PropertyName)
                UpdateTextColor();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_disposed)
                {
                    _disposed = true;

                    if (Control != null)
                    {
                        Control.Changed -= OnChanged;
                    }

                }
            }

            base.Dispose(disposing);
        }

        private void UpdatePicker()
        {
            if (Control == null || Element == null)
                return;

            var selectedIndex = Element.SelectedIndex;
            var items = Element.Items;

            if (items == null || items.Count == 0 || selectedIndex < 0)
                return;

            UpdateItemsSource();
            UpdateSelectedIndex();
        }

        private void UpdateItemsSource()
        {
            var items = Element.Items;

            ListStore listStore = new ListStore(typeof(string));
            Control.Model = listStore;

            foreach (var item in items)
            {
                listStore.AppendValues(item);
            }
        }

        private void UpdateSelectedIndex()
        {
            var selectedIndex = Element.SelectedIndex != -1 ? Element.SelectedIndex : 0;

            Control.Active = selectedIndex;
        }

        private void UpdateTextColor()
        {
            if (Control == null || Element == null)
                return;

            var cellView = Control.Child as CellView;

            if (cellView != null)
            {
                var cellRenderer = cellView.Cells.FirstOrDefault() as CellRendererText;

                if (cellRenderer != null)
                {
                    var textColor = Element.TextColor.ToGtkColor();

                    cellRenderer.ForegroundGdk = Element.TextColor.ToGtkColor();
                }
            }
        }

        private void OnChanged(object sender, System.EventArgs e)
        {
            ElementController?.SetValueFromRenderer(Picker.SelectedIndexProperty, Control.Active);
        }
    }
}