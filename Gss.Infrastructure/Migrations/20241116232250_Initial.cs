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
                    IPAddress = table.Column<string>(type: "text", nullable: false),
                    LastResponseTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Public = table.Column<bool>(type: "boolean", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    RequestedSensorID = table.Column<Guid>(type: "uuid", nullable: true),
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
                    TypeID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sensors_SensorsTypes_TypeID",
                        column: x => x.TypeID,
                        principalTable: "SensorsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MicrocontrollerSensors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MicrocontrollerID = table.Column<Guid>(type: "uuid", nullable: false),
                    SensorID = table.Column<Guid>(type: "uuid", nullable: false),
                    CriticalValue = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MicrocontrollerSensors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MicrocontrollerSensors_Microcontrollers_MicrocontrollerID",
                        column: x => x.MicrocontrollerID,
                        principalTable: "Microcontrollers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MicrocontrollerSensors_Sensors_SensorID",
                        column: x => x.SensorID,
                        principalTable: "Sensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorsData",
                columns: table => new
                {
                    MicrocontrollerID = table.Column<Guid>(type: "uuid", nullable: false),
                    SensorID = table.Column<Guid>(type: "uuid", nullable: false),
                    ValueReadTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SensorValue = table.Column<int>(type: "integer", nullable: false),
                    ValueReceivedTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorsData", x => new { x.MicrocontrollerID, x.SensorID, x.ValueReadTime });
                    table.ForeignKey(
                        name: "FK_SensorsData_Microcontrollers_MicrocontrollerID",
                        column: x => x.MicrocontrollerID,
                        principalTable: "Microcontrollers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SensorsData_Sensors_SensorID",
                        column: x => x.SensorID,
                        principalTable: "Sensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Microcontrollers_OwnerId",
                table: "Microcontrollers",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MicrocontrollerSensors_MicrocontrollerID",
                table: "MicrocontrollerSensors",
                column: "MicrocontrollerID");

            migrationBuilder.CreateIndex(
                name: "IX_MicrocontrollerSensors_SensorID",
                table: "MicrocontrollerSensors",
                column: "SensorID");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_TypeID",
                table: "Sensors",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "IX_SensorsData_SensorID",
                table: "SensorsData",
                column: "SensorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MicrocontrollerSensors");

            migrationBuilder.DropTable(
                name: "SensorsData");

            migrationBuilder.DropTable(
                name: "Microcontrollers");

            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "SensorsTypes");
        }
    }
}
