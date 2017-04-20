using Gtk;
using System;

namespace Xamarin.Forms.Platform.GTK.Extensions
{
    public static class WidgetExtensions
    {
        public static SizeRequest GetSizeRequest(
            this Widget self,
            double widthConstraint,
            double heightConstraint)
        {
            return new SizeRequest(
                new Size(self.Allocation.Width, self.Allocation.Height), 
                new Size());
        }

        public static void MoveTo(this Widget self, double x, double y)
        {
            if (self.Parent is Fixed)
            {
                var container = self.Parent as Fixed;
                var calcX = (int)Math.Round(x);
                var calcY = (int)Math.Round(y);

                container.Move(self, calcX, calcY);
            }
        }

        public static void PrintTree(this Gtk.Widget widget)
        {
            const char indent = '-';
            int level = CalculateDepthLevel(widget);

            Console.WriteLine(
                string.Format(
                    "({0}) {1} Name: {2} ({3})",
                    level,
                    new String(indent, level * 2),
                    widget.Name,
                    widget.GetType()));

            Console.WriteLine(string.Format("{0} Size: {1}", new String('\t', level), widget.Allocation.Size));
            Console.WriteLine(string.Format("{0} Location: {1}", new String('\t', level), widget.Allocation.Location));

            if (widget is Gtk.Container)
            {
                var container = widget as Container;

                foreach (Gtk.Widget child in container.Children)
                {
                    PrintTree(child);
                }
            }

        }

        private static int CalculateDepthLevel(Widget widget)
        {
            int level = 0;
            Widget current = widget;

            while ((current = current.Parent) != null)
            {
                level++;
            }

            return level;
        }
    }
}