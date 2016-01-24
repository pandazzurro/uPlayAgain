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
        public bool Save { get; set; }
        public bool Delete { get; set; }
        public bool Update { get; set; }
        public bool Insert { get; set; }

        public GameDto()
        {
            
        }
    }
}
