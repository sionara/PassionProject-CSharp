namespace myPassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventOrganization : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "OrganizationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Events", "OrganizationId");
            AddForeignKey("dbo.Events", "OrganizationId", "dbo.Organizations", "OrganizationId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "OrganizationId", "dbo.Organizations");
            DropIndex("dbo.Events", new[] { "OrganizationId" });
            DropColumn("dbo.Events", "OrganizationId");
        }
    }
}
