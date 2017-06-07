using Gdk;
using Gtk;
using System;
using Xamarin.Forms.Platform.GTK.Animations;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public enum MasterBehaviorType
    {
        Default = 0,
        Popover,
        Split
    }

    public class MasterDetailPage : Fixed
    {
        private const int DefaultMasterWidth = 300;
        private const int IsPresentedAnimationMilliseconds = 300;

        private bool _isPresented;
        private MasterDetailMasterTitleContainer _titleContainer;
        private VBox _masterContainer;
        private Widget _master;
        private Widget _detail;
        private MasterBehaviorType _masterBehaviorType;
        private bool _displayTitle;
        private bool _animationsEnabled;

        public MasterDetailPage()
        {
            _animationsEnabled = false;
            _masterBehaviorType = MasterBehaviorType.Default;

            // Master Stuff
            _masterContainer = new VBox();
            _titleContainer = new MasterDetailMasterTitleContainer();
            _titleContainer.HamburguerClicked += OnHamburgerClicked;
            _titleContainer.HeightRequest = GtkToolbarConstants.ToolbarHeight;
            _masterContainer.PackStart(_titleContainer, false, true, 0);

            _master = new EventBox();
            _masterContainer.PackEnd(_master, false, true, 0);

            // Detail Stuff
            _detail = new EventBox();

            Add(_detail);
            Add(_masterContainer);
        }

        public MasterBehaviorType MasterBehaviorType
        {
            get
            {
                return _masterBehaviorType;
            }

            set
            {
                if (_masterBehaviorType != value)
                {
                    _masterBehaviorType = value;
                }
            }
        }

        public Widget Master
        {
            get
            {
                return _master;
            }

            set
            {
                RefreshMaster(value);
            }
        }

        public Widget Detail
        {
            get
            {
                return _detail;
            }

            set
            {
                RefreshDetail(value);
            }
        }

        public bool IsPresented
        {
            get
            {
                return _isPresented;
            }

            set
            {
                RefreshPresented(value);
                NotifyIsPresentedChanged();
            }
        }

        public string MasterTitle
        {
            get
            {
                return _titleContainer.Title;
            }

            set
            {
                _titleContainer.Title = value;
            }
        }

        public bool DisplayTitle
        {
            get
            {
                return _displayTitle;
            }

            set
            {
                RefreshDisplayTitle(value);
            }
        }

        public static Pixbuf GetHamburgerPixBuf()
        {
            try
            {
                var hamburgerPixBuf = new Pixbuf("./Resources/hamburger.png");

                return hamburgerPixBuf;
            }
            catch
            {
                return null;
            }
        }

        public event EventHandler IsPresentedChanged;

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);

            _master.WidthRequest = DefaultMasterWidth;
            _master.HeightRequest = _detail.HeightRequest = allocation.Height;

            switch (_masterBehaviorType)
            {
                case MasterBehaviorType.Split:
                    _detail.WidthRequest = allocation.Width - DefaultMasterWidth;
                    _detail.MoveTo(_master.WidthRequest, 0);
                    break;
                case MasterBehaviorType.Default:
                case MasterBehaviorType.Popover:
                    _detail.WidthRequest = allocation.Width;
                    _detail.MoveTo(0, 0);
                    break;
            }
        }

        protected override void OnShown()
        {
            base.OnShown();

            _animationsEnabled = true;
        }

        private void RefreshMaster(Widget newMaster)
        {
            if (_master != null)
            {
                _masterContainer.RemoveFromContainer(_master);
            }

            _master = newMaster;
            _masterContainer.PackEnd(newMaster, false, true, 0);
            _master.ShowAll();
        }

        private void RefreshDetail(Widget newDetail)
        {
            if (_detail != null)
            {
                this.RemoveFromContainer(_detail);
            }

            Remove(_masterContainer);

            _detail = newDetail;

            Add(_detail);
            Add(_masterContainer);

            _detail.ShowAll();
        }

        private async void RefreshPresented(bool isPresented)
        {
            _isPresented = isPresented;

            if (_masterBehaviorType == MasterBehaviorType.Split) return;

            if (_animationsEnabled)
            {
                var from = (_isPresented) ? -DefaultMasterWidth : 0;
                var to = (_isPresented) ? 0 : -DefaultMasterWidth;

                await new FloatAnimation(from, to, TimeSpan.FromMilliseconds(IsPresentedAnimationMilliseconds), true, (f) =>
                {
                    Gtk.Application.Invoke(delegate
                    {
                        _masterContainer.MoveTo(f, 0);
                    });
                }).Run();
            }
            else
            {
                _masterContainer.MoveTo(_isPresented ? 0 : -DefaultMasterWidth, 0);
            }
        }

        private void RefreshDisplayTitle(bool value)
        {
            _displayTitle = value;

            _masterContainer.RemoveFromContainer(_titleContainer);

            if (_displayTitle)
            {
                _masterContainer.PackStart(_titleContainer, false, true, 0);
            }
        }

        private void OnHamburgerClicked(object sender, EventArgs e)
        {
            IsPresented = !IsPresented;
        }

        private void NotifyIsPresentedChanged()
        {
            IsPresentedChanged?.Invoke(this, EventArgs.Empty);
        }

        private class MasterDetailMasterTitleContainer : EventBox
        {
            private HBox _root;
            private ToolButton _hamburguerButton;
            private Gtk.Label _titleLabel;

            public MasterDetailMasterTitleContainer()
            {
                _root = new HBox();
                var hamburguerIcon = new Gtk.Image();

                try
                {
                    var hamburgerPixBuf = GetHamburgerPixBuf();
                    hamburguerIcon = new Gtk.Image(hamburgerPixBuf);
                }
                catch (Exception ex)
                {
                    Internals.Log.Warning("MasterDetailPage HamburguerIcon", "Could not load hamburguer icon: {0}", ex);
                }

                _hamburguerButton = new ToolButton(hamburguerIcon, string.Empty);
                _hamburguerButton.HeightRequest = GtkToolbarConstants.ToolbarItemHeight;
                _hamburguerButton.WidthRequest = GtkToolbarConstants.ToolbarItemWidth;
                _hamburguerButton.Clicked += OnHamburguerButtonClicked;

                _titleLabel = new Gtk.Label();

                _root.PackStart(_hamburguerButton, false, false, GtkToolbarConstants.ToolbarItemSpacing);
                _root.PackStart(_titleLabel, false, false, 25);

                Add(_root);
            }

            public string Title
            {
                get
                {
                    return _titleLabel.Text;
                }

                set
                {
                    _titleLabel.Text = value ?? string.Empty;
                }
            }

            public event EventHandler HamburguerClicked;

            private void OnHamburguerButtonClicked(object sender, EventArgs e)
            {
                HamburguerClicked?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}