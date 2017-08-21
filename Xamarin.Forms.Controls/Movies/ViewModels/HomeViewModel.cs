using System.Threading.Tasks;
using Movies.ViewModels.Base;
using Movies.Services.Movies;
using Movies.Services.TVShow;
using Movies.Models.Movie;
using System.Collections.ObjectModel;
using System.Linq;
using Movies.Models.TVShow;

namespace Movies.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private Movie _highlight;
        private ObservableCollection<Movie> _topRatedMovies;
        private ObservableCollection<Movie> _popularMovies;
        private ObservableCollection<TVShow> _topRatedTvShows;
        private ObservableCollection<TVShow> _popularTvShows;

        private IMoviesService _moviesService;
        private ITVShowService _tvShowService;

        public HomeViewModel(
            IMoviesService moviesService,
            ITVShowService tvShowService)
        {
            _moviesService = moviesService;
            _tvShowService = tvShowService;

            TopRatedMovies = new ObservableCollection<Movie>();
        }

        public Movie Highlight
        {
            get { return _highlight; }
            set
            {
                _highlight = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Movie> TopRatedMovies
        {
            get { return _topRatedMovies; }
            set
            {
                _topRatedMovies = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Movie> PopularMovies
        {
            get { return _popularMovies; }
            set
            {
                _popularMovies = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TVShow> TopRatedTvShows
        {
            get { return _topRatedTvShows; }
            set
            {
                _topRatedTvShows = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TVShow> PopularTvShows
        {
            get { return _popularTvShows; }
            set
            {
                _popularTvShows = value;
                OnPropertyChanged();
            }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            await LoadTopRatedMoviesAync();
            await LoadPopularMoviesAync();
            await LoadTopRatedTvShowsAync();
            await LoadPopularTvShowsAync();

            IsBusy = false;
        }

        private async Task LoadTopRatedMoviesAync()
        {
            var result = await _moviesService.GetTopRatedAsync();

            TopRatedMovies = new ObservableCollection<Movie>(result.Results);
        }

        private async Task LoadPopularMoviesAync()
        {
            var result = await _moviesService.GetPopularAsync();

            PopularMovies = new ObservableCollection<Movie>(result.Results);
            Highlight = result.Results.First();
        }

        private async Task LoadTopRatedTvShowsAync()
        {
            var result = await _tvShowService.GetTopRatedAsync();

            TopRatedTvShows = new ObservableCollection<TVShow>(result.Results);
        }

        private async Task LoadPopularTvShowsAync()
        {
            var result = await _tvShowService.GetPopularAsync();

            PopularTvShows = new ObservableCollection<TVShow>(result.Results);
        }
    }
}