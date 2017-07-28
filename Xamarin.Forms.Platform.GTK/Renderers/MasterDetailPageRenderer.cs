using Gtk;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.GTK.Controls;
using Xamarin.Forms.Platform.GTK.Extensions;
using Container = Gtk.EventBox;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class MasterDetailPageRenderer : AbstractPageRenderer<Controls.MasterDetailPage, MasterDetailPage>
    {
        private VisualElementTracker<Page, Container> _tracker;

        public MasterDetailPageRenderer()
        {
            MessagingCenter.Subscribe(this, Forms.BarTextColor, (NavigationPage sender, Color color) =>
            {
                var barTextColor = color;

                if (barTextColor.IsDefaultOrTransparent())
                {
                    Widget.UpdateBarTextColor(null);
                }
                else
                {
                    Widget.UpdateBarTextColor(color.ToGtkColor());
                }
            });

            MessagingCenter.Subscribe(this, Forms.BarBackgroundColor, (NavigationPage sender, Color color) =>
            {
                var barBackgroundColor = color;

                if (barBackgroundColor.IsDefaultOrTransparent())
                {
                    Widget.UpdateBarBackgroundColor(null);
                }
                else
                {
                    Widget.UpdateBarBackgroundColor(color.ToGtkColor());
                }
            });
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

        public override void SetElement(VisualElement element)
        {
            var oldElement = Element;

            Element = element;

            OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

            EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);
        }

        protected override void Dispose(bool disposing)
        {
            if (Widget != null)
            {
                Widget.IsPresentedChanged -= OnIsPresentedChanged;
            }

            MessagingCenter.Unsubscribe<NavigationPage, Color>(this, Forms.BarTextColor);
            MessagingCenter.Unsubscribe<NavigationPage, Color>(this, Forms.BarBackgroundColor);

            _tracker?.Dispose();
            _tracker = null;

            if (Page?.Master != null)
            {
                Page.Master.PropertyChanged -= HandleMasterPropertyChanged;
            }

            base.Dispose();
        }

        protected override async void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    Control = new Controls.Page();
                    Add(Control);
                }

                if (Widget == null)
                {
                    Widget = new Controls.MasterDetailPage();
                    var eventBox = new EventBox();
                    eventBox.Add(Widget);

                    Control.Content = eventBox;

                    Widget.IsPresentedChanged += OnIsPresentedChanged;

                    await UpdateMasterDetail();
                    UpdateMasterBehavior();
                    UpdateIsPresented();
                    UpdateBarTextColor();
                    UpdateBarBackgroundColor();
                }

                e.NewElement.PropertyChanged += OnElementPropertyChanged;
            }
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals(nameof(MasterDetailPage.Master)) || e.PropertyName.Equals(nameof(MasterDetailPage.Detail)))
                await UpdateMasterDetail();
            else if (e.PropertyName == MasterDetailPage.IsPresentedProperty.PropertyName)
                UpdateIsPresented();
            else if (e.PropertyName == MasterDetailPage.MasterBehaviorProperty.PropertyName)
                UpdateMasterBehavior();
        }

        private async void HandleMasterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Xamarin.Forms.Page.IconProperty.PropertyName)
                await UpdateHamburguerIconAsync();
        }

        private async Task UpdateMasterDetail()
        {
            Page.Master.PropertyChanged -= HandleMasterPropertyChanged;
            await UpdateHamburguerIconAsync();

            if (Platform.GetRenderer(Page.Master) == null)
                Platform.SetRenderer(Page.Master, Platform.CreateRenderer(Page.Master));
            if (Platform.GetRenderer(Page.Detail) == null)
                Platform.SetRenderer(Page.Detail, Platform.CreateRenderer(Page.Detail));

            Widget.Master = Platform.GetRenderer(Page.Master).Container;
            Widget.Detail = Platform.GetRenderer(Page.Detail).Container;
            Widget.MasterTitle = Page.Master?.Title ?? string.Empty;

            UpdateBarTextColor();
            UpdateBarBackgroundColor();

            Page.Master.PropertyChanged += HandleMasterPropertyChanged;
        }

        private void UpdateIsPresented()
        {
            Widget.IsPresented = Page.IsPresented;
        }

        private void UpdateMasterBehavior()
        {
            if (Page.Detail is NavigationPage)
            {
                Widget.MasterBehaviorType = GetMasterBehavior(Page.MasterBehavior);
            }
            else
            {
                // The only way to display Master page is from a toolbar. If we have not access to one,
                // we should force split mode to display menu (as no gestures are implemented).
                Widget.MasterBehaviorType = MasterBehaviorType.Split;
            }

            Widget.DisplayTitle = Widget.MasterBehaviorType != MasterBehaviorType.Split;
        }

        private void UpdateBarTextColor()
        {
            var navigationPage = Platform.NativeToolbarTracker.Navigation;

            if (navigationPage != null)
            {
                var barTextColor = navigationPage.BarTextColor;

                Widget.UpdateBarTextColor(barTextColor.ToGtkColor());
            }
        }

        private void UpdateBarBackgroundColor()
        {
            var navigationPage = Platform.NativeToolbarTracker.Navigation;

            if (navigationPage != null)
            {
                var barBackgroundColor = navigationPage.BarBackgroundColor;
                Widget.UpdateBarBackgroundColor(barBackgroundColor.ToGtkColor());
            }
        }

        private async Task UpdateHamburguerIconAsync()
        {
            var hamburguerIcon = Page.Master.Icon;

            if (hamburguerIcon != null)
            {
                IImageSourceHandler handler =
                    Registrar.Registered.GetHandler<IImageSourceHandler>(hamburguerIcon.GetType());

                var image = await handler.LoadImageAsync(hamburguerIcon);
                Widget.UpdateHamburguerIcon(image);

                Platform.NativeToolbarTracker.UpdateToolBar();
            }
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
            ElementController.SetValueFromRenderer(MasterDetailPage.IsPresentedProperty, Widget.IsPresented);
        }
    }
}