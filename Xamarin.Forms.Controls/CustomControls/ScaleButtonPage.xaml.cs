using Xamarin.Forms;

namespace Xamarin.Forms.Controls.CustomControls
{
    public partial class ScaleButtonPage : ContentPage
    {
        public ScaleButtonPage()
        {
            InitializeComponent();

            ScaleBtn.ValueChanged += (sender, args) =>
            {
                LabelInfo.Text = 
                string.Format("Value: {0}", ScaleBtn.Value);
            };
        }
    }
}