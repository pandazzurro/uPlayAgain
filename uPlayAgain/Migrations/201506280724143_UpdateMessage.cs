namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMessage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        MessageText = c.String(),
                        MessageDate = c.DateTimeOffset(nullable: false, precision: 7),
                        UserProponent_UserId = c.Int(nullable: false),
                        UserReceiving_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.Users", t => t.UserProponent_UserId)
                .ForeignKey("dbo.Users", t => t.UserReceiving_UserId)
                .Index(t => t.UserProponent_UserId)
                .Index(t => t.UserReceiving_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "UserReceiving_UserId", "dbo.Users");
            DropForeignKey("dbo.Messages", "UserProponent_UserId", "dbo.Users");
            DropIndex("dbo.Messages", new[] { "UserReceiving_UserId" });
            DropIndex("dbo.Messages", new[] { "UserProponent_UserId" });
            DropTable("dbo.Messages");
        }
    }
}
