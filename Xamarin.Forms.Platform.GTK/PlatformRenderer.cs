namespace Xamarin.Forms.Platform.GTK
{
	internal class PlatformRenderer : TransparentEventBox
	{
		public PlatformRenderer(Platform platform)
		{
			Platform = platform;
		}

		public Platform Platform { get; set; }
	}
}
