using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.Forms.Controls.Perf.Bindings.Views
{
    public partial class NoBindingView : ContentPage
    {
        public NoBindingView()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(TimeSpan.FromSeconds(3));

            // Call ID: SetValueActual, Call Count: 469, Time: 2.9479 ms
            Debug.WriteLine(PerformanceProfiler.GetStats());
        }
    }
}