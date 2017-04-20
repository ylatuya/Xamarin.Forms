using Gtk;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class EntryRenderer : ViewRenderer<Entry, Gtk.Entry>
    {
        private bool _disposed;

        IElementController ElementController => Element;

        IEntryController EntryController => Element;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                Gtk.Entry entry = new Gtk.Entry();
                SetNativeControl(entry);

                entry.Changed += OnChanged;
                entry.Focused += OnFocused;
                entry.EditingDone += OnEditingDone;
            }

            if (e.NewElement != null)
            {
                UpdateText();
                UpdateColor();
                UpdateAlignment();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Entry.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
                UpdateColor();
            else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
                UpdateAlignment();
            else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
            {
                UpdateColor();
            }

            base.OnElementPropertyChanged(sender, e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                if (Control != null)
                {
                    Control.Changed -= OnChanged;
                    Control.Focused -= OnFocused;
                    Control.EditingDone -= OnEditingDone;
                }
            }

            base.Dispose(disposing);
        }

        private void UpdateText()
        {
            if (Control.Text != Element.Text)
                Control.Text = Element.Text ?? string.Empty;
        }

        private void UpdateColor()
        {
            var textColor = Element.TextColor;

            Control.ModifyFg(StateType.Normal, textColor.ToGtkColor());
        }

        private void UpdateAlignment()
        {
            var hAlignmentValue = GetAlignmentValue(Element.HorizontalTextAlignment);

            Control.Alignment = hAlignmentValue;
        }

        private void OnChanged(object sender, System.EventArgs e)
        {
            ElementController.SetValueFromRenderer(Entry.TextProperty, Control.Text);
        }

        private void OnFocused(object o, FocusedArgs args)
        {
            ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
        }

        private void OnEditingDone(object sender, System.EventArgs e)
        {
            ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
            EntryController?.SendCompleted();
        }

        private static float GetAlignmentValue(TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Start:
                    return 0f;
                case TextAlignment.End:
                    return 1f;
                default:
                    return 0.5f;
            }
        }
    }
}