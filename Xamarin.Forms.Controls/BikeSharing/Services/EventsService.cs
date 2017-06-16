using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.Controls
{
    public class EventsService
    {
        private static EventsService _instance;

        private static ObservableCollection<Event> events = new ObservableCollection<Event>
        {
            new Event
            {
                Name = "NBA Match",
                StartTime = DateTime.Now,
                ImagePath = "https://connect16test.blob.core.windows.net/imgs/i_nba-match.png"
            },
            new Event
            {
                Name = "Music Ride",
                StartTime = DateTime.Now,
                ImagePath = "https://connect16test.blob.core.windows.net/imgs/i_music-ride.png"
            }
        };

        public static EventsService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventsService();
                }

                return _instance;
            }
        }

        public ObservableCollection<Event> GetEvents()
        {
            return events;
        }
    }
}
