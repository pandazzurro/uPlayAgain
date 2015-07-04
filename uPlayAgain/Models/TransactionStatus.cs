using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uPlayAgain.Models
{
    public class TransactionStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionStatusId { get; set; }

        public string Description { get; set; }
    }
}