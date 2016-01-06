using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uPlayAgain.GameImporter.ViewModel
{
    public class ImportGameViewModel : ViewModelBase
    {
        public ObservableCollection <string> GamesList { get; set; }
        public ImportGameViewModel()
        {
             GamesList = new ObservableCollection<string> { "A", "B", "C" };
        }
    }
}
