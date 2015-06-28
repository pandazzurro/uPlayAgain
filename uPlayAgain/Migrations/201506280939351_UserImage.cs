namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "Image", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "Image");
        }
    }
}
