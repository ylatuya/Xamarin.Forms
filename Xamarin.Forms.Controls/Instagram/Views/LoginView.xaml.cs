namespace Xamarin.Forms.Controls.Instagram.Views
{
    public partial class LoginView : ContentPage
    {
        public LoginView()
        {
            InitializeComponent();

            LoginBtn.Clicked += async (sender, args) =>
            {
                await Navigation.PushAsync(new FeedView());
            };
        }
    }
}