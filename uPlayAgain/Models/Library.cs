﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace uPlayAgain.Models
{
    public class Library
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LibraryId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int LibraryComponentId { get; set; }
        //[ForeignKey("LibraryComponentId")]
        public virtual IList<LibraryComponent> LibraryComponents { get; set; }

        public Library()
        {
            LibraryComponents = new List<LibraryComponent>();
        }
    }
}