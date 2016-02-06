using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPlayAgain.Data.EF.Models;
using uPlayAgain.GameImporter.Service;

namespace uPlayAgain.GameImporter.ViewModel
{
    public class ListGameViewModel : ViewModelBase
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
                RaisePropertyChanged("GamesDto");
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

        private ICommand _loadFromuPlauAgainCommand;
        public ICommand LoadFromuPlayAgainCommand
        {
            get
            {
                if (_loadFromuPlauAgainCommand == null)
                    _loadFromuPlauAgainCommand = new RelayCommand(async () => await LoadFromuPlayAgain());
                return _loadFromuPlauAgainCommand;
            }
        }
        private ICommand _saveSelectedGameCommand;
        public ICommand SaveSelectedGameCommand
        {
            get
            {
                if (_saveSelectedGameCommand == null)
                    _saveSelectedGameCommand = new RelayCommand(SaveSelectedGame);
                return _saveSelectedGameCommand;
            }
        }
        private ICommand _enableEditModeCommand;
        public ICommand EnableEditModeCommand
        {
            get
            {
                if (_enableEditModeCommand == null)
                    _enableEditModeCommand = new RelayCommand<GameDto>(EnableEditMode);
                return _enableEditModeCommand;
            }
        }
        private ICommand _selectAllGameCommand;
        public ICommand SelectAllGameCommand
        {
            get
            {
                if (_selectAllGameCommand == null)
                    _selectAllGameCommand = new RelayCommand<bool>(SelectAllGame);
                return _selectAllGameCommand;
            }
        }
        private ICommand _closeLoadingPopupCommand;
        public ICommand CloseLoadingPopupCommand
        {
            get
            {
                if (_closeLoadingPopupCommand == null)
                    _closeLoadingPopupCommand = new RelayCommand(CloseLoadingPopup);
                return _closeLoadingPopupCommand;
            }
        }
        private ICommand _updateGameCommand;
        public ICommand UpdateGameCommand
        {
            get
            {
                if (_updateGameCommand == null)
                    _updateGameCommand = new RelayCommand(async () => await UpdateGame());
                return _updateGameCommand;
            }
        }
        private ICommand _insertGameCommand;
        public ICommand InsertNewGameCommand
        {
            get
            {
                if (_insertGameCommand == null)
                    _insertGameCommand = new RelayCommand(async () => await InsertNewGame());
                return _insertGameCommand;
            }
        }
        private ICommand _deleteSelectedGameCommand;
        public ICommand DeleteSelectedGameCommand
        {
            get
            {
                if (_deleteSelectedGameCommand == null)
                    _deleteSelectedGameCommand = new RelayCommand<GameDto>(DeleteSelectedGame);
                return _deleteSelectedGameCommand;
            }
        }
        private ICommand _deleteAllGameCommand;
        public ICommand DeleteAllGameCommand
        {
            get
            {
                if (_deleteAllGameCommand == null)
                    _deleteAllGameCommand = new RelayCommand(DeleteAllGame);
                return _deleteAllGameCommand;
            }
        }
        private ICommand _deleteAllGameSelectedCommand;
        public ICommand DeleteAllGameSelectedCommand
        {
            get
            {
                if (_deleteAllGameSelectedCommand == null)
                    _deleteAllGameSelectedCommand = new RelayCommand(DeleteAllGameSelected);
                return _deleteAllGameSelectedCommand;
            }
        }
        private IConnectionWebApi _currentWebApi;

        public ListGameViewModel(IConnectionWebApi webApi)
        {
            _connectionWebApi = webApi;
            _currentWebApi = api;
            AvailableGenres = new ObservableCollection<Genre>(_currentWebApi.GetGenres());
            AvailableGenres.Add(default(Genre));
            AvailablePlatforms = new ObservableCollection<Platform>(_currentWebApi.GetPlatforms());
            AvailablePlatforms.Add(default(Platform));
            BindingOperations.EnableCollectionSynchronization(GamesDto, _lock);
            _mapper = new MapperConfiguration(MapperGameImport).CreateMapper();
        }

        private void CloseLoadingPopup()
        {
            LoadingPopupOpen = !LoadingPopupOpen;
        }

        private void MapperGameImport(IMapperConfiguration map)
        {
            map.CreateMap<Game, GameDto>().ReverseMap();
        }

        public async Task LoadFromuPlayAgain()
        {
            await Task.Factory.StartNew(async () =>
            {
                if (GamesDto.Count > 0)
                    GamesDto.Clear();
                List<GameSummary> gs = new List<GameSummary>();
                // Create oData Filter and count
                if (CurrentPlatform != default(Platform))
                    gs = await _currentWebApi.TheGamesDBGameListByPlatform(DataInizio, DataFine, CurrentPlatform);
                else
                    AvailablePlatforms.Where(x => x != default(Platform)).ToList().ForEach(async _currentPlatform =>
                    {
                        gs.AddRange(await _currentWebApi.TheGamesDBGameListByPlatform(DataInizio, DataFine, _currentPlatform));
                    });

                double percentageToIncrement = (double)100 / gs.Count;
                ProgressBarValue = 0;

                // apply oData Filter and load data
                gs.ForEach(async currentGameSummary =>
                {
                    try
                    {
                        if (await _currentWebApi.GetGameByFieldSearch(new Game() { ImportId = currentGameSummary.ID }) == default(Game))
                        {
                            GameDto result = _mapper.Map<GameDto>(await _currentWebApi.TheGamesDBGetGameDetails(currentGameSummary));
                            GamesDto.Add(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        string a = ex.Message;
                    }
                    finally
                    {
                        ProgressBarValue += percentageToIncrement;
                        if (ProgressBarValue >= 100)
                            // Apro il popup di segnalazione. Finita importazione
                            LoadingPopupOpen = true;
                    }
                });
            });
        }

        public async Task UpdateGame()
        {
            await _currentWebApi.UpdateGame(_mapper.Map<Game>(game));
        }

        public async Task InsertNewGame()
        {
            await _currentWebApi.CreateAsync(_mapper.Map<Game>(game));
        }
       
        public void SaveSelectedGame()
        {
            DeleteSelectedGame(SelectedGameDto);
            GamesDto.Add(SelectedGameDto);
        }

        public void EnableEditMode(GameDto game)
        {
            IsGameDtoEditMode = true;
        }
        public void DeleteSelectedGame(GameDto game)
        {
            GamesDto.Remove(GamesDto.Where(x => x.ImportId == game.ImportId).FirstOrDefault());
        }
        public void DeleteAllGame()
        {
            GamesDto.Clear();
            RaisePropertyChanged("GamesDto");
        }
        public void DeleteAllGameSelected()
        {
            List<GameDto> GamesToRemove = GamesDto.Where(x => x.IsChecked == true).ToList();
            GamesToRemove.ForEach(game => GamesDto.Remove(game));
            RaisePropertyChanged("GamesDto");
        }
        public void SelectAllGame(bool check)
        {
            GamesDto.ToList().ForEach(game => game.IsChecked = check);
            RaisePropertyChanged<GameDto>();
        }
    }
}
