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
        public ObservableCollection <Game> GamesList { get; set; }
        public DateTime? DataInizio { get; set; }
        public DateTime? DataFine { get; set; }
        public Platform CurrentPlatform { get; set; }
        public Genre CurrentGenre { get; set; }
        private ConnectionWebApi _currentWebApi { get; set; }

        public ICommand LoadTheGameDbGameCommand;
        public ImportGameViewModel(ConnectionWebApi api)
        {
            _currentWebApi = api;
            LoadTheGameDbGameCommand = new RelayCommand(async () => await LoadTheGameDbGame());
        }

        public async Task LoadTheGameDbGame()
        {
            List<GameSummary> gs = await _currentWebApi.TheGamesDBGameListByPlatform(DataInizio, DataFine, CurrentPlatform);
            gs.ForEach(async currentGameSummary =>
            {
                await _currentWebApi.TheGamesDBGetGameDetails(currentGameSummary);
            });
            
        }

    }
}
