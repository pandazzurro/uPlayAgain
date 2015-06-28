namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMessageAndImport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "ImportId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "ImportId");
        }
    }
}
