using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace uPlayAgain.GameImporter.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region [ Private ViewModel ]
        private ViewModelBase _currentViewModel;
        private ViewModelBase _importGameViewModel;
        private ViewModelBase _createGameViewModel;
        private ViewModelBase _listGameViewModel;
        #endregion

        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                if (_currentViewModel == value)
                    return;
                _currentViewModel = value;
                RaisePropertyChanged("CurrentViewModel");
            }
        }

        #region [ Command ]
        public ICommand ImportGameViewCommand { get; private set; }
        public ICommand CreateGameViewCommand { get; private set; }
        public ICommand ListGameViewCommand { get; private set; }
        #endregion

        #region [ Constructor ]
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ImportGameViewModel ip, ListGameViewModel lg, CreateGameViewModel cg)
        {
            _importGameViewModel = ip;
            _listGameViewModel = lg;
            _createGameViewModel = cg;

            // Associo il primo ViewModel alla finestra corrente.
            CurrentViewModel = _importGameViewModel;

            // Aggangio L'ICommand alla sua implementazione.
            ImportGameViewCommand = new RelayCommand(() => ExecuteImportGameViewCommand());
            CreateGameViewCommand = new RelayCommand(() => ExecuteCreateGameViewCommand());
            ListGameViewCommand = new RelayCommand(() => ExecuteListGameViewCommand());
        }
        #endregion

        #region [ Command Implementation ]
        private void ExecuteImportGameViewCommand()
        {
            CurrentViewModel = _importGameViewModel;
        }
        private void ExecuteCreateGameViewCommand()
        {
            CurrentViewModel = _createGameViewModel;
        }
        private void ExecuteListGameViewCommand()
        {
            CurrentViewModel = _listGameViewModel;
        }
        #endregion
    }
}