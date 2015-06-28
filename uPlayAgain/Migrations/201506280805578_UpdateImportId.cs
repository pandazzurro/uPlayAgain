namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateImportId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Platforms", "ImportId", c => c.Int(nullable: false));
            DropColumn("dbo.Platforms", "IdImport");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Platforms", "IdImport", c => c.Int(nullable: false));
            DropColumn("dbo.Platforms", "ImportId");
        }
    }
}
