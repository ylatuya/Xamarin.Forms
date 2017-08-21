using Movies.ViewModels;
using Movies.ViewModels.Base;
using Xamarin.Forms;

namespace Movies.Views
{
    public partial class MainView : MasterDetailPage
    {
        public MainView()
        {
            InitializeComponent();

            BindingContext = Locator.Instance.Resolve(typeof(MainViewModel)) as ViewModelBase;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await(BindingContext as ViewModelBase).InitializeAsync(null);
        }
    }
}