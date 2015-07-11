namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Start : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Secret = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        ApplicationType = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        AllowedOrigin = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
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
                        UserName = c.String(nullable: false),
                        Password = c.String(nullable: false, maxLength: 100),
                        ConfirmPassword = c.String(),
                        Image = c.Binary(),
                        Provider = c.String(),
                        Email = c.String(),
                        PositionUser = c.Geography(),
                        LastLogin = c.DateTimeOffset(nullable: false, precision: 7),
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
                "dbo.Games",
                c => new
                    {
                        GameId = c.Int(nullable: false, identity: true),
                        ShortName = c.String(),
                        Title = c.String(),
                        Description = c.String(),
                        ImportId = c.Int(),
                        Image = c.Binary(),
                        GenreId = c.String(maxLength: 128),
                        PlatformId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.GameId)
                .ForeignKey("dbo.Genres", t => t.GenreId)
                .ForeignKey("dbo.Platforms", t => t.PlatformId)
                .Index(t => t.GenreId)
                .Index(t => t.PlatformId);
            
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        GenreId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.GenreId);
            
            CreateTable(
                "dbo.Platforms",
                c => new
                    {
                        PlatformId = c.String(nullable: false, maxLength: 128),
                        Class = c.String(),
                        Name = c.String(),
                        ImportId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PlatformId);
            
            CreateTable(
                "dbo.Libraries",
                c => new
                    {
                        LibraryId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        LibraryComponentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LibraryId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.LibraryComponents",
                c => new
                    {
                        LibraryComponentId = c.Int(nullable: false, identity: true),
                        GameId = c.Int(nullable: false),
                        LibraryId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        GameLanguageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LibraryComponentId)
                .ForeignKey("dbo.GameLanguages", t => t.GameLanguageId, cascadeDelete: true)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .ForeignKey("dbo.Status", t => t.StatusId, cascadeDelete: true)
                .ForeignKey("dbo.Libraries", t => t.LibraryId, cascadeDelete: true)
                .Index(t => t.GameId)
                .Index(t => t.LibraryId)
                .Index(t => t.StatusId)
                .Index(t => t.GameLanguageId);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        StatusId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.StatusId);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        MessageText = c.String(),
                        MessageDate = c.DateTimeOffset(nullable: false, precision: 7),
                        UserProponent_UserId = c.Int(nullable: false),
                        UserReceiving_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.Users", t => t.UserProponent_UserId)
                .ForeignKey("dbo.Users", t => t.UserReceiving_UserId)
                .Index(t => t.UserProponent_UserId)
                .Index(t => t.UserReceiving_UserId);
            
            CreateTable(
                "dbo.ProposalComponents",
                c => new
                    {
                        ProposalComponentId = c.Int(nullable: false, identity: true),
                        LibraryComponentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProposalComponentId)
                .ForeignKey("dbo.LibraryComponents", t => t.LibraryComponentId, cascadeDelete: true)
                .ForeignKey("dbo.Proposals", t => t.ProposalComponentId)
                .Index(t => t.ProposalComponentId)
                .Index(t => t.LibraryComponentId);
            
            CreateTable(
                "dbo.Proposals",
                c => new
                    {
                        ProposalId = c.Int(nullable: false, identity: true),
                        DateStart = c.DateTimeOffset(nullable: false, precision: 7),
                        DateEnd = c.DateTimeOffset(nullable: false, precision: 7),
                        Direction = c.Boolean(nullable: false),
                        TransactionId = c.Int(nullable: false),
                        ProposalComponentId = c.Int(nullable: false),
                        UserLastChanges_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProposalId)
                .ForeignKey("dbo.Transactions", t => t.TransactionId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserLastChanges_UserId)
                .Index(t => t.TransactionId)
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
                "dbo.RefreshTokens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Subject = c.String(nullable: false, maxLength: 50),
                        ClientId = c.String(nullable: false, maxLength: 50),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.TransactionStatus",
                c => new
                    {
                        TransactionStatusId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.TransactionStatusId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Proposals", "UserLastChanges_UserId", "dbo.Users");
            DropForeignKey("dbo.Proposals", "TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.ProposalComponents", "ProposalComponentId", "dbo.Proposals");
            DropForeignKey("dbo.ProposalComponents", "LibraryComponentId", "dbo.LibraryComponents");
            DropForeignKey("dbo.Messages", "UserReceiving_UserId", "dbo.Users");
            DropForeignKey("dbo.Messages", "UserProponent_UserId", "dbo.Users");
            DropForeignKey("dbo.Libraries", "UserId", "dbo.Users");
            DropForeignKey("dbo.LibraryComponents", "LibraryId", "dbo.Libraries");
            DropForeignKey("dbo.LibraryComponents", "StatusId", "dbo.Status");
            DropForeignKey("dbo.LibraryComponents", "GameId", "dbo.Games");
            DropForeignKey("dbo.LibraryComponents", "GameLanguageId", "dbo.GameLanguages");
            DropForeignKey("dbo.Games", "PlatformId", "dbo.Platforms");
            DropForeignKey("dbo.Games", "GenreId", "dbo.Genres");
            DropForeignKey("dbo.Feedbacks", "UserId", "dbo.Users");
            DropForeignKey("dbo.Feedbacks", "TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.Transactions", "UserReceiving_UserId", "dbo.Users");
            DropForeignKey("dbo.Transactions", "UserProponent_UserId", "dbo.Users");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Proposals", new[] { "UserLastChanges_UserId" });
            DropIndex("dbo.Proposals", new[] { "TransactionId" });
            DropIndex("dbo.ProposalComponents", new[] { "LibraryComponentId" });
            DropIndex("dbo.ProposalComponents", new[] { "ProposalComponentId" });
            DropIndex("dbo.Messages", new[] { "UserReceiving_UserId" });
            DropIndex("dbo.Messages", new[] { "UserProponent_UserId" });
            DropIndex("dbo.LibraryComponents", new[] { "GameLanguageId" });
            DropIndex("dbo.LibraryComponents", new[] { "StatusId" });
            DropIndex("dbo.LibraryComponents", new[] { "LibraryId" });
            DropIndex("dbo.LibraryComponents", new[] { "GameId" });
            DropIndex("dbo.Libraries", new[] { "UserId" });
            DropIndex("dbo.Games", new[] { "PlatformId" });
            DropIndex("dbo.Games", new[] { "GenreId" });
            DropIndex("dbo.Transactions", new[] { "UserReceiving_UserId" });
            DropIndex("dbo.Transactions", new[] { "UserProponent_UserId" });
            DropIndex("dbo.Feedbacks", new[] { "UserId" });
            DropIndex("dbo.Feedbacks", new[] { "TransactionId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.TransactionStatus");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RefreshTokens");
            DropTable("dbo.ProposalStatus");
            DropTable("dbo.Proposals");
            DropTable("dbo.ProposalComponents");
            DropTable("dbo.Messages");
            DropTable("dbo.Status");
            DropTable("dbo.LibraryComponents");
            DropTable("dbo.Libraries");
            DropTable("dbo.Platforms");
            DropTable("dbo.Genres");
            DropTable("dbo.Games");
            DropTable("dbo.GameLanguages");
            DropTable("dbo.Users");
            DropTable("dbo.Transactions");
            DropTable("dbo.Feedbacks");
            DropTable("dbo.Clients");
        }
    }
}
