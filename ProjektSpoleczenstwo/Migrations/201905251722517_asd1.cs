namespace ProjektSpoleczenstwo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asd1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkHours", "ElapsedTime", c => c.Time(precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkHours", "ElapsedTime");
        }
    }
}
