using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK
{
    public class NativeToolbarTracker
    {
        private const int BackButtonItemWidth = 36;
        private const int ToolbarItemWidth = 44;
        private const int ToolbarItemHeight = 44;
        private const int ToolbarItemSpacing = 6;
        private const int NavigationTitleMinSize = 300;
        private const int ToolbarHeight = 72;

        private readonly string _defaultBackButtonTitle = "Back";

        private readonly ToolbarTracker _toolbarTracker;
        private HBox _toolbar;
        private HBox _toolbarNavigationSection;
        private HBox _toolbarTitleSection;
        private HBox _toolbarSection;
        private Gtk.Label _toolbarTitle;
        private NavigationPage _navigation;

        public NativeToolbarTracker()
        {
            _toolbarTracker = new ToolbarTracker();
            _toolbarTracker.CollectionChanged += ToolbarTrackerOnCollectionChanged;
        }

        public HBox Toolbar
        {
            get { return _toolbar; }
        }

        public NavigationPage Navigation
        {
            get { return _navigation; }
            set
            {
                if (_navigation == value)
                    return;

                if (_navigation != null)
                    _navigation.PropertyChanged -= NavigationPagePropertyChanged;

                _navigation = value;

                if (_navigation != null)
                {
                    _toolbarTracker.Target = _navigation.CurrentPage;
                    _navigation.PropertyChanged += NavigationPagePropertyChanged;
                }

                UpdateToolBar();
            }
        }

        protected virtual HBox ConfigureToolbar()
        {
            var toolbar = new HBox();
            toolbar.HeightRequest = ToolbarHeight;

            _toolbarNavigationSection = new HBox();
            toolbar.PackStart(_toolbarNavigationSection, false, true, 0);

            _toolbarTitleSection = new HBox();
            toolbar.PackStart(_toolbarTitleSection, true, true, 0);

            _toolbarSection = new HBox();
            toolbar.PackStart(_toolbarSection, false, true, 0);

            return toolbar;
        }

        private void ToolbarTrackerOnCollectionChanged(object sender, EventArgs eventArgs)
        {
            UpdateToolbarItems();
        }

        private void UpdateToolbarItems()
        {
            if (_toolbar == null || _navigation == null)
                return;

            var currentPage = _navigation.Peek(0);
            UpdateItems(currentPage.ToolbarItems);
        }

        private void NavigationPagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(NavigationPage.BarTextColorProperty.PropertyName) ||
                e.PropertyName.Equals(NavigationPage.BarBackgroundColorProperty.PropertyName))
                UpdateToolBar();
        }

        private void UpdateTitle()
        {
            if (_toolbar == null || _navigation == null)
                return;

            var title = GetCurrentPageTitle();

            if (_toolbarTitle != null)
            {
                var span = new Span()
                {
                    FontSize = 12.0d,
                    Text = title
                };

                _toolbarTitle.SetTextFromSpan(span);
            }
        }

        private string GetCurrentPageTitle()
        {
            if (_navigation == null)
                return string.Empty;
            return _navigation.Peek(0).Title ?? string.Empty;
        }

        private void UpdateBarBackgroundColor(Controls.Page page)
        {
            if (Navigation != null && Navigation.BarBackgroundColor != Color.Default)
            {
                var backgroundColor = Navigation.BarBackgroundColor.ToGtkColor();

                if (_toolbar != null && page != null)
                {
                    page.SetToolbarColor(backgroundColor);
                }
            }
        }

        private void UpdateBarTextColor()
        {
            if (Navigation != null && Navigation.BarTextColor != Color.Default)
            {
                var textColor = Navigation.BarTextColor.ToGtkColor();

                if (_toolbar != null && _toolbarTitle != null)
                {
                    _toolbarTitle.ModifyFg(StateType.Normal, textColor);
                }
            }
        }

        private void UpdateItems(IList<ToolbarItem> toolBarItems)
        {
            foreach (var toolBarItem in toolBarItems.Where(t => t.Order != ToolbarItemOrder.Secondary))
            {
                var newToolButtonIcon = new Gtk.Image(toolBarItem.Icon.ToPixbuf());
                ToolButton newToolButton = new ToolButton(newToolButtonIcon, toolBarItem.Text);
                newToolButton.HeightRequest = ToolbarItemHeight;
                newToolButton.WidthRequest = ToolbarItemWidth;
                newToolButton.TooltipText = toolBarItem.Text;

                _toolbarSection.PackStart(newToolButton, false, false, ToolbarItemSpacing);

                newToolButton.Clicked += (sender, args) =>
                {
                    toolBarItem.Command?.Execute(toolBarItem.CommandParameter);
                };
            }

            var secondaryToolBarItems = toolBarItems.Where(t => t.Order == ToolbarItemOrder.Secondary);

            if (secondaryToolBarItems.Any())
            {
                ToolButton secondaryButton = new ToolButton(Stock.Add);
                secondaryButton.HeightRequest = ToolbarItemHeight;
                secondaryButton.WidthRequest = ToolbarItemWidth;
                _toolbarSection.PackStart(secondaryButton, false, false, 0);

                Menu menu = new Menu();
                foreach (var secondaryToolBarItem in secondaryToolBarItems)
                {
                    Gtk.MenuItem menuItem = new Gtk.MenuItem(secondaryToolBarItem.Text);
                    menu.Add(menuItem);

                    menu.ButtonPressEvent += (sender, args) =>
                    {
                        secondaryToolBarItem.Command?.Execute(secondaryToolBarItem.CommandParameter);
                    };
                }

                secondaryButton.Clicked += (sender, args) =>
                {
                    menu.ShowAll();
                    menu.Popup();
                };
            }
        }

        private bool ShowBackButton()
        {
            if (_navigation == null)
                return false;

            return NavigationPage.GetHasBackButton(_navigation.CurrentPage)
                && !IsRootPage();
        }

        private bool IsRootPage()
        {
            if (_navigation == null)
                return true;

            return _navigation.StackDepth <= 1;
        }

        private void UpdateNavigationItems()
        {
            if (_toolbar == null || _navigation == null)
                return;

            var navigationItems = new List<ToolbarItem>();

            if (ShowBackButton())
            {
                var backButtonItem = new ToolbarItem
                {
                    Text = GetPreviousPageTitle(),
                    Command = new Command(async () => await NavigateBackFromBackButton())
                };

                navigationItems.Add(backButtonItem);
            }

            if(navigationItems.Any())
            {
                foreach(var navigationItem in navigationItems)
                {
                    ToolButton navigationButton = new ToolButton(Stock.GoBack);
                    navigationButton.HeightRequest = ToolbarItemHeight;
                    navigationButton.WidthRequest = BackButtonItemWidth;
                    _toolbarNavigationSection.PackStart(navigationButton, false, false, ToolbarItemSpacing);

                    navigationButton.Clicked += (sender, args) =>
                    {
                        navigationItem.Command?.Execute(navigationItem.CommandParameter);
                    };
                }
            }
        }

        private string GetPreviousPageTitle()
        {
            if (_navigation == null || _navigation.StackDepth <= 1)
                return string.Empty;

            return _navigation.Peek(1).Title ?? _defaultBackButtonTitle;
        }

        private async Task NavigateBackFromBackButton()
        {
            var popAsyncInner = _navigation?.PopAsyncInner(true, true);

            if (popAsyncInner != null)
                await popAsyncInner;
        }

        internal void UpdateToolBar()
        {
            if (_navigation == null)
            {
                if (_toolbar != null)
                    _toolbar.Visible = false;

                _toolbar = null;

                return;
            }

            var currentPage = _navigation.Peek(0);

            if (NavigationPage.GetHasNavigationBar(currentPage))
            {
                _toolbar = ConfigureToolbar();
                _toolbarTitle = new Gtk.Label
                {
                    WidthRequest = NavigationTitleMinSize
                };

                _toolbarTitleSection.Add(_toolbarTitle);

                UpdateNavigationItems();
                UpdateTitle();
                UpdateToolbarItems();
                UpdateBarTextColor();

                var pageRenderer = Platform.GetRenderer(currentPage);
                var page = pageRenderer?.Container?.Child as Controls.Page;

                if(page != null)
                {
                    page.Toolbar = _toolbar;
                    UpdateBarBackgroundColor(page);
                }
            }
            else
            {
                if (_toolbar != null)
                {
                    _toolbar.Visible = false;
                }
            }
        }
    }
}