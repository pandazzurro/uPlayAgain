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
        {
            TransactionsProponent = new HashSet<Transaction>();
            TransactionsReceiving = new HashSet<Transaction>();
            Libraries = new HashSet<Library>();
            MessagesIn = new HashSet<Message>();
            MessagesOut = new HashSet<Message>();
            Feedbacks = new HashSet<Feedback>();
            Proposals = new HashSet<Proposal>();
        }
        
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

        #region OverrideIdentityUser
        [JsonIgnore]
        public override int AccessFailedCount
        {
            get
            {
                return base.AccessFailedCount;
            }

            set
            {
                base.AccessFailedCount = value;
            }
        }

        [JsonIgnore]
        public override ICollection<IdentityUserClaim> Claims
        {
            get
            {
                return base.Claims;
            }
        }

        [JsonIgnore]
        public override bool EmailConfirmed
        {
            get
            {
                return base.EmailConfirmed;
            }

            set
            {
                base.EmailConfirmed = value;
            }
        }

        [Key]
        public override string Id
        {
            get
            {
                return base.Id;
            }

            set
            {
                base.Id = value;
            }
        }

        [JsonIgnore]
        public override bool LockoutEnabled
        {
            get
            {
                return base.LockoutEnabled;
            }

            set
            {
                base.LockoutEnabled = value;
            }
        }

        [JsonIgnore]
        public override DateTime? LockoutEndDateUtc
        {
            get
            {
                return base.LockoutEndDateUtc;
            }

            set
            {
                base.LockoutEndDateUtc = value;
            }
        }

        [JsonIgnore]
        public override ICollection<IdentityUserLogin> Logins
        {
            get
            {
                return base.Logins;
            }
        }

        [JsonIgnore]
        public override string PasswordHash
        {
            get
            {
                return base.PasswordHash;
            }

            set
            {
                base.PasswordHash = value;
            }
        }

        [JsonIgnore]
        public override string PhoneNumber
        {
            get
            {
                return base.PhoneNumber;
            }

            set
            {
                base.PhoneNumber = value;
            }
        }

        [JsonIgnore]
        public override bool PhoneNumberConfirmed
        {
            get
            {
                return base.PhoneNumberConfirmed;
            }

            set
            {
                base.PhoneNumberConfirmed = value;
            }
        }

        [JsonIgnore]
        public override ICollection<IdentityUserRole> Roles
        {
            get
            {
                return base.Roles;
            }
        }

        [JsonIgnore]
        public override string SecurityStamp
        {
            get
            {
                return base.SecurityStamp;
            }

            set
            {
                base.SecurityStamp = value;
            }
        }

        [JsonIgnore]
        public override bool TwoFactorEnabled
        {
            get
            {
                return base.TwoFactorEnabled;
            }

            set
            {
                base.TwoFactorEnabled = value;
            }
        }
        #endregion


        #region NavigationProperty
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Transaction> TransactionsProponent { get; set; }
        public virtual ICollection<Transaction> TransactionsReceiving { get; set; }
        public virtual ICollection<Library> Libraries { get; set; }
        public virtual ICollection<Message> MessagesIn { get; set; }
        public virtual ICollection<Message> MessagesOut { get; set; }
        public virtual ICollection<Proposal> Proposals { get; set; }
        #endregion

    }
}