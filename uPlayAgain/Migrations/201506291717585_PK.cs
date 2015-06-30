namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PK : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LibraryId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "LibraryId");
        }
    }
}
