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
                        GenreId = c.String(maxLength: 128),
                        PlatformId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.GameId)
                .ForeignKey("dbo.Genres", t => t.GenreId)
                .ForeignKey("dbo.Platforms", t => t.PlatformId)
                .Index(t => t.GenreId)
                .Index(t => t.PlatformId);
            
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        GenreId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.GenreId);
            
            CreateTable(
                "dbo.Platforms",
                c => new
                    {
                        PlatformId = c.String(nullable: false, maxLength: 128),
                        Class = c.String(),
                        Name = c.String(),
                        IdImport = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PlatformId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "PlatformId", "dbo.Platforms");
            DropForeignKey("dbo.Games", "GenreId", "dbo.Genres");
            DropIndex("dbo.Games", new[] { "PlatformId" });
            DropIndex("dbo.Games", new[] { "GenreId" });
            DropTable("dbo.Platforms");
            DropTable("dbo.Genres");
            DropTable("dbo.Games");
        }
    }
}
