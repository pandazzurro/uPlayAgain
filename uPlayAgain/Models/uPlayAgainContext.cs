using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace uPlayAgain.Models
{
    public class uPlayAgainContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public uPlayAgainContext() : base("name=uPlayAgainContext")
        {
            this.Configuration.ProxyCreationEnabled = false;            
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

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<GameLanguage> GameLanguages { get; set; }
        public DbSet<Library> Librarys { get; set; }
        public DbSet<LibraryComponent> LibraryComponents { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<ProposalComponent> ProposalComponents { get; set; }
        public DbSet<ProposalStatus> ProposalStaus { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionStatus> TransactionStatus { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
