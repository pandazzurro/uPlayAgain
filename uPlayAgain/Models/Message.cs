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
        public bool IsAlreadyReadProponent { get; set; }
        public bool IsAlreadyReadReceiving { get; set; }
        public bool IsAlreadyDeleteProponent { get; set; }
        public bool IsAlreadyDeleteReceiving { get; set; }
        public string MessageObject { get; set; }


        public string UserReceiving_Id { get; set; }
        public User UserReceiving { get; set; }

        public string UserProponent_Id { get; set; }
        public User UserProponent { get; set; }
    }
}