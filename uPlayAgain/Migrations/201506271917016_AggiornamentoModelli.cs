namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AggiornamentoModelli : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Feedbacks",
                c => new
                    {
                        FeedbackId = c.Int(nullable: false, identity: true),
                        TransactionId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Rate = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.FeedbackId)
                .ForeignKey("dbo.Transactions", t => t.TransactionId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.TransactionId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionId = c.Int(nullable: false, identity: true),
                        UserProponent_UserId = c.Int(nullable: false),
                        UserReceiving_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.Users", t => t.UserProponent_UserId)
                .ForeignKey("dbo.Users", t => t.UserReceiving_UserId)
                .Index(t => t.UserProponent_UserId)
                .Index(t => t.UserReceiving_UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Password = c.String(),
                        Image = c.Binary(),
                        Provider = c.String(),
                        PositionUser = c.Geography(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.GameLanguages",
                c => new
                    {
                        GameLanguageId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.GameLanguageId);
            
            CreateTable(
                "dbo.LibraryComponents",
                c => new
                    {
                        LibraryComponentId = c.Int(nullable: false),
                        GameId = c.Int(nullable: false),
                        LibraryId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        GameLanguageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LibraryComponentId)
                .ForeignKey("dbo.GameLanguages", t => t.GameLanguageId, cascadeDelete: true)
                .ForeignKey("dbo.Libraries", t => t.LibraryId, cascadeDelete: true)
                .ForeignKey("dbo.Status", t => t.StatusId, cascadeDelete: true)
                .ForeignKey("dbo.ProposalComponents", t => t.LibraryComponentId)
                .Index(t => t.LibraryComponentId)
                .Index(t => t.LibraryId)
                .Index(t => t.StatusId)
                .Index(t => t.GameLanguageId);
            
            CreateTable(
                "dbo.Libraries",
                c => new
                    {
                        LibraryId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LibraryId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        StatusId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.StatusId);
            
            CreateTable(
                "dbo.ProposalComponents",
                c => new
                    {
                        ProposalId = c.Int(nullable: false, identity: true),
                        LibraryComponentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProposalId);
            
            CreateTable(
                "dbo.Proposals",
                c => new
                    {
                        ProposalId = c.Int(nullable: false, identity: true),
                        DateStart = c.DateTimeOffset(nullable: false, precision: 7),
                        DateEnd = c.DateTimeOffset(nullable: false, precision: 7),
                        Direction = c.Boolean(nullable: false),
                        TransactionId = c.Int(nullable: false),
                        ProposalStatusId = c.Int(nullable: false),
                        UserLastChanges_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProposalId)
                .ForeignKey("dbo.LibraryComponents", t => t.ProposalStatusId, cascadeDelete: true)
                .ForeignKey("dbo.Transactions", t => t.TransactionId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserLastChanges_UserId)
                .Index(t => t.TransactionId)
                .Index(t => t.ProposalStatusId)
                .Index(t => t.UserLastChanges_UserId);
            
            CreateTable(
                "dbo.ProposalStatus",
                c => new
                    {
                        ProposalStatusId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ProposalStatusId);
            
            CreateTable(
                "dbo.TransactionStatus",
                c => new
                    {
                        TransactionStatusId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.TransactionStatusId);
            
            CreateIndex("dbo.Games", "GameId");
            AddForeignKey("dbo.Games", "GameId", "dbo.LibraryComponents", "LibraryComponentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Proposals", "UserLastChanges_UserId", "dbo.Users");
            DropForeignKey("dbo.Proposals", "TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.Proposals", "ProposalStatusId", "dbo.LibraryComponents");
            DropForeignKey("dbo.LibraryComponents", "LibraryComponentId", "dbo.ProposalComponents");
            DropForeignKey("dbo.LibraryComponents", "StatusId", "dbo.Status");
            DropForeignKey("dbo.LibraryComponents", "LibraryId", "dbo.Libraries");
            DropForeignKey("dbo.Libraries", "UserId", "dbo.Users");
            DropForeignKey("dbo.Games", "GameId", "dbo.LibraryComponents");
            DropForeignKey("dbo.LibraryComponents", "GameLanguageId", "dbo.GameLanguages");
            DropForeignKey("dbo.Feedbacks", "UserId", "dbo.Users");
            DropForeignKey("dbo.Feedbacks", "TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.Transactions", "UserReceiving_UserId", "dbo.Users");
            DropForeignKey("dbo.Transactions", "UserProponent_UserId", "dbo.Users");
            DropIndex("dbo.Proposals", new[] { "UserLastChanges_UserId" });
            DropIndex("dbo.Proposals", new[] { "ProposalStatusId" });
            DropIndex("dbo.Proposals", new[] { "TransactionId" });
            DropIndex("dbo.Libraries", new[] { "UserId" });
            DropIndex("dbo.LibraryComponents", new[] { "GameLanguageId" });
            DropIndex("dbo.LibraryComponents", new[] { "StatusId" });
            DropIndex("dbo.LibraryComponents", new[] { "LibraryId" });
            DropIndex("dbo.LibraryComponents", new[] { "LibraryComponentId" });
            DropIndex("dbo.Games", new[] { "GameId" });
            DropIndex("dbo.Transactions", new[] { "UserReceiving_UserId" });
            DropIndex("dbo.Transactions", new[] { "UserProponent_UserId" });
            DropIndex("dbo.Feedbacks", new[] { "UserId" });
            DropIndex("dbo.Feedbacks", new[] { "TransactionId" });
            DropTable("dbo.TransactionStatus");
            DropTable("dbo.ProposalStatus");
            DropTable("dbo.Proposals");
            DropTable("dbo.ProposalComponents");
            DropTable("dbo.Status");
            DropTable("dbo.Libraries");
            DropTable("dbo.LibraryComponents");
            DropTable("dbo.GameLanguages");
            DropTable("dbo.Users");
            DropTable("dbo.Transactions");
            DropTable("dbo.Feedbacks");
        }
    }
}
