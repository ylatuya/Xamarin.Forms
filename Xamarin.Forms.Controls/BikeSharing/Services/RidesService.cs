namespace Xamarin.Forms.Controls.BikeSharing.Services
{
    public class RidesService
    {
        private static RidesService _instance;

        public static RidesService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RidesService();
                }

                return _instance;
            }
        }
    }
}