namespace Xamarin.Forms.Controls
{
    public partial class MainView : MasterDetailPage
    {
        public MainView(object parameter)
        {
            InitializeComponent();

            BindingContext = new MainViewModel();
        }
    }
}