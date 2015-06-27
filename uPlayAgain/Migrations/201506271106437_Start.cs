namespace uPlayAgain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Start : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        GameId = c.Int(nullable: false, identity: true),
                        ShortName = c.String(),
                        Title = c.String(),
                        Description = c.String(),
                        GenreName = c.String(maxLength: 128),
                        PlatformName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.GameId)
                .ForeignKey("dbo.Genres", t => t.GenreName)
                .ForeignKey("dbo.Platforms", t => t.PlatformName)
                .Index(t => t.GenreName)
                .Index(t => t.PlatformName);
            
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Platforms",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        IconName = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "PlatformName", "dbo.Platforms");
            DropForeignKey("dbo.Games", "GenreName", "dbo.Genres");
            DropIndex("dbo.Games", new[] { "PlatformName" });
            DropIndex("dbo.Games", new[] { "GenreName" });
            DropTable("dbo.Platforms");
            DropTable("dbo.Genres");
            DropTable("dbo.Games");
        }
    }
}
