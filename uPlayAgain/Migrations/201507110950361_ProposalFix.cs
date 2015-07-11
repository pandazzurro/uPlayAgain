namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProposalFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Proposals", "TransactionId", "dbo.Transactions");
            DropIndex("dbo.Proposals", new[] { "TransactionId" });
            AddColumn("dbo.Transactions", "ProposalId", c => c.Int(nullable: false));
            CreateIndex("dbo.Transactions", "TransactionId");
            AddForeignKey("dbo.Transactions", "TransactionId", "dbo.Proposals", "ProposalId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "TransactionId", "dbo.Proposals");
            DropIndex("dbo.Transactions", new[] { "TransactionId" });
            DropColumn("dbo.Transactions", "ProposalId");
            CreateIndex("dbo.Proposals", "TransactionId");
            AddForeignKey("dbo.Proposals", "TransactionId", "dbo.Transactions", "TransactionId", cascadeDelete: true);
        }
    }
}
