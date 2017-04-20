using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.GTK
{
    internal class ResourcesProvider : ISystemResourcesProvider
    {
        public IResourceDictionary GetSystemResources()
        {
            return new ResourceDictionary();
        }
    }
}
