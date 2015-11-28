using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uPlayAgain.Data.EF.Models
{
    public class Library
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LibraryId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        //public int LibraryComponentId { get; set; }
        public virtual ICollection<LibraryComponent> LibraryComponents { get; set; }

        public Library()
        {
            LibraryComponents = new HashSet<LibraryComponent>();
        }
    }
}