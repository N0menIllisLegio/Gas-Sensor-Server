using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gss.Infrastructure.Migrations
{
  public partial class PredefinedValuesMigration: Migration
  {
    private const string _administratorRoleID = "2672396d-e4e2-4f8e-880f-cca9a7a260d6";
    private const string _userRoleID = "04bf1ca1-0600-4c4b-86c5-2f16998b02d8";
    private const string _administratorID = "a21afc0b-1135-4b23-a672-758e1f788bc8";

    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.InsertData(
        table: "AspNetRoles",
        columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
        values: new object[,]
        {
          { _administratorRoleID, "Administrator", "ADMINISTRATOR", Guid.NewGuid().ToString() },
          { _userRoleID, "User", "USER", Guid.NewGuid().ToString() },
        });

      migrationBuilder.InsertData(
        table: "AspNetUsers",
        columns: new[]
        {
          "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed",
          "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumberConfirmed", "TwoFactorEnabled",
          "LockoutEnabled", "AccessFailedCount", "FirstName"
        },
        values: new object[,]
        {
          {
            _administratorID, "Administrator", "ADMINISTRATOR", "example@example.com", "EXAMPLE@EXAMPLE.COM", true,
            "AQAAAAEAACcQAAAAEJpwAoOfq+3/ufQifs4EKrtqIDq2tGczMd5qti8XAn46phFUIGx6/Ps547tFcegTzA==",
            Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false, false, true, 0, "Administrator"
          },
        });

      migrationBuilder.InsertData(
        table: "AspNetUserRoles",
        columns: new[] { "UserId", "RoleId" },
        values: new object[] { _administratorID, _administratorRoleID });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DeleteData(table: "AspNetUsers", keyColumn: "Id", _administratorID);
      migrationBuilder.DeleteData(table: "AspNetRoles", keyColumn: "Id", _administratorRoleID);
      migrationBuilder.DeleteData(table: "AspNetRoles", keyColumn: "Id", _userRoleID);
    }
  }
}
