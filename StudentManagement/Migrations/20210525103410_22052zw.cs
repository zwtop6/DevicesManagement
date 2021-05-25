using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManagement.Migrations
{
    public partial class _22052zw : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceDetail_DeviceDetailId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_DeviceDetailId",
                table: "Devices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceDetail",
                table: "DeviceDetail");

            migrationBuilder.DropColumn(
                name: "DeviceDetailId",
                table: "Devices");

            migrationBuilder.RenameTable(
                name: "DeviceDetail",
                newName: "DeviceDetails");

            migrationBuilder.AddColumn<int>(
                name: "DeviceID",
                table: "DeviceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceDetails",
                table: "DeviceDetails",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceDetails",
                table: "DeviceDetails");

            migrationBuilder.DropColumn(
                name: "DeviceID",
                table: "DeviceDetails");

            migrationBuilder.RenameTable(
                name: "DeviceDetails",
                newName: "DeviceDetail");

            migrationBuilder.AddColumn<int>(
                name: "DeviceDetailId",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceDetail",
                table: "DeviceDetail",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceDetailId",
                table: "Devices",
                column: "DeviceDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceDetail_DeviceDetailId",
                table: "Devices",
                column: "DeviceDetailId",
                principalTable: "DeviceDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
