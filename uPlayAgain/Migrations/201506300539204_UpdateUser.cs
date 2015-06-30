namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUser : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "LibraryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "LibraryId", c => c.Int(nullable: false));
        }
    }
}
