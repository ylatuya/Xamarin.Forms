using Xamarin.Forms.Controls.Timeline.Models;

namespace Xamarin.Forms.Controls.Timeline.Views
{
    public partial class TimelineView : ContentPage
    {
        public TimelineView()
        {
            InitializeComponent();

            BindingContext = TimelineDataFactory.Classes;
        }

        private void TimelineItemTapped(object sender, ItemTappedEventArgs e)
        {
            TimelineListView.SelectedItem = null;
        }
    }
}