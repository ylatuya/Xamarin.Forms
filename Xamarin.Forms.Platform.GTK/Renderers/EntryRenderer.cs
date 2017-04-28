using Gtk;
using Pango;
using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;
using Xamarin.Forms.Platform.GTK.Helpers;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class EntryRenderer : ViewRenderer<Entry, Gtk.Entry>
    {
        private bool _disposed;

        IEntryController EntryController => Element;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
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
                UpdateFont();
                UpdateTextVisibility();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Entry.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
                UpdateColor();
            else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
                UpdateAlignment();
            else if (e.PropertyName == Entry.FontAttributesProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == Entry.FontFamilyProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == Entry.FontSizeProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
                UpdateTextVisibility();

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

        protected override void UpdateBackgroundColor()
        {
            Control.ModifyBase(StateType.Normal, Element.BackgroundColor.ToGtkColor());
        }

        private void UpdateText()
        {
            if (Control.Text != Element.Text)
                Control.Text = Element.Text ?? string.Empty;
        }

        private void UpdateColor()
        {
            var textColor = Element.TextColor;

            Control.ModifyText(StateType.Normal, textColor.ToGtkColor());
        }

        private void UpdateAlignment()
        {
            Control.Alignment = Element.HorizontalTextAlignment.ToNativeValue();
        }

        private void UpdateFont()
        {
            FontDescription fontDescription = FontDescriptionHelper.CreateFontDescription(
                Element.FontSize, Element.FontFamily, Element.FontAttributes);
            Control.ModifyFont(fontDescription);
        }

        private void UpdateTextVisibility()
        {
            Control.Visibility = !Element.IsPassword;
        }

        private void OnChanged(object sender, EventArgs e)
        {
            ElementController.SetValueFromRenderer(Entry.TextProperty, Control.Text);
        }

        private void OnFocused(object o, FocusedArgs args)
        {
            ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
        }

        private void OnEditingDone(object sender, EventArgs e)
        {
            ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
            EntryController?.SendCompleted();
        }
    }
}