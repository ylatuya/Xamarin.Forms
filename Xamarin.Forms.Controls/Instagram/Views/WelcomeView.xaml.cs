namespace Xamarin.Forms.Controls.Instagram.Views
{
    public partial class WelcomeView : ContentPage
    {
        public WelcomeView()
        {
            InitializeComponent();

            WelcomeBtn.Clicked += async (sender, args) =>
            {
                await Navigation.PushAsync(new LoginView());
            };
        }
    }
}