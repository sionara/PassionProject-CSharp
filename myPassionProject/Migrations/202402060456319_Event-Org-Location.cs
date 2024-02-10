namespace myPassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventOrgLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "LocationAddress", c => c.String());
            AddColumn("dbo.Organizations", "OrganizationContact", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organizations", "OrganizationContact");
            DropColumn("dbo.Locations", "LocationAddress");
        }
    }
}
