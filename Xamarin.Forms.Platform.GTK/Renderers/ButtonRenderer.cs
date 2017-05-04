using Gtk;
using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Controls;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class ButtonRenderer : ViewRenderer<Button, ImageButton>
    {
        private static Gdk.Color DefaultBackgroundColor = new Gdk.Color(0xee, 0xeb, 0xe7);
        private static Gdk.Color DefaultForegroundColor = new Gdk.Color(0x00, 0x00, 0x00);

        protected override void Dispose(bool disposing)
        {
            if (Control != null)
            {
                Control.Clicked -= OnButtonClicked;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var btn = new ImageButton();
                    SetNativeControl(btn);

                    Control.Clicked += OnButtonClicked;
                }

                UpdateBackgroundColor();
                UpdateTextColor();
                UpdateText();
                UpdateBorder();
                UpdateContent();
                UpdateSensitive();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Button.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Button.FontProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                UpdateBackgroundColor();
            else if (e.PropertyName == Button.TextColorProperty.PropertyName)
                UpdateTextColor();
            else if (e.PropertyName == Button.BorderColorProperty.PropertyName)
                UpdateBorder();
            else if (e.PropertyName == Button.BorderWidthProperty.PropertyName)
                UpdateBorder();
            else if (e.PropertyName == Button.ImageProperty.PropertyName || e.PropertyName == Button.ContentLayoutProperty.PropertyName)
                UpdateContent();
            else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                UpdateSensitive();
        }

        protected override void UpdateBackgroundColor()
        {
            if (Element.BackgroundColor.IsDefault)
            {
                Control.SetBackgroundColor(DefaultBackgroundColor);
            }
            else if (Element.BackgroundColor != Color.Transparent)
            {
                Control.SetBackgroundColor(Element.BackgroundColor.ToGtkColor());
            }

            Container.VisibleWindow = Element.BackgroundColor != Color.Transparent;
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            ((IButtonController)Element)?.SendClicked();
        }

        private void UpdateText()
        {
            var span = new Span()
            {
                FontAttributes = Element.FontAttributes,
                FontFamily = Element.FontFamily,
                FontSize = Element.FontSize,
                Text = Element.Text
            };

            Control.LabelWidget.SetTextFromSpan(span);
        }

        private void UpdateTextColor()
        {
            if (Element.TextColor.IsDefaultOrTransparent())
            {
                Control.SetForegroundColor(DefaultForegroundColor);
            }
            else
            {
                Control.SetForegroundColor(Element.TextColor.ToGtkColor());
            }
        }

        private void UpdateBorder()
        {
            var borderWidth = Element.BorderWidth >= 0
                ? (uint)Element.BorderWidth
                : 0;

            Control.SetBorderWidth(borderWidth);

            if (borderWidth == 0 || Element.BorderColor.IsDefaultOrTransparent())
            {
                if (!Element.BackgroundColor.IsDefault)
                {
                    Container.ModifyBg(StateType.Normal, Element.BackgroundColor.ToGtkColor());
                }
            }
            else
            {
                Container.ModifyBg(StateType.Normal, Element.BorderColor.ToGtkColor());
            }
        }

        private void UpdateContent()
        {
            if (!string.IsNullOrEmpty(Element.Image))
            {
                Control.SetImageFromFile(Element.Image);
                Control.ImageSpacing = (uint)Element.ContentLayout.Spacing;
                Control.SetImagePosition(Element.ContentLayout.Position.AsPositionType());
                Control.ImageWidget.Visible = true;
            }
            else
            {
                Control.ImageWidget.Visible = false;
            }
        }

        private void UpdateSensitive()
        {
            Control.Sensitive = Element.IsEnabled;
        }
    }
}