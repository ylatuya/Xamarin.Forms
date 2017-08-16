using Xamarin.Forms.Controls.Instagram.ViewModels;

namespace Xamarin.Forms.Controls.Instagram.Views
{
    public partial class FeedView : ContentPage
    {
        public FeedView()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            BindingContext = new FeedViewModel();
        }
    }
}