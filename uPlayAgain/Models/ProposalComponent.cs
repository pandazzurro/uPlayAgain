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
        public LibraryComponent LibraryComponents { get; set; }

        public int ProposalId { get; set; }
        [ForeignKey("ProposalId")]
        public Proposal Proposal { get; set; }


        [Required]
        public string UserOwnerId { get; set; }
        [ForeignKey("UserOwnerId")]
        public virtual User UserOwner { get; set; }
    }
}