using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using uPlayAgain.Data.EF.Models;
using uPlayAgain.GameImporter.Service;
using uPlayAgain.Http.TheGamesDB;

namespace uPlayAgain.GameImporter.ViewModel
{
    public class ImportGameViewModel : ViewModelBase
    {
        private ObservableCollection<Game> _gamesList;
        public ObservableCollection <Game> GamesList
        {
            get
            {
                if(_gamesList == null)
                    _gamesList = new ObservableCollection<Game>();                
                return _gamesList;
            }
            set
            {
                _gamesList = value;
                RaisePropertyChanged("GamesList");
            }
        }
        public DateTime? DataInizio { get; set; }
        public DateTime? DataFine { get; set; }
        public Platform CurrentPlatform { get; set; }
        public Genre CurrentGenre { get; set; }
        public ObservableCollection<Platform> AvailablePlatforms { get; set; }
        public ObservableCollection<Genre> AvailableGenres { get; set; }
        private IConnectionWebApi _currentWebApi;

        public ICommand LoadTheGameDbGameCommand { get; private set; }
        //public ImportGameViewModel()
        public ImportGameViewModel(IConnectionWebApi api)
        {
            _currentWebApi = api;
            AvailableGenres = new ObservableCollection<Genre>(_currentWebApi.GetGenres());
            AvailablePlatforms = new ObservableCollection<Platform>(_currentWebApi.GetPlatforms());
            LoadTheGameDbGameCommand = new RelayCommand(LoadTheGameDbGame);
        }

        public void LoadTheGameDbGame()
        {
            Task.Factory.StartNew(() =>
            {
                List<GameSummary> gs = null;
                if (CurrentPlatform != null)
                    gs = _currentWebApi.TheGamesDBGameListByPlatform(DataInizio, DataFine, CurrentPlatform).Result;
                else
                    AvailablePlatforms.ToList().ForEach(_currentPlatform =>
                    {
                        gs.AddRange(_currentWebApi.TheGamesDBGameListByPlatform(DataInizio, DataFine, _currentPlatform).Result);
                    });

                gs.ForEach(currentGameSummary =>
                {
                    GamesList.Add(_currentWebApi.TheGamesDBGetGameDetails(currentGameSummary).Result);
                });
            }).Wait();
        }

    }
}
