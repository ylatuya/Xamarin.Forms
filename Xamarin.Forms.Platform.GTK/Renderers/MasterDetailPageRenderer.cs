using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.GTK.Controls;
using Xamarin.Forms.Platform.GTK.Extensions;
using Container = Gtk.EventBox;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class MasterDetailPageRenderer : Container, IVisualElementRenderer, IEffectControlProvider
    {
        private bool _disposed;
        private MasterDetailPage _masterDetailPage;
        private VisualElementTracker<Page, Container> _tracker;

        IPageController PageController => Element as IPageController;

        IElementController ElementController => Element as IElementController;

        public Controls.MasterDetailPage Control { get; private set; }

        public Container Container => this;

        public VisualElement Element { get; private set; }

        public bool Disposed { get { return _disposed; } }

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        protected MasterDetailPage MasterDetailPage => _masterDetailPage ?? (_masterDetailPage = (MasterDetailPage)Element);

        void IEffectControlProvider.RegisterEffect(Effect effect)
        {
            var platformEffect = effect as PlatformEffect;
            if (platformEffect != null)
                platformEffect.SetContainer(Container);
        }

        protected VisualElementTracker<Page, Container> Tracker
        {
            get { return _tracker; }
            set
            {
                if (_tracker == value)
                    return;

                if (_tracker != null)
                    _tracker.Dispose();

                _tracker = value;
            }
        }

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

            OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

            EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);
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

                if (Control != null)
                {
                    Control.IsPresentedChanged -= OnIsPresentedChanged;
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

                    Control.IsPresentedChanged += OnIsPresentedChanged;

                    UpdateMasterDetail();
                    UpdateMasterBehavior();
                    UpdateIsPresented();
                    UpdateBarTextColor();
                    UpdateBarBackgroundColor();
                }

                e.NewElement.PropertyChanged += HandlePropertyChanged;
            }

            ElementChanged?.Invoke(this, e);
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Master", StringComparison.CurrentCultureIgnoreCase) || e.PropertyName.Equals("Detail", StringComparison.CurrentCultureIgnoreCase))
                UpdateMasterDetail();
            else if (e.PropertyName == MasterDetailPage.IsPresentedProperty.PropertyName)
                UpdateIsPresented();
            else if (e.PropertyName == MasterDetailPage.MasterBehaviorProperty.PropertyName)
                UpdateMasterBehavior();
        }

        private void UpdateMasterDetail()
        {
            if (Platform.GetRenderer(MasterDetailPage.Master) == null)
                Platform.SetRenderer(MasterDetailPage.Master, Platform.CreateRenderer(MasterDetailPage.Master));
            if (Platform.GetRenderer(MasterDetailPage.Detail) == null)
                Platform.SetRenderer(MasterDetailPage.Detail, Platform.CreateRenderer(MasterDetailPage.Detail));

            Control.Master = Platform.GetRenderer(MasterDetailPage.Master).Container;
            Control.Detail = Platform.GetRenderer(MasterDetailPage.Detail).Container;
            Control.MasterTitle = MasterDetailPage.Master.Title;

            UpdateBarTextColor();
            UpdateBarBackgroundColor();
        }

        private void UpdateIsPresented()
        {
            Control.IsPresented = MasterDetailPage.IsPresented;
        }

        private void UpdateMasterBehavior()
        {
            if (MasterDetailPage.Detail is NavigationPage)
            {
                Control.MasterBehaviorType = GetMasterBehavior(MasterDetailPage.MasterBehavior);
            }
            else
            {
                // The only way to display Master page is from a toolbar. If we have not access to one,
                // we should force split mode to display menu (as no gestures are implemented).
                Control.MasterBehaviorType = MasterBehaviorType.Split;
            }

            Control.DisplayTitle = Control.MasterBehaviorType != MasterBehaviorType.Split;
        }

        private void UpdateBarTextColor()
        {
            var barTextColor = Platform.NativeToolbarTracker.Navigation.BarTextColor;

            Control.UpdateBarTextColor(barTextColor.ToGtkColor());
        }

        private void UpdateBarBackgroundColor()
        {
            var barBackgroundColor = Platform.NativeToolbarTracker.Navigation.BarBackgroundColor;

            Control.UpdateBarBackgroundColor(barBackgroundColor.ToGtkColor());
        }

        private MasterBehaviorType GetMasterBehavior(MasterBehavior masterBehavior)
        {
            switch (masterBehavior)
            {
                case MasterBehavior.Split:
                case MasterBehavior.SplitOnLandscape:
                case MasterBehavior.SplitOnPortrait:
                    return MasterBehaviorType.Split;
                case MasterBehavior.Popover:
                    return MasterBehaviorType.Popover;
                case MasterBehavior.Default:
                    return MasterBehaviorType.Default;
                default:
                    throw new ArgumentOutOfRangeException(nameof(masterBehavior));
            }
        }

        private void OnIsPresentedChanged(object sender, EventArgs e)
        {
            ElementController.SetValueFromRenderer(MasterDetailPage.IsPresentedProperty, Control.IsPresented);
        }
    }
}