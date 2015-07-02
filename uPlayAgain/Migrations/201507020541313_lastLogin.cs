namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lastLogin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LastLogin", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "LastLogin");
        }
    }
}
