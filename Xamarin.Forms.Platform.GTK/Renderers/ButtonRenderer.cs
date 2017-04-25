using Gtk;
using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class ButtonRenderer : ViewRenderer<Button, ButtonRenderer.GtkButtonWrapper>
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
                    var btn = new GtkButtonWrapper();
                    SetNativeControl(btn);

                    Control.Clicked += OnButtonClicked;
                }

                UpdateBackgroundColor();
                UpdateTextColor();
                UpdateText();
                UpdateBorder();
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

        public sealed class GtkButtonWrapper : Gtk.Button
        {
            private EventBox _labelContainer;
            private Gtk.Label _label;

            public GtkButtonWrapper()
            {
                _labelContainer = new EventBox();
                _label = new Gtk.Label();
                _labelContainer.Child = _label;

                Add(_labelContainer);
                Relief = ReliefStyle.None;
                CanFocus = false;
            }

            public Gtk.Label LabelWidget => _label;

            public void SetBackgroundColor(Gdk.Color color)
            {
                _labelContainer.ModifyBg(StateType.Normal, color);
                _labelContainer.ModifyBg(StateType.Selected, color);
                _labelContainer.ModifyBg(StateType.Prelight, color);
                _labelContainer.ModifyBg(StateType.Active, color);
                _labelContainer.ModifyBg(StateType.Insensitive, color);
            }

            public void SetForegroundColor(Gdk.Color color)
            {
                _label.ModifyFg(StateType.Normal, color);
                _label.ModifyFg(StateType.Prelight, color);
                _label.ModifyFg(StateType.Active, color);
            }

            public void SetBorderWidth(uint width)
            {
                _labelContainer.BorderWidth = width;
            }

            public override void Dispose()
            {
                base.Dispose();

                _labelContainer = null;
                _label = null;
            }
        }
    }
}