using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.GTK
{
    internal class GtkPlatformServices : IPlatformServices
    {
        public bool IsInvokeRequired
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string RuntimePlatform => Device.GTK;

        public void BeginInvokeOnMainThread(Action action)
        {
            Gtk.Application.Invoke((o, s) => action());
        }

        public Ticker CreateTicker()
        {
            throw new NotImplementedException();
        }

        public Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public string GetMD5Hash(string input)
        {
            throw new NotImplementedException();
        }

        public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
        {
            switch (size)
            {
                case NamedSize.Default:
                    return 11;
                case NamedSize.Micro:
                    return 12;
                case NamedSize.Small:
                    return 14;
                case NamedSize.Medium:
                    return 17;
                case NamedSize.Large:
                    return 22;
                default:
                    throw new ArgumentOutOfRangeException(nameof(size));
            }
        }

        public Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IIsolatedStorageFile GetUserStoreForApplication()
        {
            throw new NotImplementedException();
        }

        public void OpenUriAction(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            var timer = new System.Timers.Timer(interval.TotalMilliseconds);

            timer.Elapsed += (sender, args) =>
            {
                var result = callback();

                if (!result)
                {
                    timer.Stop();
                }
            };

            timer.Start();
        }
    }
}