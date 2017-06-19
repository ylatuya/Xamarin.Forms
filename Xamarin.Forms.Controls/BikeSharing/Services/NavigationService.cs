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
            { typeof(EventSummaryViewModel), typeof(EventSummaryView) }
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

        public void NavigateTo<TDestinationViewModel>(object navigationContext = null)
        {
            Type pageType = viewModelRouting[typeof(TDestinationViewModel)];
            var page = Activator.CreateInstance(pageType, navigationContext) as Page;

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