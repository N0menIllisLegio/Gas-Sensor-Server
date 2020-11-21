using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gss.Infrastructure.Migrations
{
    public partial class ManyToManyMCSensorMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_Microcontrollers_MicrocontrollerID",
                table: "Sensors");

            migrationBuilder.DropForeignKey(
                name: "FK_SensorsData_Microcontrollers_MicrocontrollerID",
                table: "SensorsData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SensorsData",
                table: "SensorsData");

            migrationBuilder.DropIndex(
                name: "IX_SensorsData_MicrocontrollerID",
                table: "SensorsData");

            migrationBuilder.DropIndex(
                name: "IX_Sensors_MicrocontrollerID",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "MicrocontrollerID",
                table: "Sensors");

            migrationBuilder.AlterColumn<Guid>(
                name: "MicrocontrollerID",
                table: "SensorsData",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SensorID",
                table: "SensorsData",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Microcontrollers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Microcontrollers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SensorsData",
                table: "SensorsData",
                columns: new[] { "MicrocontrollerID", "SensorID", "ValueReadTime" });

            migrationBuilder.CreateTable(
                name: "MicrocontrollerSensor",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MicrocontrollerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SensorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MicrocontrollerSensor", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MicrocontrollerSensor_Microcontrollers_MicrocontrollerID",
                        column: x => x.MicrocontrollerID,
                        principalTable: "Microcontrollers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MicrocontrollerSensor_Sensors_SensorID",
                        column: x => x.SensorID,
                        principalTable: "Sensors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorsData_SensorID",
                table: "SensorsData",
                column: "SensorID");

            migrationBuilder.CreateIndex(
                name: "IX_MicrocontrollerSensor_MicrocontrollerID",
                table: "MicrocontrollerSensor",
                column: "MicrocontrollerID");

            migrationBuilder.CreateIndex(
                name: "IX_MicrocontrollerSensor_SensorID",
                table: "MicrocontrollerSensor",
                column: "SensorID");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorsData_Microcontrollers_MicrocontrollerID",
                table: "SensorsData",
                column: "MicrocontrollerID",
                principalTable: "Microcontrollers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SensorsData_Sensors_SensorID",
                table: "SensorsData",
                column: "SensorID",
                principalTable: "Sensors",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorsData_Microcontrollers_MicrocontrollerID",
                table: "SensorsData");

            migrationBuilder.DropForeignKey(
                name: "FK_SensorsData_Sensors_SensorID",
                table: "SensorsData");

            migrationBuilder.DropTable(
                name: "MicrocontrollerSensor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SensorsData",
                table: "SensorsData");

            migrationBuilder.DropIndex(
                name: "IX_SensorsData_SensorID",
                table: "SensorsData");

            migrationBuilder.DropColumn(
                name: "SensorID",
                table: "SensorsData");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Microcontrollers");

            migrationBuilder.AlterColumn<Guid>(
                name: "MicrocontrollerID",
                table: "SensorsData",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "MicrocontrollerID",
                table: "Sensors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Microcontrollers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SensorsData",
                table: "SensorsData",
                column: "ValueReadTime");

            migrationBuilder.CreateIndex(
                name: "IX_SensorsData_MicrocontrollerID",
                table: "SensorsData",
                column: "MicrocontrollerID");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_MicrocontrollerID",
                table: "Sensors",
                column: "MicrocontrollerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_Microcontrollers_MicrocontrollerID",
                table: "Sensors",
                column: "MicrocontrollerID",
                principalTable: "Microcontrollers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SensorsData_Microcontrollers_MicrocontrollerID",
                table: "SensorsData",
                column: "MicrocontrollerID",
                principalTable: "Microcontrollers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
