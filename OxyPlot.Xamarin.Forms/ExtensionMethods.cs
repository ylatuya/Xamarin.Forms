namespace OxyPlot.Xamarin.Forms
{
    public static class ExtensionMethods
    {
        public static global::Xamarin.Forms.Color ToXamarinForms(this OxyColor c)
        {
            return global::Xamarin.Forms.Color.FromRgba(c.R, c.G, c.B, c.A);
        }

        public static OxyColor ToOxyColor(this global::Xamarin.Forms.Color c)
        {
            return OxyColor.FromArgb((byte)(c.A * 255), (byte)(c.R * 255), (byte)(c.G * 255), (byte)(c.B * 255));
        }
    }
}