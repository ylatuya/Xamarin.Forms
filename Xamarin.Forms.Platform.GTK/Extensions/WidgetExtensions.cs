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
            var desiredSize = self.SizeRequest();

            var widthFits = widthConstraint >= desiredSize.Width;
            var heightFits = heightConstraint >= desiredSize.Height;

            if (widthFits && heightFits) // Enough space with given constraints
            {
                return new SizeRequest(new Size(desiredSize.Width, desiredSize.Height));
            }

            if (!widthFits)
            {
                self.SetSizeRequest((int)widthConstraint, -1);

                desiredSize = self.SizeRequest();
                heightFits = heightConstraint >= desiredSize.Height;
            }

            var size = new Size(desiredSize.Width, heightFits ? desiredSize.Height : (int)heightConstraint);

            return new SizeRequest(size);
        }

        public static void MoveTo(this Widget self, double x, double y)
        {
            if (self.Parent is Fixed)
            {
                var container = self.Parent as Fixed;
                var calcX = (int)Math.Round(x);
                var calcY = (int)Math.Round(y);

                var containerChild = container[self] as Fixed.FixedChild;

                if (containerChild.X != calcX || containerChild.Y != calcY)
                {
                    container.Move(self, calcX, calcY);
                }
            }
        }

        public static void SetSize(this Widget self, double width, double height)
        {
            int calcWidth = (int)Math.Round(width);
            int calcHeight = (int)Math.Round(height);

            if (calcWidth != self.WidthRequest || calcHeight != self.HeightRequest)
            {
                self.SetSizeRequest(calcWidth, calcHeight);
            }
        }

        public static void Remove(this Widget self, Widget child)
        {
            var container = self as Container;

            if (container != null)
            {
                container.Remove(child);
            }
        }

        public static void PrintTree(this Widget widget)
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

            if (widget is Container)
            {
                var container = widget as Container;

                foreach (Widget child in container.Children)
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