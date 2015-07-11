namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateReadMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "IsAlreadyRead", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "IsAlreadyRead");
        }
    }
}
