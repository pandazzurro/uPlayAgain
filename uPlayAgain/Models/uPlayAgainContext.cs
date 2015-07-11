using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using uPlayAgain.Entities;

namespace uPlayAgain.Models
{
    public class uPlayAgainContext : IdentityDbContext<User>
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<GameLanguage> GameLanguages { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<LibraryComponent> LibraryComponents { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<ProposalComponent> ProposalComponents { get; set; }
        public DbSet<ProposalStatus> ProposalStaus { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionStatus> TransactionStatus { get; set; }
        //public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public uPlayAgainContext() : base("name=uPlayAgainContext")
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;            
            // Log query DB
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
               .Entity<Transaction>()
               .HasRequired<User>(p => p.UserProponent)
               .WithMany()
               .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<Transaction>()
                .HasRequired<User>(p => p.UserReceiving)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<Proposal>()
                .HasRequired<User>(p => p.UserLastChanges)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder
              .Entity<Message>()
              .HasRequired<User>(p => p.UserProponent)
              .WithMany()
              .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<Message>()
                .HasRequired<User>(p => p.UserReceiving)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Library>()
                .HasMany(t => t.LibraryComponents)
                .WithRequired(t => t.Library)
                .HasForeignKey(p => p.LibraryId);

            modelBuilder.Entity<Transaction>()
                .HasRequired(x => x.Proposal)
                .WithRequiredDependent(x => x.Transaction);

            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<IdentityUser>()
            //   .ToTable("User", "dbo")
            //   .Property(p => p.Id)
            //   .HasColumnName("User_Id");
            modelBuilder.Entity<IdentityUser>().ToTable("Users");
            modelBuilder.Entity<User>().ToTable("Users").HasKey(x => x.Id);
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");            
        }

    }
}
