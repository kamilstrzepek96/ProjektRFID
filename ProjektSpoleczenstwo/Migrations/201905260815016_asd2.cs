namespace ProjektSpoleczenstwo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asd2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WorkHours", "ElapsedTime", c => c.Time(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WorkHours", "ElapsedTime", c => c.Time(precision: 7));
        }
    }
}
