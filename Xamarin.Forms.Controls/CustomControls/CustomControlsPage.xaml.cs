using Xamarin.Forms;

namespace Xamarin.Forms.Controls.CustomControls
{
    public partial class CustomControlsPage : ContentPage
    {
        public CustomControlsPage()
        {
            InitializeComponent();

            ColorPickerBtn.Clicked += (sender, args) => Navigation.PushAsync(new ColorPickerPage());
            ExpanderBtn.Clicked += (sender, args) => Navigation.PushAsync(new ExpanderPage());
            HyperLinkBtn.Clicked += (sender, args) => Navigation.PushAsync(new HyperLinkPage());
        }
    }
}