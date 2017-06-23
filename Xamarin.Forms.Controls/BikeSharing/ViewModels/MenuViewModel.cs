using System.Collections.ObjectModel;
using System.Windows.Input;

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

        public ICommand ItemSelectedCommand => new Command<BikeMenuItem>(OnSelectItem);

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
                ViewModelType = typeof(CustomRideViewModel),
                IsEnabled = true
            });

            MenuItems.Add(new BikeMenuItem
            {
                Title = "My Rides",
                MenuItemType = MenuItemType.MyRides,
                ViewModelType = typeof(MainViewModel),
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
                ViewModelType = typeof(ReportIncidentViewModel),
                IsEnabled = true
            });

            MenuItems.Add(new BikeMenuItem
            {
                Title = "Profile",
                MenuItemType = MenuItemType.Profile,
                ViewModelType = typeof(MainViewModel),
                IsEnabled = true
            });
        }

        private void OnSelectItem(BikeMenuItem item)
        {
            if (item.IsEnabled)
            {
                object parameter = null;

                NavigationService.Instance.NavigateTo(item.ViewModelType, parameter);
            }
        }
    }
}
