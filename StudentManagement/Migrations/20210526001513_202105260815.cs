using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManagement.Migrations
{
    public partial class _202105260815 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ErrorNum",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ErrorNum1",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ErrorNum2",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ErrorNum3",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ErrorNum4",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ErrorNum5",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ErrorNum6",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarningNum",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarningNum1",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarningNum2",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarningNum3",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarningNum4",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarningNum5",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarningNum6",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorNum",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "ErrorNum1",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "ErrorNum2",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "ErrorNum3",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "ErrorNum4",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "ErrorNum5",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "ErrorNum6",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "WarningNum",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "WarningNum1",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "WarningNum2",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "WarningNum3",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "WarningNum4",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "WarningNum5",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "WarningNum6",
                table: "DeviceDetails");
        }
    }
}
