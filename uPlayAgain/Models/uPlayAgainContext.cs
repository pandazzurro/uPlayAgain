using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace uPlayAgain.Models
{
    public class uPlayAgainContext : IdentityDbContext<IdentityUser>
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
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        public uPlayAgainContext() : base("name=uPlayAgainContext")
        {
            this.Configuration.ProxyCreationEnabled = false;
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

            base.OnModelCreating(modelBuilder);
        }

    }
}
