namespace Xamarin.Forms.Controls
{
    public partial class ReportIncidentView : ContentPage
    {
        public ReportIncidentView(object parameter)
        {
            InitializeComponent();

            BindingContext = new ReportIncidentViewModel();
        }
    }
}