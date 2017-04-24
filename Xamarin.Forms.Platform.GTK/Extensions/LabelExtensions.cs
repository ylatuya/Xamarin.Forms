using Pango;
using System.Text;

namespace Xamarin.Forms.Platform.GTK.Extensions
{
    internal static class LabelExtensions
    {
        internal static void SetTextFromFormatted(this Gtk.Label self, FormattedString formatted)
        {
            string markupText = GenerateMarkupText(formatted);
            self.Markup = markupText;
        }

        internal static void SetTextFromSpan(this Gtk.Label self, Span span)
        {
            string markupText = GenerateMarkupText(span);
            self.Markup = markupText;
        }

        private static string GenerateMarkupText(FormattedString formatted)
        {
            StringBuilder builder = new StringBuilder();

            foreach (Span span in formatted.Spans)
            {
                builder.Append(GenerateMarkupText(span));
            }

            return builder.ToString();
        }

        private static string GenerateMarkupText(Span span)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("<span ");

            FontDescription fontDescription = new FontDescription();
            fontDescription.Size = (int)(span.FontSize * Pango.Scale.PangoScale);
            fontDescription.Family = span.FontFamily;
            fontDescription.Weight = span.FontAttributes == FontAttributes.Bold ? Weight.Bold : Weight.Normal;
            fontDescription.Style = span.FontAttributes == FontAttributes.Italic ? Pango.Style.Italic : Pango.Style.Normal;
            fontDescription.Family = span.FontFamily;

            builder.AppendFormat(" font=\"{0}\"", fontDescription.ToString());

            // BackgroundColor => 
            if (!span.BackgroundColor.IsDefault)
            {
                builder.AppendFormat(" bgcolor=\"{0}\"", span.BackgroundColor.ToRgbaColor());
            }

            // ForegroundColor => 
            if (!span.ForegroundColor.IsDefault)
            {
                builder.AppendFormat(" fgcolor=\"{0}\"", span.ForegroundColor.ToRgbaColor());
            }

            builder.Append(">"); // Complete opening span tag
            // Text
            builder.Append(span.Text);
            builder.Append("</span>");

            return builder.ToString();
        }
    }
}
