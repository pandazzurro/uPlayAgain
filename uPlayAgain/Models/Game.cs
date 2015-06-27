using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uPlayAgain.Models
{
    public class Game
    {
        public int GameId { get; set; }
      
        public string ShortName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // Foreign key
        public string GenreName { get; set; }
        [ForeignKey("GenreName")]
        public virtual Genre Genre { get; set; }
        public string PlatformName { get; set; }
        [ForeignKey("PlatformName")]
        public virtual Platform Platform { get; set; }
    }
}