using Xamarin.Forms;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Xamarin.Forms.Controls.Perf.Layout
{
    public partial class InvalidationView : ContentPage
    {
        public InvalidationView()
        {
            InitializeComponent();
            TestInvalidation();
        }

        private void TestInvalidation()
        {
            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                UpdateLabel();

                Debug.WriteLine(LayoutsProfiler.GetStats());

                return false;
            });
        }

        private void UpdateLabel()
        {
            Button1.Text = "Updated!";
        }
    }
}