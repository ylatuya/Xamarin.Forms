using Xamarin.Forms;

namespace GtkToolkit.Controls
{
    public class ScaleButton : ContentView
    {
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly BindableProperty MinimumProperty =
            BindableProperty.Create("Minimum", typeof(double), typeof(ScaleButton), 0.0d,
                BindingMode.TwoWay);

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly BindableProperty MaximumProperty =
            BindableProperty.Create("Maximum", typeof(double), typeof(ScaleButton), 100.0d,
                BindingMode.TwoWay);

        public double StepIncrement
        {
            get { return (double)GetValue(StepIncrementProperty); }
            set { SetValue(StepIncrementProperty, value); }
        }

        public static readonly BindableProperty StepIncrementProperty =
            BindableProperty.Create("StepIncrement", typeof(double), typeof(ScaleButton), 1.0d,
                BindingMode.TwoWay);
    }
}