using System;
using Container = Gtk.EventBox;

namespace Xamarin.Forms.Platform.GTK
{
    public interface IVisualElementRenderer : IDisposable, IRegisterable
    {
        event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        VisualElement Element { get; }

        Container Container { get; }

        void SetElement(VisualElement element);

        SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

        void SetElementSize(Size size);

        void UpdateLayout();
    }
}
