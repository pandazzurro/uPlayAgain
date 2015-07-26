using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using uPlayAgain.Converters;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace uPlayAgain.Models
{
    public class User : IdentityUser
    {
        public User() : base()
        { }

        public User(string userName) 
            : base(userName)
        { }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }        

        [Required]
        public override string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public byte[] Image { get; set; }
        public string Provider { get; set; }
        [DataType(DataType.EmailAddress)]
        public override string Email { get; set; }

        
        [JsonConverter(typeof(DbGeographyConverter))]
        public DbGeography PositionUser { get; set; }
        public DateTimeOffset LastLogin { get; set; }


        #region NavigationProperty
        public ICollection<Transaction> TransactionsProponent { get; set; }
        public ICollection<Transaction> TransactionsReceiving { get; set; }
        public ICollection<Library> Libraries { get; set; }
        public ICollection<Message> MessagesIn { get; set; }
        public ICollection<Message> MessagesOut { get; set; }
        #endregion

    }
}