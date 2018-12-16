namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FideProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FideId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Birth = c.Int(nullable: false),
                        Std = c.Int(nullable: false),
                        Rpd = c.Int(nullable: false),
                        Blz = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RcfProfileId = c.Int(),
                        FideProfileId = c.Int(),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FideProfiles", t => t.FideProfileId)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.RcfProfiles", t => t.RcfProfileId)
                .Index(t => t.RcfProfileId)
                .Index(t => t.FideProfileId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.RcfProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RcfId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Birth = c.Int(nullable: false),
                        Std = c.Int(nullable: false),
                        Rpd = c.Int(nullable: false),
                        Blz = c.Int(nullable: false),
                        FideProfileId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FideProfiles", t => t.FideProfileId)
                .Index(t => t.FideProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Profiles", "RcfProfileId", "dbo.RcfProfiles");
            DropForeignKey("dbo.RcfProfiles", "FideProfileId", "dbo.FideProfiles");
            DropForeignKey("dbo.Profiles", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Profiles", "FideProfileId", "dbo.FideProfiles");
            DropIndex("dbo.RcfProfiles", new[] { "FideProfileId" });
            DropIndex("dbo.Profiles", new[] { "GroupId" });
            DropIndex("dbo.Profiles", new[] { "FideProfileId" });
            DropIndex("dbo.Profiles", new[] { "RcfProfileId" });
            DropTable("dbo.RcfProfiles");
            DropTable("dbo.Profiles");
            DropTable("dbo.Groups");
            DropTable("dbo.FideProfiles");
        }
    }
}
