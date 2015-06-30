using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using uPlayAgain.Models;

namespace uPlayAgain.Dto
{
    public class SearchGame
    {
        public virtual Status Status { get; set; }
        public virtual GameLanguage GameLanguage { get; set; }
        public virtual Genre Genre { get; set; }
        public virtual Platform Platform { get; set; }
        public virtual double? Distance { get; set; }        
    }
}