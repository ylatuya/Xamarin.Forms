using Xamarin.Forms;
using OxyPlot;
using OxyPlot.Series;

namespace Xamarin.Forms.Controls.CustomControls
{
    public partial class ChartPage : ContentPage
    {
        private PlotModel _plotModel;

        public ChartPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _plotModel = new PlotModel
            {
                Title = "Bar"
            };

            var barSerie = new BarSeries
            {
                StrokeThickness = 2.0
            };

            barSerie.Items.Add(new BarItem(1));
            barSerie.Items.Add(new BarItem(2));
            barSerie.Items.Add(new BarItem(3));
            barSerie.Items.Add(new BarItem(4));
            barSerie.Items.Add(new BarItem(5));

            _plotModel.Series.Add(barSerie);

            GtkPlot.Model = _plotModel;
        }
    }
}