using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManagement.Migrations
{
    public partial class zw2105251912 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceDetails_Devices_DeviceID",
                table: "DeviceDetails");

            migrationBuilder.DropIndex(
                name: "IX_DeviceDetails_DeviceID",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "DeviceID",
                table: "DeviceDetails");

            migrationBuilder.AddColumn<int>(
                name: "DeviceDetailId",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GUID",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceGUID",
                table: "DeviceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceDetailId",
                table: "Devices",
                column: "DeviceDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceDetails_DeviceDetailId",
                table: "Devices",
                column: "DeviceDetailId",
                principalTable: "DeviceDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceDetails_DeviceDetailId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_DeviceDetailId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DeviceDetailId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "GUID",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DeviceGUID",
                table: "DeviceDetails");

            migrationBuilder.AddColumn<int>(
                name: "DeviceID",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDetails_DeviceID",
                table: "DeviceDetails",
                column: "DeviceID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceDetails_Devices_DeviceID",
                table: "DeviceDetails",
                column: "DeviceID",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
