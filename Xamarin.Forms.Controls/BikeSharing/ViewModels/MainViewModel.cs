using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Xamarin.Forms.Controls
{
    public class MainViewModel : BindableObject
    {
        private ObservableCollection<Event> _events;
        private ObservableCollection<Suggestion> _suggestions;
        private EventsService _eventsService;

        public MainViewModel()
        {
            _eventsService = EventsService.Instance;

            Events = _eventsService.GetEvents();
            Suggestions = _eventsService.GetSuggestions();
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

        public ICommand ShowEventCommand => new Command<Event>(ShowEvent);

        public ObservableCollection<Suggestion> Suggestions
        {
            get
            {
                return _suggestions;
            }

            set
            {
                _suggestions = value;
                OnPropertyChanged();
            }
        }

        private void ShowEvent(Event @event)
        {
            if (@event != null)
            {
                 NavigationService.Instance.NavigateTo<EventSummaryViewModel>(@event);
            }
        }
    }
}
