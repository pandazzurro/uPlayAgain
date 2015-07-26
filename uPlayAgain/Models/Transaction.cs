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
        
        public ICollection<Proposal> Proposals { get; set; }

        public TransactionStatus TransactionStatus { get; set; }

        public string UserReceiving_Id { get; set; }
        public User UserReceiving { get; set; }

        public string UserProponent_Id { get; set; }
        public User UserProponent { get; set; }
    }
}