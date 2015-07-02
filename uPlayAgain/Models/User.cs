using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity.Spatial;
using System.ComponentModel.DataAnnotations.Schema;

namespace uPlayAgain.Models
{
    public class User
    {
        public User()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public byte[] Image { get; set; }
        public string Provider { get; set; }
        public DbGeography PositionUser { get; set; }   
        public DateTimeOffset LastLogin { get; set; }    
    }
}