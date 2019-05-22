namespace ProjektSpoleczenstwo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class a : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Jobs", "EmployeeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Jobs", "EmployeeId", c => c.Int(nullable: false));
        }
    }
}
