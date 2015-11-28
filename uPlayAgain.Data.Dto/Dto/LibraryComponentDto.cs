using uPlayAgain.Data.EF.Models;

namespace uPlayAgain.Data.Dto
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