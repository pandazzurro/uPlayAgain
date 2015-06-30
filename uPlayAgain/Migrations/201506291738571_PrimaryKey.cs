namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrimaryKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Games", "GameId", "dbo.LibraryComponents");
            DropForeignKey("dbo.Proposals", "ProposalStatusId", "dbo.LibraryComponents");
            DropIndex("dbo.LibraryComponents", new[] { "LibraryComponentId" });
            DropPrimaryKey("dbo.LibraryComponents");
            AlterColumn("dbo.LibraryComponents", "LibraryComponentId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.LibraryComponents", "LibraryComponentId");
            CreateIndex("dbo.LibraryComponents", "LibraryComponentId");
            AddForeignKey("dbo.Games", "GameId", "dbo.LibraryComponents", "LibraryComponentId");
            AddForeignKey("dbo.Proposals", "ProposalStatusId", "dbo.LibraryComponents", "LibraryComponentId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Proposals", "ProposalStatusId", "dbo.LibraryComponents");
            DropForeignKey("dbo.Games", "GameId", "dbo.LibraryComponents");
            DropIndex("dbo.LibraryComponents", new[] { "LibraryComponentId" });
            DropPrimaryKey("dbo.LibraryComponents");
            AlterColumn("dbo.LibraryComponents", "LibraryComponentId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.LibraryComponents", "LibraryComponentId");
            CreateIndex("dbo.LibraryComponents", "LibraryComponentId");
            AddForeignKey("dbo.Proposals", "ProposalStatusId", "dbo.LibraryComponents", "LibraryComponentId", cascadeDelete: true);
            AddForeignKey("dbo.Games", "GameId", "dbo.LibraryComponents", "LibraryComponentId");
        }
    }
}
