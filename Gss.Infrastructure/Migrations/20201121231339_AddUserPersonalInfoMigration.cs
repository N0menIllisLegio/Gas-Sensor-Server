using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gss.Infrastructure.Migrations
{
    public partial class AddUserPersonalInfoMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Birthday",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            var administratorRoleID = Guid.NewGuid();

            migrationBuilder.Sql(
                $"INSERT INTO [AspNetRoles]([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES" +
                $" ('{administratorRoleID}', 'Administrator', 'ADMINISTRATOR', '{Guid.NewGuid()}')," +
                $" ('{Guid.NewGuid()}', 'User', 'USER', '{Guid.NewGuid()}')");

            var administratorID = Guid.NewGuid();

            migrationBuilder.Sql(
                $"INSERT INTO [AspNetUsers]([Id],[UserName],[NormalizedUserName]," +
                $"[Email],[NormalizedEmail],[EmailConfirmed],[PasswordHash],[SecurityStamp],[ConcurrencyStamp]" +
                $",[PhoneNumberConfirmed],[TwoFactorEnabled],[LockoutEnabled],[AccessFailedCount]) VALUES('{administratorID}'," +
                $"'Administrator','ADMINISTRATOR','example@example.com','EXAMPLE@EXAMPLE.COM',0," +
                $"'AQAAAAEAACcQAAAAEJpwAoOfq+3/ufQifs4EKrtqIDq2tGczMd5qti8XAn46phFUIGx6/Ps547tFcegTzA==','{Guid.NewGuid()}'," +
                $"'{Guid.NewGuid()}',0,0,1,0)");

            migrationBuilder.Sql(
              $"INSERT INTO [AspNetUserRoles]([UserId],[RoleId]) VALUES('{administratorID}', '{administratorRoleID}')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarPath",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Birthday",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");
        }
    }
}
