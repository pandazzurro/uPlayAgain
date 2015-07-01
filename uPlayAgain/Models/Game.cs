using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace uPlayAgain.Models
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GameId { get; set; }
      
        public string ShortName { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public string Title { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public string Description { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public int? ImportId { get; set; }
        public byte[] Image { get; set; }

        // Foreign key
        public string GenreId { get; set; }
        [ForeignKey("GenreId")]
        public virtual Genre Genre { get; set; }
        public string PlatformId { get; set; }
        [ForeignKey("PlatformId")]
        public virtual Platform Platform { get; set; }
    }
}