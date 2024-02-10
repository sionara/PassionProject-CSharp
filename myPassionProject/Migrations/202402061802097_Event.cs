namespace myPassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Event : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Events", "HostedBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "HostedBy", c => c.String());
        }
    }
}
