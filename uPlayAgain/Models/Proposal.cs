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
        public DateTimeOffset? DateEnd { get; set; }
        public bool Direction { get; set; }

        public string ProposalText { get; set; }
        public string ProposalObject { get; set; }
      
        public int TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
        
        public string UserLastChanges_Id { get; set; }
        [ForeignKey("UserLastChanges_Id")]
        public virtual User UserLastChanges { get; set; }

        #region NavigationProperty
        public ICollection<ProposalComponent> ProposalComponents { get; set; }
        #endregion

        public Proposal()
        {
            ProposalComponents = new HashSet<ProposalComponent>();
        }
    }
}