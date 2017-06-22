namespace Xamarin.Forms.Controls
{
    public partial class CustomRideView : ContentPage
    {
        public CustomRideView(object parameter)
        {
            InitializeComponent();

            BindingContext = new CustomRideViewModel();
        }
    }
}