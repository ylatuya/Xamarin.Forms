using Movies.Models;
using Movies.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Movies.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        private ObservableCollection<MenuItem> _menuItems;

        public ObservableCollection<MenuItem> MenuItems
        {
            get
            {
                return _menuItems;
            }
            set
            {
                _menuItems = value;
                OnPropertyChanged();
            }
        }

        public override Task InitializeAsync(object navigationData)
        {
            InitMenuItems();

            return base.InitializeAsync(navigationData);
        }

        private void InitMenuItems()
        {
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem
                {
                    Title = "Discover",
                    MenuItemType = MenuItemType.Discover,
                    ViewModelType = typeof(HomeViewModel),
                    IsEnabled = true
                },
                new MenuItem
                {
                    Title = "Movies",
                    MenuItemType = MenuItemType.Movies,
                    IsEnabled = false
                },
                new MenuItem
                {
                    Title = "TV Shows",
                    MenuItemType = MenuItemType.TVShows,
                    IsEnabled = false
                },
                new MenuItem
                {
                    Title = "Favourites",
                    MenuItemType = MenuItemType.Favourites,
                    IsEnabled = false
                },
                new MenuItem
                {
                    Title = "Settings",
                    MenuItemType = MenuItemType.Settings,
                    IsEnabled = false
                }
            };
        }
    }
}