namespace ProjektSpoleczenstwo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "WorkFromTime", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.Jobs", "WorkToTime", c => c.Time(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "WorkToTime");
            DropColumn("dbo.Jobs", "WorkFromTime");
        }
    }
}
