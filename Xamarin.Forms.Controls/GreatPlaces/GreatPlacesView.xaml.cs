using Xamarin.Forms.Controls.GreatPlaces.Models;

namespace Xamarin.Forms.Controls.GreatPlaces.Views
{
    public partial class GreatPlacesView : ContentPage
    {
        public GreatPlacesView()
        {
            InitializeComponent();

            BindingContext = GreatPlacesDataFactory.Places;
        }
    }
}