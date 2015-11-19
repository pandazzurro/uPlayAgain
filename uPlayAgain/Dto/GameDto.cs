using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using uPlayAgain.Models;

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

        public GameDto() { }

        public GameDto(Game g)
        {
            this.GameId = g.GameId;
            this.ShortName = g.ShortName;
            this.Title = g.Title;
            this.Description = g.Description;
            this.ImportId = g.ImportId;
            this.RegistrationDate = g.RegistrationDate;
            this.Image = g.ImageThumb;
            this.GenreId = g.GenreId;
            this.PlatformId = g.PlatformId;
        } 
    }
}