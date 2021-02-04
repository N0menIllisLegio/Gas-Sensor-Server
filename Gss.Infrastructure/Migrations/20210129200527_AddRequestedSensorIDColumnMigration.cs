using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gss.Infrastructure.Migrations
{
    public partial class AddRequestedSensorIDColumnMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequestedSensorID",
                table: "Microcontrollers",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestedSensorID",
                table: "Microcontrollers");
        }
    }
}
