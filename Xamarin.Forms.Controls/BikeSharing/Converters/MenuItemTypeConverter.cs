using System;
using System.Globalization;

namespace Xamarin.Forms.Controls
{
    public class MenuItemTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var menuItemType = (MenuItemType)value;

            switch (menuItemType)
            {
                case MenuItemType.Profile:
                    return "ic_user.png";
                case MenuItemType.MyRides:
                    return "ic_my_rides.png";
                case MenuItemType.UpcomingRide:
                    return "ic_upcoming_ride.png";
                case MenuItemType.ReportIncident:
                    return "ic_report.png";
                case MenuItemType.NewRide:
                    return "ic_new_ride.png";
                case MenuItemType.Home:
                    return "ic_home.png";
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
