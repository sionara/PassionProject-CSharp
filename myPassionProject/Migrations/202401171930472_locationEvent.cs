namespace myPassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class locationEvent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        LocationId = c.Int(nullable: false, identity: true),
                        LocationName = c.String(),
                    })
                .PrimaryKey(t => t.LocationId);
            
            AddColumn("dbo.Events", "LocationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Events", "LocationId");
            AddForeignKey("dbo.Events", "LocationId", "dbo.Locations", "LocationId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "LocationId", "dbo.Locations");
            DropIndex("dbo.Events", new[] { "LocationId" });
            DropColumn("dbo.Events", "LocationId");
            DropTable("dbo.Locations");
        }
    }
}
