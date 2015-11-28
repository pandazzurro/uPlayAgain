using System.Collections.Generic;
using uPlayAgain.Data.EF.Models;

namespace uPlayAgain.Data.Dto
{
    public class SearchGame
    {
        public virtual Status Status { get; set; }
        public virtual GameLanguage GameLanguage { get; set; }
        public virtual Genre Genre { get; set; }
        public virtual Platform Platform { get; set; }
        public virtual GameDto Game { get; set; }
        public virtual User User { get; set; }
        public virtual LibraryComponent LibraryComponent { get; set; }
        public virtual double? Distance { get; set; }                
    }

    public class SearchGames
    {
        public IList<SearchGame> SearchGame { get; set; }
        public int Count { get; set; }
    }
}