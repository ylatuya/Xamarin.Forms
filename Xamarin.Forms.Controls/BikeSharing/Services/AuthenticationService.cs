namespace Xamarin.Forms.Controls
{
    public class AuthenticationService
    {
        private static AuthenticationService _instance;

        public static AuthenticationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AuthenticationService();
                }

                return _instance;
            }
        }

        public bool Login(string username, string password)
        {
            if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                return true;

            return false;
        }
    }
}
