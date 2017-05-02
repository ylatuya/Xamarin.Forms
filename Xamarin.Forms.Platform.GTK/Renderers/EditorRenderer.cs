using Gtk;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;
using System;
using Gdk;
using Pango;
using Xamarin.Forms.Platform.GTK.Helpers;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class EditorRenderer : ViewRenderer<Editor, TextView>
    {
        private const string TextColorTagName = "text-color";

        private bool _disposed;

        protected IEditorController EditorController => Element as IEditorController;

        protected override void UpdateBackgroundColor()
        {
            if (!Element.BackgroundColor.IsDefaultOrTransparent())
            {
                var color = Element.BackgroundColor.ToGtkColor();

                Control.ModifyBg(StateType.Normal, color);
                Control.ModifyBase(StateType.Normal, color);
            }
        }
        
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {

            if (Control == null)
            {
                var textView = new TextView
                {
                    AcceptsTab = true,
                    BorderWidth = 1,
                    WrapMode = Gtk.WrapMode.WordChar
                };

                textView.Buffer.TagTable.Add(new TextTag(TextColorTagName));
                textView.Buffer.Changed += TextViewBufferChanged;
                textView.Focused += TextViewFocused;
                textView.FocusOutEvent += TextViewFocusedOut;

                SetNativeControl(textView);
            }

            if (e.NewElement != null)
            {
                UpdateText();
                UpdateFont();
                UpdateTextColor();
                UpdateEditable();
            }

            base.OnElementChanged(e);
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

                if (Control != null)
                {
                    Control.Buffer.Changed -= TextViewBufferChanged;
                    Control.Focused -= TextViewFocused;
                    Control.FocusOutEvent += TextViewFocusedOut;
                }
            }

            base.Dispose(disposing);
        }

        private void UpdateText()
        {
            if (Control.Buffer.Text != Element.Text)
            {
                Control.Buffer.Text = Element.Text ?? string.Empty;
                UpdateTextColor();
            }
        }

        private void UpdateEditable()
        {
            Control.Editable = Element.IsEnabled;
        }

        private void UpdateFont()
        {
            FontDescription fontDescription = FontDescriptionHelper.CreateFontDescription(
                Element.FontSize, Element.FontFamily, Element.FontAttributes);
            Control.ModifyFont(fontDescription);
        }

        private void UpdateTextColor()
        {
            if (!Element.TextColor.IsDefaultOrTransparent())
            {
                var textColor = Element.TextColor.ToGtkColor();

                TextTag tag = Control.Buffer.TagTable.Lookup(TextColorTagName);
                tag.ForegroundGdk = textColor;
                Control.Buffer.ApplyTag(tag, Control.Buffer.StartIter, Control.Buffer.EndIter);
            }
        }

        private void TextViewBufferChanged(object sender, EventArgs e)
        {
            if (Element.Text != Control.Buffer.Text)
                ((IElementController)Element).SetValueFromRenderer(Editor.TextProperty, Control.Buffer.Text);

            UpdateTextColor();
        }

        private void TextViewFocused(object o, FocusedArgs args)
        {
            ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
        }

        private void TextViewFocusedOut(object o, FocusOutEventArgs args)
        {
            ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
            EditorController.SendCompleted();
        }
    }
}