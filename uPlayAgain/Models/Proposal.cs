using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int ProposalComponentId { get; set; }
        [ForeignKey("ProposalComponentId")]
        public virtual IList<ProposalComponent> ProposalComponents { get; set; }
        //public int UserLastChangesId { get; set; }
        //[ForeignKey("UserLastChangesId")]
        public virtual User UserLastChanges { get; set; }

        public Proposal()
        {
            ProposalComponents = new List<ProposalComponent>();
        }
    }
}