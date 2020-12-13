using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManagement.Migrations
{
    public partial class First : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "City", "ClassName", "Name", "PhotoPath" },
                values: new object[] { 1, "北京", 1, "PS1-0001", null });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "City", "ClassName", "Name", "PhotoPath" },
                values: new object[] { 2, "陕西", 3, "PH2-0001", null });
        }
    }
}
