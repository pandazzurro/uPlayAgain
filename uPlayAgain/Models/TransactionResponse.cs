using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uPlayAgain.Models
{
    public class TransactionResponse
    {
        public string UserId;
        public DateTimeOffset LastChange;
        public ProposalStatus MyStatus;
        public ProposalStatus TheirStatus;
        public List<LibraryComponent> MyItems;
        public List<LibraryComponent> TheirItems;
    }
}