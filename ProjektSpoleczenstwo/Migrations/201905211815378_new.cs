namespace ProjektSpoleczenstwo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _new : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                {
                    Id = c.Int(nullable: false),
                    Name = c.String(),
                    Location = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.Id)
                .Index(t => t.Id);

            CreateTable(
                "dbo.Employees",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    Surname = c.String(),
                    Age = c.String(),
                    Token = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Jobs",
                c => new
                {
                    Id = c.Int(nullable: false),
                    Title = c.String(),
                    Salary = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.Id)
                .Index(t => t.Id);

            CreateTable(
                "dbo.WorkHours",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    PunchTime = c.DateTime(nullable: false),
                    EmployeeId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Departments", "Id", "dbo.Employees");
            DropForeignKey("dbo.WorkHours", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Jobs", "Id", "dbo.Employees");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.WorkHours", new[] { "EmployeeId" });
            DropIndex("dbo.Jobs", new[] { "Id" });
            DropIndex("dbo.Departments", new[] { "Id" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.WorkHours");
            DropTable("dbo.Jobs");
            DropTable("dbo.Employees");
            DropTable("dbo.Departments");
        }
    }
}
