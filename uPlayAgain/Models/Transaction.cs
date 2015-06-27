using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace uPlayAgain.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }

        //public int UserReceivingId { get ;set; }
        //[ForeignKey("UserReceivingId")]
        public virtual User UserReceiving { get; set; }

        //public int UserProponentId { get; set; }
        //[ForeignKey("UserProponentId")]
        public virtual User UserProponent { get; set; }
    }
}