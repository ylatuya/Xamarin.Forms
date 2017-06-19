using System;
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

        private static ObservableCollection<Suggestion> suggestions = new ObservableCollection<Suggestion>
        {
            new Suggestion
            {
                Name = "Central Park",
                Distance = 1900,
                ImagePath = "suggestion_central_park.png",
                Latitude = 40.7828687f,
                Longitude = -73.9675438f
            },
            new Suggestion
            {
                Name = "Flushing Meadows Corona Park",
                Distance = 2200,
                ImagePath = "suggestion_corona_park.png",
                Latitude = 40.7397176f,
                Longitude = -73.8429737f
            },
            new Suggestion
            {
                Name = "Liberty State Park",
                Distance = 3500,
                ImagePath = "suggestion_liberty_state_park.png",
                Latitude = 40.703336f,
                Longitude = -73.8429737f
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

        public ObservableCollection<Suggestion> GetSuggestions()
        {
            return suggestions;
        }
    }
}
