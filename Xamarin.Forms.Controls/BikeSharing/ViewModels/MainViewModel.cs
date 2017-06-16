using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.Controls
{
    public class MainViewModel : BindableObject
    {
        private ObservableCollection<Event> _events;

        private EventsService _eventsService;

        public MainViewModel()
        {
            _eventsService = EventsService.Instance;

            Events = _eventsService.GetEvents();
        }

        public ObservableCollection<Event> Events
        {
            get
            {
                return _events;
            }

            set
            {
                _events = value;
                OnPropertyChanged();
            }
        }
    }
}
