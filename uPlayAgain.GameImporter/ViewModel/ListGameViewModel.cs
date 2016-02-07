using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using uPlayAgain.Data.EF.Models;
using uPlayAgain.GameImporter.Model;
using uPlayAgain.GameImporter.Service;
using uPlayAgain.Http.TheGamesDB;

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
        private GameDto _createdGameDto;
        public GameDto CreatedGameDto
        {
            get { return _createdGameDto; }
            set
            {
                _createdGameDto = value;
                RaisePropertyChanged("CreatedGameDto");
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
        private bool _isGameDtoCreateMode;

        public bool IsGameDtoCreateMode
        {
            get { return _isGameDtoCreateMode; }
            set {
                _isGameDtoCreateMode = value;
                RaisePropertyChanged("IsGameDtoCreateMode");
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
        private ICommand _addGameCommand;
        public ICommand AddGameCommand
        {
            get
            {
                if (_addGameCommand == null)
                    _addGameCommand = new RelayCommand(AddGame);
                return _addGameCommand;
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
        private IConnectionWebApi _currentWebApi;

        public ListGameViewModel(IConnectionWebApi webApi)
        {
            _currentWebApi = webApi;
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
            IEnumerable<Game> gs;
            Game GameFilter = new Game();
            await Task.Factory.StartNew(async () =>
            {
                if (GamesDto.Count > 0)
                    GamesDto.Clear();


                // Create oData Filter and count
                if (CurrentPlatform != default(Platform))
                    GameFilter.PlatformId = CurrentPlatform.PlatformId;
                if (CurrentGenre != default(Genre))
                    GameFilter.GenreId = CurrentGenre.GenreId;

                gs = await _currentWebApi.GetGameIds(GameFilter);

                double percentageToIncrement = (double)100 / gs.Count();
                ProgressBarValue = 0;

                // apply oData Filter and load data
                gs.ToList().ForEach(async currentGameSummary =>
                {
                    try
                    {
                        GameDto result = _mapper.Map<GameDto>(await _currentWebApi.GetGameById(new Game() { GameId = currentGameSummary.GameId }));
                        if(result != null)
                            GamesDto.Add(result);
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
            GameDto dtoToUpdate = (GameDto)SelectedGameDto.Clone();
            DeleteSelectedGame(SelectedGameDto);
            GamesDto.Add(dtoToUpdate);            
            await _currentWebApi.UpdateGame(_mapper.Map<Game>(dtoToUpdate));
        }

        public async Task InsertNewGame()
        {
            await _currentWebApi.InsertGame(_mapper.Map<Game>(CreatedGameDto));
        }
       
        public void EnableEditMode(GameDto game)
        {
            IsGameDtoEditMode = true;
        }
        public void DeleteSelectedGame(GameDto game)
        {
            GamesDto.Remove(GamesDto.Where(x => x.ImportId == game.ImportId).FirstOrDefault());
        }
        public void AddGame()
        {
            IsGameDtoCreateMode = true;
            CreatedGameDto = new GameDto()
            {
                Genre = new Genre(),
                Platform = new Platform()
            };
        }
    }
}
