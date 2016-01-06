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
        private IConnectionWebApi _connectionWebApi { get; set; }        

        public ObservableCollection<Game> GameList { get; set; }

        public ListGameViewModel(IConnectionWebApi webApi)
        {
            _connectionWebApi = webApi;
        }
    }
}
