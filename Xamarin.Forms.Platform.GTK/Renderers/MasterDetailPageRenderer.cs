using System;
using System.ComponentModel;
using Container = Gtk.EventBox;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class MasterDetailPageRenderer : Container, IVisualElementRenderer
    {
        private bool _disposed;
        private MasterDetailPage _masterDetailPage;

        IPageController PageController => Element as IPageController;

        public Controls.MasterDetailPage Control { get; private set; }

        public Container Container => this;

        public VisualElement Element { get; private set; }

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        protected MasterDetailPage MasterDetailPage => _masterDetailPage ?? (_masterDetailPage = (MasterDetailPage)Element);

        public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            var result = new Size(
                 widthConstraint,
                heightConstraint);

            return new SizeRequest(result);
        }

        public void SetElement(VisualElement element)
        {
            var oldElement = Element;

            Element = element;

            UpdateMasterDetail();

            OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));
        }

        public void SetElementSize(Size size)
        {
            Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                PageController?.SendDisappearing();

                if (Element != null)
                {
                    Element.PropertyChanged -= HandlePropertyChanged;
                    Element = null;
                }

                _disposed = true;
            }

            base.Dispose();
        }

        protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= HandlePropertyChanged;

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    Control = new Controls.MasterDetailPage();
                    Add(Control);

                    UpdateMasterDetail();
                    UpdateIsPresented();
                }

                e.NewElement.PropertyChanged += HandlePropertyChanged;
            }

            ElementChanged?.Invoke(this, e);
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Master", StringComparison.CurrentCultureIgnoreCase) || e.PropertyName.Equals("Detail", StringComparison.CurrentCultureIgnoreCase))
                UpdateMasterDetail();
            else if (e.PropertyName == MasterDetailPage.IsPresentedProperty.PropertyName || e.PropertyName == MasterDetailPage.MasterBehaviorProperty.PropertyName)
                UpdateIsPresented();
        }

        private void UpdateMasterDetail()
        {
            if (Platform.GetRenderer(MasterDetailPage.Master) == null)
                Platform.SetRenderer(MasterDetailPage.Master, Platform.CreateRenderer(MasterDetailPage.Master));
            if (Platform.GetRenderer(MasterDetailPage.Detail) == null)
                Platform.SetRenderer(MasterDetailPage.Detail, Platform.CreateRenderer(MasterDetailPage.Detail));

            if (Control != null)
            {
                Control.Master = Platform.GetRenderer(MasterDetailPage.Master).Container;
                Control.Detail = Platform.GetRenderer(MasterDetailPage.Detail).Container;
            }
        }

        private void UpdateIsPresented()
        {
            if (Control != null)
            {
                Control.IsPresented = MasterDetailPage.IsPresented;
            }
        }
    }
}
