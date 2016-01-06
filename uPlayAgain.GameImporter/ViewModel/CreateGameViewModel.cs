using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPlayAgain.GameImporter.Service;

namespace uPlayAgain.GameImporter.ViewModel
{
    public class CreateGameViewModel : ViewModelBase
    {
        private IConnectionWebApi _connectionWebApi { get; set; }

        public CreateGameViewModel(IConnectionWebApi webApi)
        {
            _connectionWebApi = webApi;
        }
    }
}
