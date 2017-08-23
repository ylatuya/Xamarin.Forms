using System.Threading.Tasks;
using Movies.ViewModels.Base;
using Movies.Models.Movie;
using Movies.Services.People;
using Movies.Models.People;
using Xamarin.Forms.Maps;
using System.Collections.ObjectModel;

namespace Movies.ViewModels
{
    public class PeopleViewModel : ViewModelBase
    {
        private Person _person;
        private ObservableCollection<Position> _positions;

        private IPeopleService _peopleService;

        public PeopleViewModel(IPeopleService peopleService)
        {
            _peopleService = peopleService;
        }

        public Person Person
        {
            get { return _person; }
            set
            {
                _person = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Position> Positions
        {
            get { return _positions; }
            set
            {
                _positions = value;
                OnPropertyChanged();
            }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData is MovieCastMember)
            {
                IsBusy = true;

                var movieCastMember = (MovieCastMember)navigationData;
                Person = await _peopleService.FindByIdAsync(movieCastMember.PersonId);

                var geoCoder = new Geocoder();
                var positions = await geoCoder.GetPositionsForAddressAsync(Person.PlaceOfBirth);
                Positions = new ObservableCollection<Position>(positions);

                IsBusy = false;
            }
        }
    }
}