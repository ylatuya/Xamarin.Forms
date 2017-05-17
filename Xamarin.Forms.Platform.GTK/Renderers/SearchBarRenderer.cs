using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Controls;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class SearchBarRenderer : ViewRenderer<SearchBar, SearchEntry>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new SearchEntry());

                    Control.SearchTextChanged += SearchTextChanged;
                    Control.SearchButtonClicked += SearchButtonClicked;
                }

                UpdateText();
                UpdateTextColor();
                UpdateFont();
                UpdateAlignment();
                UpdatePlaceholder();
                UpdateCancelButtonColor();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == SearchBar.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == SearchBar.TextColorProperty.PropertyName)
                UpdateTextColor();
            else if (e.PropertyName == SearchBar.HorizontalTextAlignmentProperty.PropertyName)
                UpdateAlignment();
            else if (e.PropertyName == SearchBar.FontAttributesProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == SearchBar.FontFamilyProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == SearchBar.FontSizeProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == SearchBar.PlaceholderProperty.PropertyName)
                UpdatePlaceholder();
            else if (e.PropertyName == SearchBar.PlaceholderColorProperty.PropertyName)
                UpdatePlaceholder();
            else if (e.PropertyName == SearchBar.CancelButtonColorProperty.PropertyName)
                UpdateCancelButtonColor();
        }

        protected override void UpdateBackgroundColor()
        {
            base.UpdateBackgroundColor();

            if (!Element.BackgroundColor.IsDefaultOrTransparent())
            {
                Control.SetBackgroundColor(Element.BackgroundColor.ToGtkColor());
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Control != null)
                {
                    Control.SearchTextChanged -= SearchTextChanged;
                    Control.SearchButtonClicked -= SearchButtonClicked;
                }
            }
        }

        private void UpdateText()
        {
            Control.SearchText = Element.Text;
        }

        private void UpdatePlaceholder()
        {
            Control.PlaceholderText = Element.Placeholder;
            Control.SetPlaceholderTextColor(Element.PlaceholderColor.ToGtkColor());
        }

        private void UpdateTextColor()
        {
            Control.SetTextColor(Element.TextColor.ToGtkColor());
        }

        private void UpdateCancelButtonColor()
        {
            if (!Element.CancelButtonColor.IsDefaultOrTransparent())
            {
                Control.SetCancelButtonColor(Element.CancelButtonColor.ToGtkColor());
            }
        }

        private void UpdateFont()
        {
            Pango.FontDescription fontDescription = Helpers.FontDescriptionHelper.CreateFontDescription(
                   Element.FontSize, Element.FontFamily, Element.FontAttributes);
            Control.SetFont(fontDescription);
        }

        private void UpdateAlignment()
        {
            Control.SetAlignment(Element.HorizontalTextAlignment.ToNativeValue());
        }

        private void SearchTextChanged(object sender, EventArgs e)
        {
            ElementController.SetValueFromRenderer(SearchBar.TextProperty, Control.SearchText);
        }

        private void SearchButtonClicked(object sender, EventArgs e)
        {
            Element.OnSearchButtonPressed();
        }
    }
}
