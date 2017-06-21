namespace Xamarin.Forms.Controls
{
    public partial class MenuView : ContentPage
    {
        public MenuView()
        {
            InitializeComponent();

            BindingContext = new MenuViewModel();
        }
    }
}