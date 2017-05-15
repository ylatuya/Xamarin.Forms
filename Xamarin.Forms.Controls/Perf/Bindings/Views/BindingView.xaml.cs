using Xamarin.Forms;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms.Controls.Perf.Bindings.ViewModels;

namespace Xamarin.Forms.Controls.Perf.Bindings.Views
{
    public partial class BindingView : ContentPage
    {
        public BindingView()
        {
            InitializeComponent();
            BindingContext = new BindingViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(TimeSpan.FromSeconds(3));

            // Call ID: SetValueActual, Call Count: 559, Time: 4.9589 ms
            Debug.WriteLine(PerformanceProfiler.GetStats());
        }
    }
}