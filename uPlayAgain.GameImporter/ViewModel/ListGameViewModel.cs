using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        private string _giochiCaricati;
        public string GiochiCaricati
        {
            get { return _giochiCaricati; }
            set
            {
                _giochiCaricati = value;
                RaisePropertyChanged("GiochiCaricati");
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
        private ICommand _loadEditImage;
        public ICommand LoadEditImageCommand
        {
            get
            {
                if (_loadEditImage == null)
                    _loadEditImage = new RelayCommand(LoadEditImage);
                return _loadEditImage;
            }
        }
        private ICommand _loadCreatingImage;
        public ICommand LoadCreatingImageCommand
        {
            get
            {
                if (_loadCreatingImage == null)
                    _loadCreatingImage = new RelayCommand(LoadCreatingImage);
                return _loadCreatingImage;
            }
        }
        private ProgressDialogController progressDialog = null;
        private IConnectionWebApi _currentWebApi;
        private IDialogCoordinator _dialogCoordinator;

        public ListGameViewModel(IConnectionWebApi webApi, IDialogCoordinator dialogCoordinator)
        {
            _currentWebApi = webApi;
            _dialogCoordinator = dialogCoordinator;
            AvailableGenres = new ObservableCollection<Genre>(_currentWebApi.GetGenres());
            AvailableGenres.Add(default(Genre));
            AvailablePlatforms = new ObservableCollection<Platform>(_currentWebApi.GetPlatforms());
            AvailablePlatforms.Add(default(Platform));
            BindingOperations.EnableCollectionSynchronization(GamesDto, _lock);
            _mapper = new MapperConfiguration(MapperGameImport).CreateMapper();
        }
        
        private void MapperGameImport(IMapperConfiguration map)
        {
            map.CreateMap<Game, GameDto>().ReverseMap();
        }

        public async Task LoadFromuPlayAgain()
        {
            List<Game> gs = null;
            Game GameFilter = new Game();
            await Task.Factory.StartNew(async () =>
            {
                GiochiCaricati = string.Empty;
                await App.Current.Dispatcher.BeginInvoke((Action)async delegate ()
                {
                    progressDialog = await _dialogCoordinator.ShowProgressAsync(this, "Attendi...", "Attendi mentre carico la lista dei giochi", false, new MetroDialogSettings()
                    {
                        ColorScheme = MetroDialogColorScheme.Accented
                    });
                    progressDialog.SetIndeterminate();
                });

                if (GamesDto.Count > 0)
                    GamesDto.Clear();
                
                // Create oData Filter and count
                if (CurrentPlatform != default(Platform))
                    GameFilter.PlatformId = CurrentPlatform.PlatformId;
                if (CurrentGenre != default(Genre))
                    GameFilter.GenreId = CurrentGenre.GenreId;

                gs = (await _currentWebApi.GetGameIds(GameFilter)).ToList();

                await App.Current.Dispatcher.BeginInvoke((Action)async delegate ()
                {
                    await progressDialog.CloseAsync();
                });

                int numGiochi = gs.Count;
                int giocoCorrente = 0;
                double percentageToIncrement = (double)100 / numGiochi;
                ProgressBarValue = 0;

                // apply oData Filter and load data
                gs.ForEach(async currentGameSummary =>
                {
                    try
                    {
                        GameDto result = _mapper.Map<GameDto>(await _currentWebApi.GetGameById(new Game() { GameId = currentGameSummary.GameId }));
                        if(result != null)
                            await App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                             {
                                 GamesDto.Add(result);
                             });
                    }
                    catch (Exception ex)
                    {
                        string a = ex.Message;
                    }
                    finally
                    {
                        giocoCorrente++;
                        GiochiCaricati = string.Format("{0} / {1}", giocoCorrente, numGiochi);
                        ProgressBarValue += percentageToIncrement;
                        if (numGiochi == giocoCorrente)
                            // Apro il popup di segnalazione. Finita importazione
                            await App.Current.Dispatcher.BeginInvoke((Action)async delegate ()
                            {
                                await _dialogCoordinator.ShowMessageAsync(this, "Caricamento completato...", "Puoi modificare i giochi o crearne di nuovi", MessageDialogStyle.Affirmative, new MetroDialogSettings()
                                {
                                    ColorScheme = MetroDialogColorScheme.Accented,
                                    AnimateShow = true
                                });
                            });
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
            IsGameDtoEditMode = false;
            IsGameDtoCreateMode = false;
        }

        public async Task InsertNewGame()
        {
            CreatedGameDto.GenreId = CreatedGameDto.Genre.GenreId;
            CreatedGameDto.PlatformId = CreatedGameDto.Platform.PlatformId;
            await _currentWebApi.InsertGame(_mapper.Map<Game>(CreatedGameDto));
            CreatedGameDto = null;
            IsGameDtoEditMode = false;
            IsGameDtoCreateMode = false;
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
            IsGameDtoEditMode = false;
            IsGameDtoCreateMode = true;
            CreatedGameDto = new GameDto()
            {
                Genre = new Genre(),
                Platform = new Platform(),
                RegistrationDate = DateTime.Now
            };
        }

        public void LoadEditImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();            
            fileDialog.ShowDialog();

            if (fileDialog.ShowDialog().HasValue && fileDialog.ShowDialog().Value)
            {
                SelectedGameDto.Image = File.ReadAllBytes(fileDialog.FileName);
                SelectedGameDto.ImageThumb = SelectedGameDto.Resize();
                RaisePropertyChanged("SelectedGameDto");
            }
        }

        public void LoadCreatingImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            bool? result = fileDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                CreatedGameDto.Image = File.ReadAllBytes(fileDialog.FileName);
                CreatedGameDto.ImageThumb = CreatedGameDto.Resize();
                RaisePropertyChanged("CreatedGameDto");
            }
        }
    }
}
