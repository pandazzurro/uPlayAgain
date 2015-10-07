using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uPlayAgain.Models
{
    public class TransactionResponse
    {
        public string UserId { get; set; }
        public DateTimeOffset LastChange { get; set; }
        public ProposalStatus MyStatus { get; set; }
        public ProposalStatus TheirStatus { get; set; }
        public IList<LibraryComponent> MyItems { get; set; }
        public IList<LibraryComponent> TheirItems { get; set; }
    }
}