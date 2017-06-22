using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.GTKSpecific;

namespace Xamarin.Forms.Controls
{
    public partial class CustomNavigationPage : NavigationPage
    {
        public CustomNavigationPage() : base()
        {
            InitializeComponent();
        }

        public CustomNavigationPage(Page root) : base(root)
        {
            InitializeComponent();
            this.On<GTK>().SetBackButtonIcon("gtk_back_button.png");
        }
    }
}