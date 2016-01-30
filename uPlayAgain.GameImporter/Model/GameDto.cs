using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPlayAgain.Data.EF.Models;

namespace uPlayAgain.GameImporter.Model
{
    public class GameDto : Game
    {
        public bool IsSelected { get; set; }

        public GameDto()
        {
            
        }
    }
}
