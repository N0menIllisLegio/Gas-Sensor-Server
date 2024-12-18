using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gss.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLastResponseAndRequestSensorIdColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastResponseTime",
                table: "Microcontrollers");

            migrationBuilder.DropColumn(
                name: "RequestedMicrocontrollerSensorId",
                table: "Microcontrollers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastResponseTime",
                table: "Microcontrollers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequestedMicrocontrollerSensorId",
                table: "Microcontrollers",
                type: "uuid",
                nullable: true);
        }
    }
}
