using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uPlayAgain.Models
{
    public class MessageCountResponse
    {
        public int Incoming { get; set; }
        public int Outgoing { get; set; }
        public int Transactions { get; set; }
        public int LibrariesComponents { get; set; }
    }
}
