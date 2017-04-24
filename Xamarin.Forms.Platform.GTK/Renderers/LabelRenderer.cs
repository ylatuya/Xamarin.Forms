using Gtk;
using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;
using NativeLabel = Gtk.Label;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class LabelRenderer : ViewRenderer<Label, NativeLabel>
    {
        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            Control.SetSizeRequest(-1, -1); // force widget to calculate its desired size

            return base.GetDesiredSize(widthConstraint, heightConstraint);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new NativeLabel());
                }

                UpdateText();
                UpdateColor();
                UpdateLineBreakMode();
                UpdateTextAlignment();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Label.TextColorProperty.PropertyName)
                UpdateColor();
            else if (e.PropertyName == Label.FontProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Label.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Label.FontAttributesProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Label.FormattedTextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName || e.PropertyName == Label.VerticalTextAlignmentProperty.PropertyName)
                UpdateTextAlignment();
            else if (e.PropertyName == Label.LineBreakModeProperty.PropertyName)
                UpdateLineBreakMode();
        }

        private void UpdateText()
        {
            string markupText = string.Empty;
            FormattedString formatted = Element.FormattedText;

            if (formatted != null)
            {
                Control.SetTextFromFormatted(formatted);
            }
            else
            {
                var span = new Span()
                {
                    FontAttributes = Element.FontAttributes,
                    FontFamily = Element.FontFamily,
                    FontSize = Element.FontSize,
                    Text = Element.Text
                };

                Control.SetTextFromSpan(span);
            }
        }

        private void UpdateColor()
        {
            if (Control == null)
                return;

            var textColor = Element.TextColor != Color.Default ? Element.TextColor : Color.Black;

            Control.ModifyFg(StateType.Normal, textColor.ToGtkColor());
        }

        private void UpdateTextAlignment()
        {
            var hAlignmentValue = GetAlignmentValue(Element.HorizontalTextAlignment);
            var vAlignmentValue = GetAlignmentValue(Element.VerticalTextAlignment);

            Control.SetAlignment(hAlignmentValue, vAlignmentValue);
        }

        private void UpdateLineBreakMode()
        {
            switch (Element.LineBreakMode)
            {
                case LineBreakMode.NoWrap:
                    Control.SingleLineMode = true;
                    Control.LineWrap = false;
                    Control.Ellipsize = Pango.EllipsizeMode.None;
                    break;
                case LineBreakMode.WordWrap:
                    Control.LineWrap = true;
                    Control.SingleLineMode = false;
                    Control.LineWrapMode = Pango.WrapMode.Word;
                    Control.Ellipsize = Pango.EllipsizeMode.None;
                    break;
                case LineBreakMode.CharacterWrap:
                    Control.LineWrap = true;
                    Control.SingleLineMode = false;
                    Control.LineWrapMode = Pango.WrapMode.Char;
                    Control.Ellipsize = Pango.EllipsizeMode.None;
                    break;
                case LineBreakMode.HeadTruncation:
                    Control.LineWrap = false;
                    Control.SingleLineMode = true;
                    Control.LineWrapMode = Pango.WrapMode.Word;
                    Control.Ellipsize = Pango.EllipsizeMode.Start;
                    break;
                case LineBreakMode.TailTruncation:
                    Control.LineWrap = false;
                    Control.SingleLineMode = true;
                    Control.LineWrapMode = Pango.WrapMode.Word;
                    Control.Ellipsize = Pango.EllipsizeMode.End;
                    break;
                case LineBreakMode.MiddleTruncation:
                    Control.LineWrap = false;
                    Control.SingleLineMode = true;
                    Control.LineWrapMode = Pango.WrapMode.Word;
                    Control.Ellipsize = Pango.EllipsizeMode.Middle;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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