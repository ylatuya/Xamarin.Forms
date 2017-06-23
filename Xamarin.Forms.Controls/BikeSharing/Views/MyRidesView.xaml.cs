namespace Xamarin.Forms.Controls
{
    public partial class MyRidesView : ContentPage
    {
        public MyRidesView(object parameter)
        {
            InitializeComponent();

            BindingContext = new MyRidesViewModel();
        }
    }
}