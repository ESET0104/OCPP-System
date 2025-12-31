using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class driverIdedited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Drivers");

            migrationBuilder.AddColumn<int>(
                name: "SOC",
                table: "ChargingSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SOC",
                table: "ChargingSessions");

            migrationBuilder.AddColumn<string>(
                name: "DriverId",
                table: "Drivers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
