namespace Xamarin.Forms.Controls
{
    public partial class MainView : ContentPage
    {
        public MainView(object parameter)
        {
            InitializeComponent();

            BindingContext = new MainViewModel();
        }
    }
}