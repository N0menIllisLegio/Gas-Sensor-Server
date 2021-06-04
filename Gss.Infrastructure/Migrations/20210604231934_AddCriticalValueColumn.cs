using Microsoft.EntityFrameworkCore.Migrations;

namespace Gss.Infrastructure.Migrations
{
  public partial class AddCriticalValueColumn: Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<int>(
        name: "CriticalValue",
        table: "MicrocontrollerSensors",
        type: "int",
        nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
        name: "CriticalValue",
        table: "MicrocontrollerSensors");
    }
  }
}
