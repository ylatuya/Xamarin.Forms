using Movies.Services.Navigation;
using Movies.ViewModels.Base;
using System.Threading.Tasks;

namespace Movies.ViewModels
{
    public class SplashViewModel : ViewModelBase
    {
        private INavigationService _navigationService;

        public SplashViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            //await Task.Delay(3000);
            await _navigationService.NavigateToAsync<MainViewModel>();
        }
    }
}