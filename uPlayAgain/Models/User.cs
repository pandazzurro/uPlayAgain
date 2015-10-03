using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using uPlayAgain.Converters;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using uPlayAgain.Utilities;

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

        [Key]
        public override string Id
        {
            get { return base.Id; }
            set { base.Id = value; }
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }        

        [Required]
        public override string UserName { get; set; }

        [JsonIgnore]
        public byte[] Image { get; set; }
        [JsonIgnore]
        public string Provider { get; set; }
        [JsonConverter(typeof(DbGeographyConverter))]
        [JsonIgnore]
        public DbGeography PositionUser { get; set; }
        [JsonIgnore]
        public DateTimeOffset LastLogin { get; set; }

        #region [ Overrides ]
        [DataType(DataType.EmailAddress)]
        [JsonIgnore]
        public override string Email { get; set; }
        
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

        public Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager, User user)
        {
            return manager.CreateIdentityAsync(user, "UPlayAgainAuth");
        }
    }
}