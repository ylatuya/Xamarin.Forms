using Gtk;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class EditorRenderer : ViewRenderer<Editor, TextView>
    {
        private bool _disposed;

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var textView = new TextView
                {
                    AcceptsTab = true,
                    BorderWidth = 1,
                    WrapMode = WrapMode.WordChar
                };

                SetNativeControl(textView);
            }

            if (e.NewElement == null) return;
            UpdateText();
            UpdateFont();
            UpdateTextColor();
            UpdateEditable();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Editor.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                UpdateEditable();
            else if (e.PropertyName == Editor.TextColorProperty.PropertyName)
                UpdateTextColor();
            else if (e.PropertyName == Editor.FontAttributesProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == Editor.FontFamilyProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == Editor.FontSizeProperty.PropertyName)
                UpdateFont();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }

            base.Dispose(disposing);
        }

        private void UpdateText()
        {
            if (Control.Buffer.Text != Element.Text)
                Control.Buffer.Text = Element.Text ?? string.Empty;
        }

        private void UpdateEditable()
        {
            Control.Editable = Element.IsEnabled;
        }

        private void UpdateFont()
        {
            // TODO:
        }

        private void UpdateTextColor()
        {
            var textColor = Element.TextColor.ToGtkColor();

            Control.ModifyFg(StateType.Normal, textColor);
        }
    }
}