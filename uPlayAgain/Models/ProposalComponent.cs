using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uPlayAgain.Models
{
    public class ProposalComponent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProposalComponentId { get; set; } 
               
        public int LibraryComponentId { get; set; }
        [ForeignKey("LibraryComponentId")]
        public virtual LibraryComponent LibraryComponents { get; set; }
    }
}