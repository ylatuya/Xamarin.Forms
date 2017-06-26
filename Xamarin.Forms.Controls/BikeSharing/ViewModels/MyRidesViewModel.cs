using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Xamarin.Forms.Controls
{
    public class MyRidesViewModel : BindableObject
    {
        private ObservableCollection<Ride> _myRides;
    
        public MyRidesViewModel()
        {
            MyRides = RidesService.Instance.GetUserRides();
        }

        public ObservableCollection<Ride> MyRides
        {
            get
            {
                return _myRides;
            }
            set
            {
                _myRides = value;
                OnPropertyChanged();
            }
        }
    }
}