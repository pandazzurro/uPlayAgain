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

        
        //public ImportGameViewModel()
        public ImportGameViewModel(IConnectionWebApi api)
        {
            _currentWebApi = api;
            AvailableGenres = new ObservableCollection<Genre>(_currentWebApi.GetGenres());
            AvailableGenres.Add(default(Genre));
            AvailablePlatforms = new ObservableCollection<Platform>(_currentWebApi.GetPlatforms());
            AvailablePlatforms.Add(default(Platform));
            SaveSelectedGameCommand = new RelayCommand(async () => await SaveSelectedGame());
            EnableEditModeCommand = new RelayCommand<GameDto>(EnableEditMode);
            SelectAllGameCommand = new RelayCommand<bool>(SelectAllGame);
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
            map.CreateMap<Game, GameDto>().ReverseMap();
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
                        if (ProgressBarValue >= 100 )
                            // Apro il popup di segnalazione. Finita importazione
                            LoadingPopupOpen = true;
                    }
                });
            });
        }
        public async Task ImportTouPlayAgain()
        {
            await Task.Factory.StartNew(() =>
            {
                List<GameDto> ToEleborate = GamesDto.Where(x => x.IsChecked).ToList();
                double percentageToIncrement = (double)100 / ToEleborate.Count;
                ProgressBarValue = 0;

                ToEleborate.Where(x => x.IsChecked).ToList().ForEach(async game =>
                {
                    ProgressBarValue += percentageToIncrement;
                    // Se esiste già l'idImport allora NON posso inserirlo
                    if (await _currentWebApi.GetGameByFieldSearch(new Game() { ImportId = game.ImportId }) == default(Game))
                    {
                        await _currentWebApi.InsertGame(_mapper.Map<Game>(game));
                        // Rimuovo i giochi già processati
                        GamesDto.Remove(game);
                    }
                });
            });
        }

        public async Task SaveSelectedGame()
        {
            await Task.Factory.StartNew(async () =>
            {
                string a = "";
            });
        }

        public void EnableEditMode(GameDto game)
        {
            IsGameDtoEditMode = true;
        }
        public void DeleteSelectedGame(GameDto game)
        {
            GamesDto.Remove(GamesDto.Where(x => x.ImportId == game.ImportId).FirstOrDefault());
            RaisePropertyChanged("GamesDto");
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
