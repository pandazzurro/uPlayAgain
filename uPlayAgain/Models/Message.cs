using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uPlayAgain.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }
        public string MessageText { get; set; }
        public DateTimeOffset MessageDate { get; set; }

        public virtual User UserReceiving { get; set; }

        public virtual User UserProponent { get; set; }
    }
}