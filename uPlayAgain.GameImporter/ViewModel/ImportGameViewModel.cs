using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using uPlayAgain.Data.EF.Models;
using uPlayAgain.GameImporter.Service;
using uPlayAgain.Http.TheGamesDB;

namespace uPlayAgain.GameImporter.ViewModel
{
    public class ImportGameViewModel : ViewModelBase
    {
        private ObservableCollection<Game> _gamesList;
        public ObservableCollection<Game> GamesList
        {
            get
            {
                if (_gamesList == null)
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
        private static object _lock = new object();

        private double _progressBarValue;

        public double ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;
                RaisePropertyChanged("ProgressBarValue");
            }
        }
        private IConnectionWebApi _currentWebApi;

        public ICommand LoadTheGameDbGameCommand { get; private set; }
        //public ImportGameViewModel()
        public ImportGameViewModel(IConnectionWebApi api)
        {
            _currentWebApi = api;
            AvailableGenres = new ObservableCollection<Genre>(_currentWebApi.GetGenres());
            AvailablePlatforms = new ObservableCollection<Platform>(_currentWebApi.GetPlatforms());
            LoadTheGameDbGameCommand = new RelayCommand(async () => await LoadTheGameDbGame());            
            BindingOperations.EnableCollectionSynchronization(GamesList, _lock);
        }

        public async Task LoadTheGameDbGame()
        {
            await Task.Factory.StartNew(async () =>
            {
                List<GameSummary> gs = null;
                if (CurrentPlatform != null)
                    gs = await _currentWebApi.TheGamesDBGameListByPlatform(DataInizio, DataFine, CurrentPlatform);
                else
                    AvailablePlatforms.ToList().ForEach(async _currentPlatform =>
                    {
                        gs.AddRange(await _currentWebApi.TheGamesDBGameListByPlatform(DataInizio, DataFine, _currentPlatform));
                    });

                double percentageToIncrement = (double)100 / gs.Count;
                ProgressBarValue = 0;

                gs.ForEach(async currentGameSummary =>
                {
                    ProgressBarValue += percentageToIncrement;
                    GamesList.Add(await _currentWebApi.TheGamesDBGetGameDetails(currentGameSummary));
                });
            });
        }
    }
}
