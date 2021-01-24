using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gss.Infrastructure.Migrations
{
    public partial class RenameMicrocontrollerSensorMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MicrocontrollerSensor");

            migrationBuilder.CreateTable(
                name: "MicrocontrollerSensors",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MicrocontrollerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SensorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MicrocontrollerSensors", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MicrocontrollerSensors_Microcontrollers_MicrocontrollerID",
                        column: x => x.MicrocontrollerID,
                        principalTable: "Microcontrollers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MicrocontrollerSensors_Sensors_SensorID",
                        column: x => x.SensorID,
                        principalTable: "Sensors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MicrocontrollerSensors_MicrocontrollerID",
                table: "MicrocontrollerSensors",
                column: "MicrocontrollerID");

            migrationBuilder.CreateIndex(
                name: "IX_MicrocontrollerSensors_SensorID",
                table: "MicrocontrollerSensors",
                column: "SensorID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MicrocontrollerSensors");

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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MicrocontrollerSensor_MicrocontrollerID",
                table: "MicrocontrollerSensor",
                column: "MicrocontrollerID");

            migrationBuilder.CreateIndex(
                name: "IX_MicrocontrollerSensor_SensorID",
                table: "MicrocontrollerSensor",
                column: "SensorID");
        }
    }
}
