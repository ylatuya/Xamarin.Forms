using Xamarin.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System;

namespace Xamarin.Forms.Controls.CustomControls
{
    public partial class ChartPage : TabbedPage
    {
        private PlotModel _areaPlotModel;
        private PlotModel _barPlotModel;
        private PlotModel _columnPlotModel;
        private PlotModel _functionPlotModel;
        private PlotModel _linePlotModel;
        private PlotModel _piePlotModel;
        private PlotModel _stackBarPlotModel;
        private PlotModel _scatterPlotModel;
        private PlotModel _volumePlotModel;

        public ChartPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadAreaPlot();
            LoadBarPlot();
            LoadColumnPlot();
            LoadFunctionPlot();
            LoadLinePlot();
            LoadPiePlot();
            LoadScatterPlot();
            LoadStackBarPlot();
            LoadVolumePlot();
        }

        private void LoadAreaPlot()
        {
            _areaPlotModel = new PlotModel
            {
                Title = "Area"
            };

            var areaSerie = new AreaSeries
            {
                StrokeThickness = 2.0
            };

            areaSerie.Points.Add(new DataPoint(0, 50));
            areaSerie.Points.Add(new DataPoint(10, 60));
            areaSerie.Points.Add(new DataPoint(20, 140));
            areaSerie.Points2.Add(new DataPoint(0, 50));
            areaSerie.Points2.Add(new DataPoint(5, 70));
            areaSerie.Points2.Add(new DataPoint(15, 60));

            _areaPlotModel.Series.Add(areaSerie);

            AreaPlot.Model = _areaPlotModel;
        }

        private void LoadBarPlot()
        {
            _barPlotModel = new PlotModel
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

            _barPlotModel.Series.Add(barSerie);

            BarPlot.Model = _barPlotModel;
        }

        private void LoadColumnPlot()
        {
            _columnPlotModel = new PlotModel
            {
                Title = "Column"
            };

            var columnSerie = new ColumnSeries
            {
                StrokeThickness = 2.0
            };

            columnSerie.Items.Add(new ColumnItem(1));
            columnSerie.Items.Add(new ColumnItem(2));
            columnSerie.Items.Add(new ColumnItem(3));
            columnSerie.Items.Add(new ColumnItem(4));
            columnSerie.Items.Add(new ColumnItem(5));

            _columnPlotModel.Series.Add(columnSerie);

            ColumnPlot.Model = _columnPlotModel;
        }

        private void LoadFunctionPlot()
        {
            _functionPlotModel = new PlotModel
            {
                Title = "Function"
            };

            Func<double, double> batFn1 = (x) => 2 * Math.Sqrt(-Math.Abs(Math.Abs(x) - 1) * Math.Abs(3 - Math.Abs(x)) / ((Math.Abs(x) - 1) * (3 - Math.Abs(x)))) * (1 + Math.Abs(Math.Abs(x) - 3) / (Math.Abs(x) - 3)) * Math.Sqrt(1 - Math.Pow((x / 7), 2)) + (5 + 0.97 * (Math.Abs(x - 0.5) + Math.Abs(x + 0.5)) - 3 * (Math.Abs(x - 0.75) + Math.Abs(x + 0.75))) * (1 + Math.Abs(1 - Math.Abs(x)) / (1 - Math.Abs(x)));
            Func<double, double> batFn2 = (x) => -3 * Math.Sqrt(1 - Math.Pow((x / 7), 2)) * Math.Sqrt(Math.Abs(Math.Abs(x) - 4) / (Math.Abs(x) - 4));
            Func<double, double> batFn3 = (x) => Math.Abs(x / 2) - 0.0913722 * (Math.Pow(x, 2)) - 3 + Math.Sqrt(1 - Math.Pow((Math.Abs(Math.Abs(x) - 2) - 1), 2));
            Func<double, double> batFn4 = (x) => (2.71052 + (1.5 - .5 * Math.Abs(x)) - 1.35526 * Math.Sqrt(4 - Math.Pow((Math.Abs(x) - 1), 2))) * Math.Sqrt(Math.Abs(Math.Abs(x) - 1) / (Math.Abs(x) - 1)) + 0.9;

            _functionPlotModel.Series.Add(new FunctionSeries(batFn1, -8, 8, 0.0001));
            _functionPlotModel.Series.Add(new FunctionSeries(batFn2, -8, 8, 0.0001));
            _functionPlotModel.Series.Add(new FunctionSeries(batFn3, -8, 8, 0.0001));
            _functionPlotModel.Series.Add(new FunctionSeries(batFn4, -8, 8, 0.0001));

            FunctionPlot.Model = _functionPlotModel;
        }

        private void LoadLinePlot()
        {
            _linePlotModel = new PlotModel
            {
                Title = "Line"
            };

            var lineSerie = new LineSeries
            {
                StrokeThickness = 2.0
            };

            lineSerie.Points.Add(new DataPoint(0, 0));
            lineSerie.Points.Add(new DataPoint(10, 20));
            lineSerie.Points.Add(new DataPoint(30, 1));
            lineSerie.Points.Add(new DataPoint(40, 12));

            _linePlotModel.Series.Add(lineSerie);

            LinePlot.Model = _linePlotModel;
        }

        private void LoadPiePlot()
        {
            _piePlotModel = new PlotModel
            {
                Title = "Pie"
            };

            var pieSlice = new PieSeries
            {
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.8,
                AngleSpan = 360,
                StartAngle = 0
            };

            pieSlice.Slices.Add(new PieSlice("1º", 1) { IsExploded = true });
            pieSlice.Slices.Add(new PieSlice("2º", 2) { IsExploded = true });
            pieSlice.Slices.Add(new PieSlice("3º", 3) { IsExploded = true });
            pieSlice.Slices.Add(new PieSlice("4º", 4) { IsExploded = true });
            pieSlice.Slices.Add(new PieSlice("5º", 5) { IsExploded = true });

            _piePlotModel.Series.Add(pieSlice);

            PiePlot.Model = _piePlotModel;
        }

        private void LoadScatterPlot()
        {
            _scatterPlotModel = new PlotModel
            {
                Title = "Scatter"
            };

            var scatterSerie = new ScatterSeries
            {
                MarkerType = MarkerType.Circle
            };

            var r = new Random(100);
            for (int i = 0; i < 100; i++)
            {
                var x = r.NextDouble();
                var y = r.NextDouble();
                var size = r.Next(5, 15);
                var colorValue = r.Next(100, 1000);
                scatterSerie.Points.Add(new ScatterPoint(x, y, size, colorValue));
            }

            _scatterPlotModel.Series.Add(scatterSerie);

            ScatterPlot.Model = _scatterPlotModel;
        }

        private void LoadStackBarPlot()
        {
            _stackBarPlotModel = new PlotModel
            {
                Title = "Bar"
            };

            var barSerie1 = new BarSeries
            {
                Title = "Series 1",
                IsStacked = true,
                StrokeColor = OxyColors.Black,
                StrokeThickness = 1
            };
            barSerie1.Items.Add(new BarItem { Value = 1 });
            barSerie1.Items.Add(new BarItem { Value = 2 });
            barSerie1.Items.Add(new BarItem { Value = 3 });
            barSerie1.Items.Add(new BarItem { Value = 4 });

            var barSerie2 = new BarSeries
            {
                Title = "Series 2",
                IsStacked = true,
                StrokeColor = OxyColors.Black,
                StrokeThickness = 1
            };

            barSerie2.Items.Add(new BarItem { Value = 4 });
            barSerie2.Items.Add(new BarItem { Value = 3 });
            barSerie2.Items.Add(new BarItem { Value = 2 });
            barSerie2.Items.Add(new BarItem { Value = 1 });

            _stackBarPlotModel.Series.Add(barSerie1);
            _stackBarPlotModel.Series.Add(barSerie2);

            StackBarPlot.Model = _stackBarPlotModel;
        }

        private void LoadVolumePlot()
        {
            _volumePlotModel = new PlotModel
            {
                Title = "Volume"
            };

            var volumeSeries = new VolumeSeries
            {
                StrokeThickness = 2.0
            };

            volumeSeries.Items.Add(new OhlcvItem(1, 20, 4, 1, 1));
            volumeSeries.Items.Add(new OhlcvItem(2, 10, 6, 1, 2));
            volumeSeries.Items.Add(new OhlcvItem(3, 50, 3, 1, 0.5));

            _volumePlotModel.Series.Add(volumeSeries);

            VolumePlot.Model = _volumePlotModel;
        }
    }
}