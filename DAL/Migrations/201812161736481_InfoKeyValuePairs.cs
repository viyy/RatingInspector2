using System.Data.Entity.Migrations;

namespace DAL.Migrations
{
    public partial class InfoKeyValuePairs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.InfoValues",
                    c => new
                    {
                        Id = c.Int(false, true),
                        Name = c.String(),
                        Value = c.String()
                    })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.InfoValues");
        }
    }
}