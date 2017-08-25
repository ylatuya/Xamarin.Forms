using Movies.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Movies.Converters
{
    public class MenuItemTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var menuItemType = (MenuItemType)value;

            switch (menuItemType)
            {
                case MenuItemType.Discover:
                    return "movies-discover.png";
                case MenuItemType.Movies:
                    return "movies-movie.png";
                case MenuItemType.TVShows:
                    return "movies-shows.png";
                case MenuItemType.Upcoming:
                    return "movies-upcoming.png";
                case MenuItemType.Favourites:
                    return "movies-favourites.png";
                case MenuItemType.Settings:
                    return "movies-settings.png";
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