using System.Collections.ObjectModel;

namespace Xamarin.Forms.Controls
{
    public class MenuViewModel : BindableObject
    {
        private ObservableCollection<BikeMenuItem> _menuItems;

        public MenuViewModel()
        {
            _menuItems = new ObservableCollection<BikeMenuItem>();
            InitMenuItems();
        }

        public ObservableCollection<BikeMenuItem> MenuItems
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
        private void InitMenuItems()
        {
            MenuItems.Add(new BikeMenuItem
            {
                Title = "Home",
                MenuItemType = MenuItemType.Home,
                ViewModelType = typeof(MainViewModel),
                IsEnabled = true
            });

            MenuItems.Add(new BikeMenuItem
            {
                Title = "New Ride",
                MenuItemType = MenuItemType.NewRide,
                IsEnabled = true
            });

            MenuItems.Add(new BikeMenuItem
            {
                Title = "My Rides",
                MenuItemType = MenuItemType.MyRides,
                IsEnabled = true
            });

            MenuItems.Add(new BikeMenuItem
            {
                Title = "Upcoming ride",
                MenuItemType = MenuItemType.UpcomingRide,
                IsEnabled = true
            });

            MenuItems.Add(new BikeMenuItem
            {
                Title = "Report",
                MenuItemType = MenuItemType.ReportIncident,
                IsEnabled = true
            });

            MenuItems.Add(new BikeMenuItem
            {
                Title = "Profile",
                MenuItemType = MenuItemType.Profile,
                IsEnabled = true
            });
        }
    }
}
