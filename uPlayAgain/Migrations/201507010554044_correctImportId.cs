namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class correctImportId : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Games", "ImportId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Games", "ImportId", c => c.Int(nullable: false));
        }
    }
}
