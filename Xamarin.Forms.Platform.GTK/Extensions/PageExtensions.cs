namespace Xamarin.Forms.Platform.GTK.Extensions
{
    internal static class PageExtensions
    {
        internal static bool ShouldDisplayNativeWindow(this Page page)
        {
            return page.Parent is NavigationPage ||
                   page.Parent is MasterDetailPage;
        }
    }
}
