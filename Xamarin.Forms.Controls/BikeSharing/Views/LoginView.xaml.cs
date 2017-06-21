namespace Xamarin.Forms.Controls
{
    public partial class LoginView : ContentPage
    {
        public LoginView()
        {
            InitializeComponent();

            BindingContext = new LoginViewModel();
        }
    }
}