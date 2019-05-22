namespace ProjektSpoleczenstwo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ab : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Departments", "EmployeeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Departments", "EmployeeId", c => c.Int(nullable: false));
        }
    }
}
