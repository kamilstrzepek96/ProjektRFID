namespace ProjektSpoleczenstwo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class new3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Departments", "Id", "dbo.Employees");
            DropForeignKey("dbo.Jobs", "Id", "dbo.Employees");
            DropIndex("dbo.Departments", new[] { "Id" });
            DropIndex("dbo.Jobs", new[] { "Id" });
            DropPrimaryKey("dbo.Departments");
            DropPrimaryKey("dbo.Jobs");
            AddColumn("dbo.Departments", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.Employees", "Department_Id", c => c.Int());
            AddColumn("dbo.Employees", "Job_Id", c => c.Int());
            AddColumn("dbo.Jobs", "EmployeeId", c => c.Int(nullable: false));
            AlterColumn("dbo.Departments", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Jobs", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Departments", "Id");
            AddPrimaryKey("dbo.Jobs", "Id");
            CreateIndex("dbo.Employees", "Department_Id");
            CreateIndex("dbo.Employees", "Job_Id");
            AddForeignKey("dbo.Employees", "Department_Id", "dbo.Departments", "Id");
            AddForeignKey("dbo.Employees", "Job_Id", "dbo.Jobs", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employees", "Job_Id", "dbo.Jobs");
            DropForeignKey("dbo.Employees", "Department_Id", "dbo.Departments");
            DropIndex("dbo.Employees", new[] { "Job_Id" });
            DropIndex("dbo.Employees", new[] { "Department_Id" });
            DropPrimaryKey("dbo.Jobs");
            DropPrimaryKey("dbo.Departments");
            AlterColumn("dbo.Jobs", "Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Departments", "Id", c => c.Int(nullable: false));
            DropColumn("dbo.Jobs", "EmployeeId");
            DropColumn("dbo.Employees", "Job_Id");
            DropColumn("dbo.Employees", "Department_Id");
            DropColumn("dbo.Departments", "EmployeeId");
            AddPrimaryKey("dbo.Jobs", "Id");
            AddPrimaryKey("dbo.Departments", "Id");
            CreateIndex("dbo.Jobs", "Id");
            CreateIndex("dbo.Departments", "Id");
            AddForeignKey("dbo.Jobs", "Id", "dbo.Employees", "Id");
            AddForeignKey("dbo.Departments", "Id", "dbo.Employees", "Id");
        }
    }
}
