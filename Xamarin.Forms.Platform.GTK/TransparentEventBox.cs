using System;
using System.Collections.Generic;
using Cairo;
using Gdk;
using Gtk;

namespace Xamarin.Forms.Platform.GTK
{
	public class TransparentEventBox : Gtk.EventBox
	{
		private Color _backgroundColor;

		public TransparentEventBox()
		{
			VisibleWindow = false;
			BackgroundColor = Color.Transparent;
		}

		Color BackgroundColor
		{
			get => _backgroundColor;
			set
			{
				_backgroundColor = value;
				QueueDraw();
			}
		}

		public void SetBackgroundColor(Color color)
		{
			BackgroundColor = color;
		}

		protected virtual void Draw(Gdk.Rectangle area, Context cr)
		{
		}

		protected override bool OnExposeEvent(EventExpose evnt)
		{
			// And finally forward the expose event to the children
			using (var cr = CairoHelper.Create(GdkWindow))
			{
				var clipBox = evnt.Area;
				if (VisibleWindow)
				{
					// Windowless widgets receive expose events with the whole area of
					// of it's container, so we firt clip it to the allocation of the
					// widget it self
					clipBox = Clip(evnt.Area);
				}
				cr.Rectangle(clipBox.X, clipBox.Y, clipBox.Width, clipBox.Height);
				cr.Clip();

				// Draw first the background with the color defined in BackgroundColor
				if (!BackgroundColor.IsDefault)
				{
					cr.Save();
					cr.SetSourceRGBA(BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, BackgroundColor.A);
					cr.Operator = Operator.Over;
					cr.Paint();
					cr.Restore();
				}

				// Let subclasses perform their own drawing operations
				// We also apply a translation when we don't have a visible window
				if (!VisibleWindow)
				{
					cr.Translate(Allocation.X, Allocation.Y);
				}
				Draw(clipBox, cr);

				base.OnExposeEvent(evnt);

			}
			return false;
		}

		private Gdk.Rectangle Clip(Gdk.Rectangle area)
		{
			int startX = Math.Max(area.X, Allocation.X);
			int endX = Math.Min(area.X + area.Width, Allocation.X + Allocation.Width);
			int startY = Math.Max(area.Y, Allocation.Y);
			int endY = Math.Min(area.Y + area.Height, Allocation.Y + Allocation.Height);

			return new Gdk.Rectangle(startX, startY, endX - startX, endY - startY);
		}
	}
}
