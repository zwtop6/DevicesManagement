using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManagement.Migrations
{
    public partial class AddHealthStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HealthStatus",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HealthStatus",
                table: "Devices");
        }
    }
}
