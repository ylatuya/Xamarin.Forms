using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Forms.Controls
{
    public class NavigationService
    {
        private static NavigationService _instance;

        private IDictionary<Type, Type> viewModelRouting =
            new Dictionary<Type, Type>()
        {
            { typeof(LoginViewModel), typeof(LoginView) },
            { typeof(MainViewModel), typeof(MainView) },
            { typeof(EventSummaryViewModel), typeof(EventSummaryView) },
            { typeof(CustomRideViewModel), typeof(CustomRideView) },
            { typeof(ReportIncidentViewModel), typeof(ReportIncidentView) }
         };

        public static NavigationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NavigationService();
                }

                return _instance;
            }
        }

        public void NavigateTo(Type viewModelType, object parameter = null)
        {
            Type pageType = viewModelRouting[viewModelType];
            InternalNavigation(pageType, parameter);
        }

        public void NavigateTo<TDestinationViewModel>(object parameter = null)
        {
            Type pageType = viewModelRouting[typeof(TDestinationViewModel)];
            InternalNavigation(pageType, parameter);
        }

        private void InternalNavigation(Type pageType, object parameter = null)
        {
            var page = Activator.CreateInstance(pageType, parameter) as Page;

            if (page is LoginView)
            {
                Application.Current.MainPage.Navigation.PushAsync(page);
            }
            else if (page is MainView)
            {
                Application.Current.MainPage = page;
            }
            else
            {
                var mainPage = Application.Current.MainPage as MainView;
                var navigationPage = mainPage.Detail as CustomNavigationPage;

                if (navigationPage != null)
                {
                    navigationPage.PushAsync(page);
                }
            }
        }

        public void NavigateBack()
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}