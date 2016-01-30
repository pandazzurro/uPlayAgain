using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using uPlayAgain.Data.EF.Models;
using uPlayAgain.GameImporter.Model;
using uPlayAgain.GameImporter.Service;
using uPlayAgain.Http.TheGamesDB;

namespace uPlayAgain.GameImporter.ViewModel
{
    public class ImportGameViewModel : ViewModelBase
    {
        private ObservableCollection<GameDto> _gamesList;
        public ObservableCollection<GameDto> GamesDto
        {
            get
            {
                if (_gamesList == null)
                    _gamesList = new ObservableCollection<GameDto>();
                return _gamesList;
            }
            set
            {
                _gamesList = value;
                RaisePropertyChanged("GamesList");
            }
        }

        private GameDto _selectedGameDto;

        public GameDto SelectedGameDto
        {
            get { return _selectedGameDto; }
            set
            {
                _selectedGameDto = value;
                RaisePropertyChanged("SelectedGameDto");
            }
        }

        public bool _isGameDtoEditMode;
        public bool IsGameDtoEditMode
        {
            get { return _isGameDtoEditMode; }
            set
            {
                _isGameDtoEditMode = value;
                RaisePropertyChanged("IsGameDtoEditMode");
            }
        }
        public DateTime? DataInizio { get; set; }
        public DateTime? DataFine { get; set; }
        public Platform CurrentPlatform { get; set; }
        public Genre CurrentGenre { get; set; }
        public ObservableCollection<Platform> AvailablePlatforms { get; set; }
        public ObservableCollection<Genre> AvailableGenres { get; set; }
        private bool _loadingPopupOpen;

        public bool LoadingPopupOpen
        {
            get { return _loadingPopupOpen; }
            set
            {
                _loadingPopupOpen = value;
                RaisePropertyChanged("LoadingPopupOpen");
            }
        }
        
        private IMapper _mapper { get; set; }
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
        private ICommand _loadTheGameDbGameCommand;
        public ICommand LoadTheGameDbGameCommand
        {
            get
            {
                if(_loadTheGameDbGameCommand == null)
                    _loadTheGameDbGameCommand = new RelayCommand(async () => await LoadTheGameDbGame());
                return _loadTheGameDbGameCommand;
            }
        }
        public ICommand SaveSelectedGameCommand { get; private set; }
        public ICommand EnableEditModeCommand { get; private set; }
        public ICommand SelectAllGameCommand { get; private set; }
        public ICommand CloseLoadingPopupCommand { get; private set; }
        private ICommand _importTouPlayAgainCommand;
        public ICommand ImportTouPlayAgainCommand
        {
            get
            {
                if (_importTouPlayAgainCommand == null)
                    _importTouPlayAgainCommand = new RelayCommand(async () => await ImportTouPlayAgain());
                return _importTouPlayAgainCommand;
            }
        }
        private IConnectionWebApi _currentWebApi;

        
        //public ImportGameViewModel()
        public ImportGameViewModel(IConnectionWebApi api)
        {
            _currentWebApi = api;
            AvailableGenres = new ObservableCollection<Genre>(_currentWebApi.GetGenres());
            AvailableGenres.Add(default(Genre));
            AvailablePlatforms = new ObservableCollection<Platform>(_currentWebApi.GetPlatforms());
            AvailablePlatforms.Add(default(Platform));
            SaveSelectedGameCommand = new RelayCommand(async () => await SaveSelectedGame());
            EnableEditModeCommand = new RelayCommand(EnableEditMode);
            SelectAllGameCommand = new RelayCommand(() => SelectAllGame());
            CloseLoadingPopupCommand = new RelayCommand(CloseLoadingPopup);

            _mapper = new MapperConfiguration(MapperGameImport).CreateMapper();
            BindingOperations.EnableCollectionSynchronization(GamesDto, _lock);            
        }

        private void CloseLoadingPopup()
        {
            LoadingPopupOpen = !LoadingPopupOpen;
        }

        private void MapperGameImport(IMapperConfiguration map)
        {
            map.CreateMap<Game, GameDto>();
        }

        public async Task LoadTheGameDbGame()
        {
            await Task.Factory.StartNew(async () =>
            {
                if (GamesDto.Count > 0)
                    GamesDto.Clear();
                List<GameSummary> gs = new List<GameSummary>();
                if (CurrentPlatform != default(Platform))
                    gs = await _currentWebApi.TheGamesDBGameListByPlatform(DataInizio, DataFine, CurrentPlatform);
                else
                    AvailablePlatforms.Where(x => x != default(Platform)).ToList().ForEach(async _currentPlatform =>
                    {
                        gs.AddRange(await _currentWebApi.TheGamesDBGameListByPlatform(DataInizio, DataFine, _currentPlatform));
                    });

                double percentageToIncrement = (double)100 / gs.Count;
                ProgressBarValue = 0;

                gs.ForEach(async currentGameSummary =>
                {
                    ProgressBarValue += percentageToIncrement;
                    try
                    {
                        GamesDto.Add(_mapper.Map<GameDto>(await _currentWebApi.TheGamesDBGetGameDetails(currentGameSummary)));
                    }
                    catch (Exception ex)
                    {
                        string a = ex.Message;
                    }
                });

                // Apro il popup di segnalazione. Finita importazione
                LoadingPopupOpen = true;
            });
        }
        public async Task ImportTouPlayAgain()
        {   
            GamesDto.Where(x => x.IsSelected).ToList().ForEach(async game =>
            {
                // Se esiste già l'idImport allora NON posso inserirlo
                if(await _currentWebApi.GetGameByFieldSearch(new Game() { ImportId = game.ImportId }) != default(Game))
                {
                    await _currentWebApi.InsertGame(game);
                }
            });
            // Rimuovo i giochi già processati
            GamesDto = new ObservableCollection<GameDto>(GamesDto.Where(x => !x.IsSelected));
        }

        public async Task SaveSelectedGame()
        {
            await Task.Factory.StartNew(async () =>
            {
                string a = "";
            });
        }

        public void EnableEditMode()
        {
            IsGameDtoEditMode = true;
        }

        public void SelectAllGame()
        {
            GamesDto.ToList().ForEach(game => game.IsSelected = true);
        }
    }
}
