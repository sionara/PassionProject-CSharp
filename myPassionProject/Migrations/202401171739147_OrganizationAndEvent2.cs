namespace myPassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrganizationAndEvent2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        EventId = c.Int(nullable: false, identity: true),
                        EventName = c.String(),
                        HostedBy = c.String(),
                        registrationWebsite = c.String(),
                    })
                .PrimaryKey(t => t.EventId);
            
            CreateTable(
                "dbo.Organizations",
                c => new
                    {
                        OrganizationId = c.Int(nullable: false, identity: true),
                        OrganizationName = c.String(),
                    })
                .PrimaryKey(t => t.OrganizationId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Organizations");
            DropTable("dbo.Events");
        }
    }
}
