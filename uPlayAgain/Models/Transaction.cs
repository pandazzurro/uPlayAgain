using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uPlayAgain.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }
        
        public TransactionStatus TransactionStatus { get; set; }

        [Required]
        [StringLength(128)]
        public string UserReceiving_Id { get; set; }
        public User UserReceiving { get; set; }

        [Required]
        [StringLength(128)]
        public string UserProponent_Id { get; set; }
        public User UserProponent { get; set; }

        #region NavigationProperty
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Proposal> Proposals { get; set; }
        #endregion

        public Transaction()
        {
            Feedbacks = new HashSet<Feedback>();
            Proposals = new HashSet<Proposal>();
        }
    }
}