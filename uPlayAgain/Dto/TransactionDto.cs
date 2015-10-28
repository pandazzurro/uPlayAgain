using System;
using System.Collections.Generic;
using uPlayAgain.Models;

namespace uPlayAgain.Dto
{
    public class TransactionDto
    {
        public TransactionDto()
        {
            MyItems = new HashSet<LibraryComponent>();
            TheirItems = new HashSet<LibraryComponent>();
        }
        public int TransactionId { get; set; }
        public int TheirLibraryId { get; set; }
        public Proposal Proposal { get; set; }
        public bool Direction { get; set; }        
        public string UserOwnerId { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset LastChange { get; set; }
        public ProposalStatus MyStatus { get; set; }
        public ProposalStatus TheirStatus { get; set; }
        public ICollection<LibraryComponent> MyItems { get; set; }
        public ICollection<LibraryComponent> TheirItems { get; set; }
    }
}