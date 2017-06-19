namespace Xamarin.Forms.Controls
{
    public partial class HomeView : ContentPage
    {
        public HomeView()
        {
            InitializeComponent();

            BindingContext = new MainViewModel();
        }
    }
}