using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManagement.Migrations
{
    public partial class _2020525zw : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceDetails_Devices_DeviceID",
                table: "DeviceDetails");

            migrationBuilder.DropIndex(
                name: "IX_DeviceDetails_DeviceID",
                table: "DeviceDetails");
        }
    }
}
