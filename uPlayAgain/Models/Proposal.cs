using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace uPlayAgain.Models
{
    public class Proposal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProposalId { get; set; }
        
        public DateTimeOffset DateStart { get; set; }
        public DateTimeOffset DateEnd { get; set; }
        public bool Direction { get; set; }
        
        public int TransactionId { get; set; }
        [ForeignKey("TransactionId")]
        public virtual Transaction Transaction { get; set; }
        public int ProposalStatusId { get; set; }
        [ForeignKey("ProposalStatusId")]
        public virtual LibraryComponent LibraryComponent { get; set; }
        //public int UserLastChangesId { get; set; }
        //[ForeignKey("UserLastChangesId")]
        public virtual User UserLastChanges { get; set; }
    }
}