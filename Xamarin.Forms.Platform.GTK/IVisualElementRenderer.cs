using System;
using Container = Xamarin.Forms.Platform.GTK.TransparentEventBox;

namespace Xamarin.Forms.Platform.GTK
{
	public interface IVisualElementRenderer : IDisposable, IRegisterable
	{
		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		VisualElement Element { get; }

		Container Container { get; }

		bool Disposed { get; }

		void SetElement(VisualElement element);

		SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

		void SetElementSize(Size size);
	}
}
