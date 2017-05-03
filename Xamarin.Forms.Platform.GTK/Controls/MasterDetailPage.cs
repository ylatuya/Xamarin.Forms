using Gtk;
using System;
using System.Linq;
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
        private MasterBehaviorType _masterBehaviorType;
        private bool _isPresented;
        private Widget _master;
        private Widget _detail;
        private HBox _rootPanel;
        private VBox _masterContent;
        private VBox _masterContainer;

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
                    _rootPanel = new HBox();
                    _masterContent = new VBox();
                    var masterHeader = new HBox();

                    var menuButton = new Gtk.Button();
                    menuButton.Name = "menuButton";
                    menuButton.Label = null;
                    menuButton.CanFocus = false;
                    var image = new Gtk.Image(new Gdk.Pixbuf("./Resources/hamburger.png"));
                    menuButton.Image = image;
                    menuButton.Clicked += OnMenuButtonClicked;

                    masterHeader.PackStart(menuButton, false, false, 0);
                    _masterContent.PackStart(masterHeader, false, false, 0);

                    _masterContainer = new VBox();
                    _masterContent.PackEnd(_masterContainer, true, true, 3);

                    _rootPanel.PackStart(_masterContent, false, false, 0);

                    Add(_rootPanel);
                    RefreshPresented = RefreshSplitPresented;
                    break;
            }
        }

        private void OnMenuButtonClicked(object sender, EventArgs e)
        {
            IsPresented = !IsPresented;
        }

        private void RefreshPopoverPresented()
        {

        }

        private async void RefreshSplitPresented()
        {
            _masterContainer.Visible = _isPresented;

            var from = (_isPresented) ? 0 : 200;
            var to = (_isPresented) ? 200 : 0;

            await new FloatAnimation(from, to, TimeSpan.FromSeconds(0.5), true, (f) =>
            {
                Gtk.Application.Invoke(delegate
                {
                    _masterContent.WidthRequest = (int)f;
                });
            }).Run();
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
                _rootPanel.Remove(newDetail);
            }

            _detail = newDetail;

            _rootPanel.PackEnd(_detail, true, true, 0);
        }

        private void RefreshPopoverDetail()
        {

        }
    }
}