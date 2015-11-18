using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uPlayAgain.Dto
{
    public class GameDto
    {
        public int GameId { get; set; }

        public string ShortName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? ImportId { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        // Thum
        public byte[] Image { get; set; }
        public string GenreId { get; set; }
        public string PlatformId { get; set; }        
    }
}