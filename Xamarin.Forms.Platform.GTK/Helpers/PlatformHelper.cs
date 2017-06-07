using System;

namespace Xamarin.Forms.Platform.GTK.Helpers
{
    public enum GTKPlatform
    {
        Linux,
        MacOS,
        Windows
    }

    public class PlatformHelper
    {
        public static GTKPlatform GetGTKPlatform()
        {
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;

            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return GTKPlatform.Windows;
                case PlatformID.Unix:
                    return GTKPlatform.Linux;
                case PlatformID.MacOSX:
                    return GTKPlatform.MacOS;
                default:
                    return GTKPlatform.Windows;

            }
        }
    }
}
