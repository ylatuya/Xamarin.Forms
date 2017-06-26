using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.Controls
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

        private static ObservableCollection<Station> stations = new ObservableCollection<Station>
        {
            new Station
            {
                Id = 1,
                Name = "Alki Beach Park I",
                Slots = 22,
                Occupied = 4,
                Latitude = 47.5790791f,
                Longitude = -122.4136163f
            },
            new Station
            {
                Id = 2,
                Name = "Alki Beach Park II",
                Slots = 12,
                Occupied = 7,
                Latitude = 47.5743905f,
                Longitude = -122.4023376f
            },
            new Station
            {
                Id = 3,
                Name = "Alki Point Lighthouse",
                Slots = 5,
                Occupied = 15,
                Latitude = 47.5766275f,
                Longitude = -122.4217906f
            }
        };

        private static ObservableCollection<Ride> rides = new ObservableCollection<Ride>
        {
            new Ride
            {
                EventId = 1,
                RideType = RideType.Event,
                Name = "Ride Cultural",
                Start = DateTime.Now.AddDays(-7),
                Stop = DateTime.Now.AddDays(-7),
                Duration = 3600,
                Distance = 19,
                From = stations[0].Name,
                To = stations[2].Name
            },
            new Ride
            {
                RideType = RideType.Custom,
                Start = DateTime.Now.AddDays(-14),
                Stop = DateTime.Now.AddDays(-14),
                Duration = 2500,
                Distance = 8900,
                From = stations[1].Name,
                To = stations[0].Name
            },
            new Ride
            {
                RideType = RideType.Suggestion,
                Start = DateTime.Now.AddDays(-14),
                Stop = DateTime.Now.AddDays(-14),
                Duration = 1800,
                Distance = 10100,
                From = stations[2].Name,
                To = stations[1].Name
            },
                        new Ride
            {
                EventId = 1,
                RideType = RideType.Event,
                Name = "Ride Cultural",
                Start = DateTime.Now.AddDays(-7),
                Stop = DateTime.Now.AddDays(-7),
                Duration = 3600,
                Distance = 19,
                From = stations[0].Name,
                To = stations[2].Name
            },
            new Ride
            {
                RideType = RideType.Custom,
                Start = DateTime.Now.AddDays(-14),
                Stop = DateTime.Now.AddDays(-14),
                Duration = 2500,
                Distance = 8900,
                From = stations[1].Name,
                To = stations[0].Name
            },
            new Ride
            {
                RideType = RideType.Suggestion,
                Start = DateTime.Now.AddDays(-14),
                Stop = DateTime.Now.AddDays(-14),
                Duration = 1800,
                Distance = 10100,
                From = stations[2].Name,
                To = stations[1].Name
            },
                        new Ride
            {
                EventId = 1,
                RideType = RideType.Event,
                Name = "Ride Cultural",
                Start = DateTime.Now.AddDays(-7),
                Stop = DateTime.Now.AddDays(-7),
                Duration = 3600,
                Distance = 19,
                From = stations[0].Name,
                To = stations[2].Name
            },
            new Ride
            {
                RideType = RideType.Custom,
                Start = DateTime.Now.AddDays(-14),
                Stop = DateTime.Now.AddDays(-14),
                Duration = 2500,
                Distance = 8900,
                From = stations[1].Name,
                To = stations[0].Name
            },
            new Ride
            {
                RideType = RideType.Suggestion,
                Start = DateTime.Now.AddDays(-14),
                Stop = DateTime.Now.AddDays(-14),
                Duration = 1800,
                Distance = 10100,
                From = stations[2].Name,
                To = stations[1].Name
            },
                        new Ride
            {
                EventId = 1,
                RideType = RideType.Event,
                Name = "Ride Cultural",
                Start = DateTime.Now.AddDays(-7),
                Stop = DateTime.Now.AddDays(-7),
                Duration = 3600,
                Distance = 19,
                From = stations[0].Name,
                To = stations[2].Name
            },
            new Ride
            {
                RideType = RideType.Custom,
                Start = DateTime.Now.AddDays(-14),
                Stop = DateTime.Now.AddDays(-14),
                Duration = 2500,
                Distance = 8900,
                From = stations[1].Name,
                To = stations[0].Name
            },
            new Ride
            {
                RideType = RideType.Suggestion,
                Start = DateTime.Now.AddDays(-14),
                Stop = DateTime.Now.AddDays(-14),
                Duration = 1800,
                Distance = 10100,
                From = stations[2].Name,
                To = stations[1].Name
            }
        };

        public ObservableCollection<Ride> GetUserRides()
        {
            return rides;
        }
    }
}