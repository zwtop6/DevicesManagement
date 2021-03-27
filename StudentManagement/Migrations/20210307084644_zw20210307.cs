using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManagement.Migrations
{
    public partial class zw20210307 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeviceDetailId",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeviceDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LowPressStartP = table.Column<double>(type: "float", nullable: false),
                    LowPressEndP = table.Column<double>(type: "float", nullable: false),
                    LowPressDuring = table.Column<double>(type: "float", nullable: false),
                    HighPressStartP = table.Column<double>(type: "float", nullable: false),
                    HighPressEndP = table.Column<double>(type: "float", nullable: false),
                    HighPressDuring = table.Column<double>(type: "float", nullable: false),
                    GasAdvice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VacuumPress = table.Column<double>(type: "float", nullable: false),
                    AirPress = table.Column<double>(type: "float", nullable: false),
                    StandardAirPress = table.Column<double>(type: "float", nullable: false),
                    AirPressMax = table.Column<double>(type: "float", nullable: false),
                    AirPressMin = table.Column<double>(type: "float", nullable: false),
                    SenorAdvice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenValve = table.Column<bool>(type: "bit", nullable: false),
                    CloseValve = table.Column<bool>(type: "bit", nullable: false),
                    AutoValve = table.Column<bool>(type: "bit", nullable: false),
                    ValveAdvice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseDuring = table.Column<double>(type: "float", nullable: false),
                    LastChangeOilTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangOilTime = table.Column<double>(type: "float", nullable: false),
                    PumpAdvice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpMotor = table.Column<bool>(type: "bit", nullable: false),
                    DownMotor = table.Column<bool>(type: "bit", nullable: false),
                    StopMotor = table.Column<bool>(type: "bit", nullable: false),
                    UpDuring = table.Column<double>(type: "float", nullable: false),
                    DownDuring = table.Column<double>(type: "float", nullable: false),
                    MotorAdvice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartStove = table.Column<bool>(type: "bit", nullable: false),
                    StopStove = table.Column<bool>(type: "bit", nullable: false),
                    HoldStove = table.Column<bool>(type: "bit", nullable: false),
                    StoveTempMax = table.Column<double>(type: "float", nullable: false),
                    StoveTempMin = table.Column<double>(type: "float", nullable: false),
                    StoveAirTemp = table.Column<double>(type: "float", nullable: false),
                    StandardAirTemp = table.Column<double>(type: "float", nullable: false),
                    StoveAdvice = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDetail", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceDetail_DeviceDetailId",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "DeviceDetail");

            migrationBuilder.DropIndex(
                name: "IX_Devices_DeviceDetailId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DeviceDetailId",
                table: "Devices");
        }
    }
}
