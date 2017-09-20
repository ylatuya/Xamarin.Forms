using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.WPF;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Maps.WPF;

[assembly: Dependency(typeof(StringProvider))]
namespace Xamarin.Forms.ControlGallery.WPF
{
	public partial class MainWindow 
	{
		public MainWindow()
		{
			Forms.Init();
			FormsMaps.Init();
			var app = new Controls.App();
			InitializeComponent();
			LoadApplication(app);
		}
	}

	public class StringProvider : IStringProvider
	{
		public string CoreGalleryTitle { get { return "WPF Core Gallery"; } }
	}
}
