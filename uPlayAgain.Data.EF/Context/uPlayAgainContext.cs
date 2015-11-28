using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using uPlayAgain.Data.EF.Models;

namespace uPlayAgain.Data.EF.Context
{
    public class uPlayAgainContext : IdentityDbContext<User>
    {
        private NLog.Logger _log = NLog.LogManager.GetLogger("uPlayAgain");

        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<GameLanguage> GameLanguages { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<LibraryComponent> LibraryComponents { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<ProposalComponent> ProposalComponents { get; set; }        
        public DbSet<Status> Status { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public uPlayAgainContext() : base("name=uPlayAgainContext")
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;            
            // Log query DB
            this.Database.Log = s => _log.Info(s);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // FK User
            modelBuilder.Entity<User>()
                .HasMany(c => c.TransactionsProponent)
                .WithRequired(d => d.UserProponent)
                .HasForeignKey(d => d.UserProponent_Id);

            modelBuilder.Entity<User>()
                .HasMany(c => c.TransactionsReceiving)
                .WithRequired(d => d.UserReceiving)
                .HasForeignKey(d => d.UserReceiving_Id);

            modelBuilder.Entity<User>()
               .HasMany(c => c.Libraries)
               .WithRequired(d => d.User)
               .HasForeignKey(d => d.UserId);

            modelBuilder.Entity<User>()
               .HasMany(c => c.MessagesOut)
               .WithRequired(d => d.UserProponent)
               .HasForeignKey(d => d.UserProponent_Id);

            modelBuilder.Entity<User>()
                .HasMany(c => c.MessagesIn)
                .WithRequired(d => d.UserReceiving)
                .HasForeignKey(d => d.UserReceiving_Id);

            // FK Transaction
            modelBuilder.Entity<Transaction>()
                .HasMany(c => c.Proposals)
                .WithRequired(d => d.Transaction)
                .HasForeignKey(d => d.TransactionId);
            
            modelBuilder.Entity<Proposal>()
                .HasMany(e => e.ProposalComponents)
                .WithRequired(e => e.Proposal)
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


            base.OnModelCreating(modelBuilder);
            modelBuilder = UserReference(modelBuilder);
        }

        private DbModelBuilder UserReference(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUser>().ToTable("Users");
            modelBuilder.Entity<User>().ToTable("Users").HasKey(x => x.Id);
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            return modelBuilder;
        }

        public static uPlayAgainContext Create()
        {
            return new uPlayAgainContext();
        }

    }
}
