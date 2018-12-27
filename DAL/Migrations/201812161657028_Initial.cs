using System.Data.Entity.Migrations;

namespace DAL.Migrations
{
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.FideProfiles",
                    c => new
                    {
                        Id = c.Int(false, true),
                        FideId = c.Int(false),
                        Name = c.String(false),
                        Birth = c.Int(false),
                        Std = c.Int(false),
                        Rpd = c.Int(false),
                        Blz = c.Int(false)
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                    "dbo.Groups",
                    c => new
                    {
                        Id = c.Int(false, true),
                        Name = c.String(false)
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                    "dbo.Profiles",
                    c => new
                    {
                        Id = c.Int(false, true),
                        RcfProfileId = c.Int(),
                        FideProfileId = c.Int(),
                        GroupId = c.Int(false)
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FideProfiles", t => t.FideProfileId)
                .ForeignKey("dbo.Groups", t => t.GroupId, true)
                .ForeignKey("dbo.RcfProfiles", t => t.RcfProfileId)
                .Index(t => t.RcfProfileId)
                .Index(t => t.FideProfileId)
                .Index(t => t.GroupId);

            CreateTable(
                    "dbo.RcfProfiles",
                    c => new
                    {
                        Id = c.Int(false, true),
                        RcfId = c.Int(false),
                        Name = c.String(false),
                        Birth = c.Int(false),
                        Std = c.Int(false),
                        Rpd = c.Int(false),
                        Blz = c.Int(false),
                        FideProfileId = c.Int()
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
            DropIndex("dbo.RcfProfiles", new[] {"FideProfileId"});
            DropIndex("dbo.Profiles", new[] {"GroupId"});
            DropIndex("dbo.Profiles", new[] {"FideProfileId"});
            DropIndex("dbo.Profiles", new[] {"RcfProfileId"});
            DropTable("dbo.RcfProfiles");
            DropTable("dbo.Profiles");
            DropTable("dbo.Groups");
            DropTable("dbo.FideProfiles");
        }
    }
}