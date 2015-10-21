using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using uPlayAgain.Models;

namespace uPlayAgain.Dto
{
    public class LibraryComponentDto
    {
        public string UserId { get; set; }
        public LibraryComponent LibraryComponents { get; set; }
        public Status Status { get; set; }
        public GameLanguage GameLanguage { get; set; }
        public Game Games { get; set; }
    }
}