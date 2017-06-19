namespace Xamarin.Forms.Controls
{
    public partial class EventSummaryView : ContentPage
    {
        public EventSummaryView(object parameter)
        {
            InitializeComponent();

            BindingContext = new EventSummaryViewModel(parameter);
        }
    }
}