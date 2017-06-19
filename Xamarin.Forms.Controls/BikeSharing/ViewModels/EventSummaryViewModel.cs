namespace Xamarin.Forms.Controls
{
    public class EventSummaryViewModel : BindableObject
    {
        private Event _event;

        public EventSummaryViewModel(object parameter)
        {
            Event = parameter as Event;
        }

        public Event Event
        {
            get
            {
                return _event;
            }

            set
            {
                _event = value;
                OnPropertyChanged();
            }
        }
    }
}