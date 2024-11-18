using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gss.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Microcontrollers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LastResponseTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Public = table.Column<bool>(type: "boolean", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Key = table.Column<string>(type: "text", nullable: false),
                    RequestedMicrocontrollerSensorId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Microcontrollers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SensorsTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Units = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorsTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sensors_SensorsTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SensorsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MicrocontrollerSensors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MicrocontrollerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SensorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CriticalValue = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MicrocontrollerSensors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MicrocontrollerSensors_Microcontrollers_MicrocontrollerId",
                        column: x => x.MicrocontrollerId,
                        principalTable: "Microcontrollers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MicrocontrollerSensors_Sensors_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorsData",
                columns: table => new
                {
                    MicrocontrollerSensorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReadTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    ReceivedTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorsData", x => new { x.MicrocontrollerSensorId, x.ReadTime });
                    table.ForeignKey(
                        name: "FK_SensorsData_MicrocontrollerSensors_MicrocontrollerSensorId",
                        column: x => x.MicrocontrollerSensorId,
                        principalTable: "MicrocontrollerSensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Microcontrollers_OwnerId",
                table: "Microcontrollers",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MicrocontrollerSensors_MicrocontrollerId",
                table: "MicrocontrollerSensors",
                column: "MicrocontrollerId");

            migrationBuilder.CreateIndex(
                name: "IX_MicrocontrollerSensors_SensorId",
                table: "MicrocontrollerSensors",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_TypeId",
                table: "Sensors",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensorsData");

            migrationBuilder.DropTable(
                name: "MicrocontrollerSensors");

            migrationBuilder.DropTable(
                name: "Microcontrollers");

            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "SensorsTypes");
        }
    }
}
