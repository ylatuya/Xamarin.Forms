using System;
using System.Linq;
using Gtk;
using Xamarin.Forms.Platform.GTK.Animations;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public enum MasterBehaviorType
    {
        Default = 0,
        Popover,
        Split
    }

    public class MasterDetailPage : VBox
    {
        public const int DefaultWidth = 300;
        public const int DefaultToolbarSize = 48;

        private MasterBehaviorType _masterBehaviorType;

        private bool _isPresented;
        private Widget _master;
        private Widget _detail;
        private Gdk.Color _toolbarForeground;
        private Gdk.Color _toolbarBackground;
        private HBox _hboxContainer;
        private Fixed _masterContainer;
        private int _masterPanelWidth = DefaultWidth;
        private Gtk.Button _menuButton;
        private HBox _masterToolbar;
        private Gtk.Label _detailTitleLabel;

        private System.Action RefreshPresented;

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

                    RefreshBehavior();
                }
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
                if (_isPresented != value)
                {
                    _isPresented = value;

                    RefreshPresented();
                }
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
                if (_detail != value)
                {
                    RefreshDetail(value);
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
                if (_master != value)
                {
                    _master = value;
                    RefreshMaster();
                }
            }
        }

        public int MasterPanelWidth
        {
            get
            {
                return _masterPanelWidth;
            }
            set
            {
                _masterPanelWidth = value;
                RefreshMasterSize();
            }
        }

        public string DetailTitle
        {
            get
            {
                return _detailTitleLabel.Text;
            }
            set
            {
                _detailTitleLabel.Text = value;
            }
        }

        public Gdk.Color ToolbarForeground
        {
            get
            {
                return _toolbarForeground;
            }
            set
            {
                _toolbarForeground = value;
                _detailTitleLabel.ModifyFg(StateType.Normal, value);
            }
        }

        public Gdk.Color ToolbarBackground
        {
            get
            {
                return _toolbarBackground;
            }
            set
            {
                _toolbarBackground = value;
                _menuButton.ModifyBg(StateType.Normal, value);
                _masterToolbar.ModifyBg(StateType.Normal, value);
            }
        }

        public bool DetailTitleVisibility
        {
            get
            {
                return _detailTitleLabel.Visible;
            }

            set
            {
                _detailTitleLabel.Visible = value;
            }
        }

        public bool MasterToolbarVisibility
        {
            get
            {
                return _masterToolbar.Visible;
            }

            set
            {
                _masterToolbar.Visible = value;
            }
        }

        public bool ContentTogglePaneButtonVisibility
        {
            get
            {
                return _menuButton.Visible;
            }

            set
            {
                _menuButton.Visible = value;
            }
        }

        public MasterDetailPage()
        {
            RefreshBehavior();
            RefreshPresented();
        }

        private void RefreshBehavior()
        {
            switch (_masterBehaviorType)
            {
                case MasterBehaviorType.Popover:
                    RefreshPresented = RefreshPopoverPresented;
                    break;
                case MasterBehaviorType.Default:
                case MasterBehaviorType.Split:
                default:
                    var root = new VBox();
                    var header = new HBox();
                    _menuButton = new Gtk.Button();
                    _menuButton.WidthRequest = DefaultToolbarSize;
                    _menuButton.HeightRequest = DefaultToolbarSize;
                    _menuButton.Label = null;
                    _menuButton.CanFocus = false;
                    _menuButton.Relief = ReliefStyle.None;
                    var image = new Gtk.Image(new Gdk.Pixbuf("./Resources/hamburger.png"));
                    _menuButton.Image = image;
                    _menuButton.Clicked += OnMenuButtonClicked;
                    header.PackStart(_menuButton, false, false, 0);
                    _masterToolbar = new HBox();
                    _masterToolbar.HeightRequest = DefaultToolbarSize;
                    _detailTitleLabel = new Gtk.Label();
                    _masterToolbar.PackStart(_detailTitleLabel, false, false, 25);
                    header.PackEnd(_masterToolbar);
                    root.PackStart(header, false, false, 0);
                    _hboxContainer = new HBox();
                    root.PackEnd(_hboxContainer);
                    _masterContainer = new Fixed();
                    _masterContainer.SizeAllocated += MasterContainer_SizeAllocated;
                    _masterContainer.HasWindow = true;
                    _hboxContainer.PackStart(_masterContainer, false, false, 0);
                    Add(root);
                    RefreshPresented = RefreshSplitPresented;
                    break;
            }
        }

        private void MasterContainer_SizeAllocated(object o, SizeAllocatedArgs args)
        {
            RefreshMasterSize();
        }

        private void OnMenuButtonClicked(object sender, EventArgs e)
        {
            IsPresented = !IsPresented;
        }

        private void RefreshPopoverPresented()
        {

        }

        private void RefreshMasterSize()
        {
            if (_master != null)
            {
                _master.WidthRequest = _masterPanelWidth;
                _master.HeightRequest = _masterContainer.Allocation.Height;
            }
        }

        private async void RefreshSplitPresented()
        {
            if (_isPresented)
            {
                _masterContainer.Visible = true;
                _masterContainer.WidthRequest = _masterPanelWidth;
            }

            var from = (_isPresented) ? 0 : _masterPanelWidth;
            var to = (_isPresented) ? _masterPanelWidth : 0;

            await new FloatAnimation(from, to, TimeSpan.FromSeconds(0.5), true, (f) =>
            {
                Gtk.Application.Invoke(delegate
                {
                    _masterContainer.WidthRequest = (int)f;
                    RefreshMasterSize();
                });
            }).Run();

            if (!_isPresented)
            {
                _masterContainer.Visible = false;
                _masterContainer.WidthRequest = 0;
            }
        }

        private void RefreshMaster()
        {
            if (_masterContainer.Children.Length > 0)
            {
                _masterContainer.Remove(_masterContainer.Children.First());
            }

            _masterContainer.Add(_master);

            RefreshPresented();
        }

        private void RefreshDetail(Widget newDetail)
        {
            if (_detail != null)
            {
                _hboxContainer.Remove(_detail);
            }

            _detail = newDetail;

            _hboxContainer.PackEnd(_detail, true, true, 0);
            _detail.ShowAll();
        }


        private void RefreshPopoverDetail()
        {

        }
    }
}