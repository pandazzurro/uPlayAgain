﻿using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        private ObservableCollection<GameDto> _gamesDto;
        public ObservableCollection<GameDto> GamesDto
        {
            get
            {
                if (_gamesDto == null)
                    _gamesDto = new ObservableCollection<GameDto>();
                return _gamesDto;
            }
            set
            {
                _gamesDto = value;
                RaisePropertyChanged("GamesDto");
            }
        }
        private ObservableCollection<GameDto> _gamesSearchDto;
        public ObservableCollection<GameDto> GamesSearchDto
        {
            get
            {
                if (_gamesSearchDto == null)
                    _gamesSearchDto = new ObservableCollection<GameDto>();
                return _gamesSearchDto;
            }
            set
            {
                _gamesSearchDto = value;
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

        private string _titleSearch;
        public string TitleSearch
        {
            get { return _titleSearch; }
            set
            {
                _titleSearch = value;
                if (GamesDto.Count > 0)
                {
                    if (string.IsNullOrEmpty(_titleSearch))
                        GamesSearchDto = new ObservableCollection<GameDto>(GamesDto);
                    else
                        GamesDto = new ObservableCollection<GameDto>(GamesDto.Where(x => x.Title.Contains(TitleSearch)));
                }
            }
        }
        private bool _isEnableTitleSearch;

        public bool IsReadOnlyTitleSearch
        {
            get { return _isEnableTitleSearch; }
            set
            {
                _isEnableTitleSearch = value;
                RaisePropertyChanged("IsReadOnlyTitleSearch");
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
                if (_loadTheGameDbGameCommand == null)
                    _loadTheGameDbGameCommand = new RelayCommand(async () => await LoadTheGameDbGame());
                return _loadTheGameDbGameCommand;
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
        private ProgressDialogController progressDialog = null;
        private IConnectionWebApi _currentWebApi;
        private IDialogCoordinator _dialogCoordinator;

        public ImportGameViewModel(IConnectionWebApi api, IDialogCoordinator dialogCoordinator)
        {
            _currentWebApi = api;
            _dialogCoordinator = dialogCoordinator;

            AvailableGenres = new ObservableCollection<Genre>(_currentWebApi.GetGenres());
            AvailableGenres.Add(new Genre { Name = "Tutti", GenreId = "Tutti" });
            AvailablePlatforms = new ObservableCollection<Platform>(_currentWebApi.GetPlatforms());
            AvailablePlatforms.Add(new Platform() { Name = "Tutti", PlatformId = "Tutti"});
            BindingOperations.EnableCollectionSynchronization(GamesDto, _lock);
            _mapper = new MapperConfiguration(MapperGameImport).CreateMapper();
            BindingOperations.EnableCollectionSynchronization(GamesDto, _lock);
            DataInizio = DateTime.Now.AddMonths(-3);
            DataFine = DateTime.Now;
            IsReadOnlyTitleSearch = true;
        }
        
        private void MapperGameImport(IMapperConfiguration map)
        {
            map.CreateMap<Game, GameDto>().ReverseMap();
        }

        public async Task LoadTheGameDbGame()
        {
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
                
                IsReadOnlyTitleSearch = true;
                if (GamesDto.Count > 0)
                    GamesDto.Clear();
                List<GameSummary> gs = new List<GameSummary>();
                if (CurrentPlatform.Name != "Tutti")
                    gs = await _currentWebApi.TheGamesDBGameListByPlatform(DataInizio, DataFine, CurrentPlatform);
                else
                    AvailablePlatforms.Where(x => x.Name != "Tutti").ToList().ForEach(async _currentPlatform =>
                    {
                        gs.AddRange(await _currentWebApi.TheGamesDBGameListByPlatform(DataInizio, DataFine, _currentPlatform));
                    });

                await App.Current.Dispatcher.BeginInvoke((Action)async delegate ()
                {
                    await progressDialog.CloseAsync();
                });

                int numGiochi = gs.Count;
                int giocoCorrente = 0;
                double percentageToIncrement = (double)100 / numGiochi;
                ProgressBarValue = 0;

                gs.ForEach(async currentGameSummary =>
                {
                    try
                    {
                        if (await _currentWebApi.GetGameByFieldSearch(new Game() { ImportId = currentGameSummary.ID }) == default(Game))
                        {
                            GameDto result = _mapper.Map<GameDto>(await _currentWebApi.TheGamesDBGetGameDetails(currentGameSummary));
                            await App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                GamesDto.Add(result);
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        numGiochi--;
                        giocoCorrente--;
                        string a = ex.Message;
                    }
                    finally
                    {
                        giocoCorrente++;
                        GiochiCaricati = string.Format("{0} / {1}", giocoCorrente, numGiochi);
                        ProgressBarValue += percentageToIncrement;
                        if (numGiochi == giocoCorrente)
                        {
                            await App.Current.Dispatcher.BeginInvoke((Action)async delegate ()
                            {
                                await _dialogCoordinator.ShowMessageAsync(this, "Caricamento completato...", "Puoi modificare i giochi prima dell'importazione",  MessageDialogStyle.Affirmative, new MetroDialogSettings()
                                {
                                    ColorScheme = MetroDialogColorScheme.Accented
                                });                                
                            });                            
                            IsReadOnlyTitleSearch = false;
                        }
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
                        await App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            // Rimuovo i giochi già processati
                            GamesDto.Remove(game);
                        });
                    }
                });
            });
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
        /// <summary>
        /// Assurdo workaround! Non funzionava
        /// </summary>
        /// <param name="check"></param>
        public void SelectAllGame(bool check)
        {
            GamesDto.ToList().ForEach(game => game.IsChecked = !check);
            GamesDto = new ObservableCollection<GameDto>(GamesDto);
        }
        public void LoadEditImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            bool? result = fileDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                SelectedGameDto.Image = File.ReadAllBytes(fileDialog.FileName);
                SelectedGameDto.ImageThumb = SelectedGameDto.Resize();
                RaisePropertyChanged("SelectedGameDto");
            }
        }

    }
}
