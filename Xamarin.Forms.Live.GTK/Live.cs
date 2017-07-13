using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Xamarin.Forms.Live.GTK
{
    public static class Live
    {
        static readonly Regex Regex = new Regex("x:Class=\"([^\"]+)\"");

        public static void Init(Application application)
        {
            var directory = Directory.GetCurrentDirectory();

            var fw = new FileSystemWatcher(directory)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite
            };

            fw.Changed += (sender, eventArgs) =>
            {
                Console.WriteLine(string.Format("Waiting changes in XAML files from {0}", eventArgs.FullPath));

                var extension = Path.GetExtension(eventArgs.FullPath);

                if (extension != ".xaml")
                    return;

                var path = eventArgs.FullPath;

                var xaml = string.Empty;
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var textReader = new StreamReader(fileStream))
                {
                    xaml = textReader.ReadToEnd();
                }

                Console.WriteLine(xaml);

                var match = Regex.Match(xaml);
                if (!match.Success) return;
                var className = match.Groups[1].Value;
                var page = GetPage(application.MainPage, className);

                if (page == null)
                    return;

                UpdatePageFromXaml(page, xaml);
            };
        }

        private static Page GetPage(Page page, string fullTypeName)
        {
            if (page == null)
                return null;

            if (page.GetType().FullName == fullTypeName)
                return page;

            return null;
        }

        private static void UpdatePageFromXaml(Page page, string xaml)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var bindingContext = page.BindingContext;
                try
                {
                    Console.WriteLine("Loading XAML...");
                    LoadXaml(page, xaml);
                    page.ForceLayout();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    page.BindingContext = bindingContext;
                    Console.WriteLine("XAML Loaded!");
                }
            });
        }

        private static void LoadXaml(BindableObject view, string xaml)
        {
            var xamlAssembly = Assembly.Load(new AssemblyName("Xamarin.Forms.Xaml"));
            var xamlLoaderType = xamlAssembly.GetType("Xamarin.Forms.Xaml.XamlLoader");
            var loadMethod = xamlLoaderType.GetRuntimeMethod("Load", new[] { typeof(BindableObject), typeof(string) });

            try
            {
                loadMethod.Invoke(null, new object[] { view, xaml });
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }
    }
}