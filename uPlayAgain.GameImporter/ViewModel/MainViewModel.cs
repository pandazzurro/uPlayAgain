using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System;

namespace uPlayAgain.GameImporter.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region [ Private ViewModel ]
        private ViewModelBase _currentViewModel;
        private ViewModelBase _importGameViewModel;
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
        public ICommand ListGameViewCommand { get; private set; }
        private ICommand _collapseCommand;
        public ICommand CollapseViewCommand
        {
            get
            {
                if (_collapseCommand == null)
                    _collapseCommand = new RelayCommand(ExecuteCollapseBarViewCommand);
                return _collapseCommand;
            }
        }
        public string CollapseButtonImage
        {
            get
            {
                if(IsMinimized)
                    return "/uPlayAgain.GameImporter;component/Image/up.png";
                return "/uPlayAgain.GameImporter;component/Image/down.png";
            }
        }
        private bool _isMinimized;
        public bool IsMinimized
        {
            get { return _isMinimized; }
            set
            {
                _isMinimized = value;
                RaisePropertyChanged("IsMinimized");
                RaisePropertyChanged("CollapseButtonImage");
                RaisePropertyChanged("RibbonHeight");                
            }
        }

        public int RibbonHeight
        {
            get
            {
                if(IsMinimized)
                    return 48;
                return 135;
            }
        }

        #endregion

        #region [ Constructor ]
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ImportGameViewModel ip, ListGameViewModel lg)
        {
            _importGameViewModel = ip;
            _listGameViewModel = lg;

            // Aggangio L'ICommand alla sua implementazione.
            ImportGameViewCommand = new RelayCommand(() => ExecuteImportGameViewCommand());
            ListGameViewCommand = new RelayCommand(() => ExecuteListGameViewCommand());
        }
        #endregion

        #region [ Command Implementation ]

        private void ExecuteCollapseBarViewCommand()
        {
            IsMinimized = !IsMinimized;
        }

        private void ExecuteImportGameViewCommand()
        {
            CurrentViewModel = _importGameViewModel;
        }
        private void ExecuteListGameViewCommand()
        {
            CurrentViewModel = _listGameViewModel;
        }
        #endregion
    }
}