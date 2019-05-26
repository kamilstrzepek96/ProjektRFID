namespace ProjektSpoleczenstwo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class e2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "CardUID", c => c.String());
            AddColumn("dbo.WorkHours", "PunchType", c => c.Int(nullable: false));
            DropColumn("dbo.Employees", "Token");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "Token", c => c.String());
            DropColumn("dbo.WorkHours", "PunchType");
            DropColumn("dbo.Employees", "CardUID");
        }
    }
}
