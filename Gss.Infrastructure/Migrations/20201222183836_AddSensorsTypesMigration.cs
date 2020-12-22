using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gss.Infrastructure.Migrations
{
    public partial class AddSensorsTypesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TypeID",
                table: "Sensors",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "SensorsTypes",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Units = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorsTypes", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_TypeID",
                table: "Sensors",
                column: "TypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_SensorsTypes_TypeID",
                table: "Sensors",
                column: "TypeID",
                principalTable: "SensorsTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.CreateIndex(
               name: "IX_SensorsTypes_Name",
               table: "SensorsTypes",
               column: "Name",
               unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_SensorsTypes_TypeID",
                table: "Sensors");

            migrationBuilder.DropTable(
                name: "SensorsTypes");

            migrationBuilder.DropIndex(
                name: "IX_Sensors_TypeID",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "TypeID",
                table: "Sensors");
        }
    }
}
