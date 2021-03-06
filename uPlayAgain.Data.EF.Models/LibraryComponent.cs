﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace uPlayAgain.Data.EF.Models
{
    public class LibraryComponent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LibraryComponentId { get; set; }

        //Chiavi esterne
        public int GameId { get; set; }
        [ForeignKey("GameId")]
        public virtual Game Games { get; set; }

        public string Note { get; set; }

        public bool IsExchangeable { get; set; }

        public bool IsDeleted { get; set; }

        public int LibraryId { get; set; }    
        [JsonIgnore]
        [XmlIgnore]
        public virtual Library Library { get; set; }

        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; }
        public int GameLanguageId { get; set; }
        [ForeignKey("GameLanguageId")]
        public virtual GameLanguage GameLanguage { get; set; }

        #region NavigationProperty
        #endregion
    }
}