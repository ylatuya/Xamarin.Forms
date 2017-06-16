using System;
using System.Diagnostics;
using Xamarin.Forms;
using System.Windows.Input;

namespace Xamarin.Forms.Controls
{
    public class LoginViewModel : BindableObject
    {
        private string _userName;
        private string _password;

        private readonly AuthenticationService _authenticationService;
        private readonly NavigationService _navigationService;

        public LoginViewModel()
        {
            _authenticationService = AuthenticationService.Instance;
            _navigationService = NavigationService.Instance;
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public ICommand SignInCommand => new Command(SignIn);

        private void SignIn()
        {
            bool isAuthenticated = false;

            try
            {
                isAuthenticated = _authenticationService.Login(UserName, Password);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SignIn] Error signing in: {ex}");
            }

            if (isAuthenticated)
            {
                _navigationService.NavigateTo<MainViewModel>();
            }
        }
    }
}