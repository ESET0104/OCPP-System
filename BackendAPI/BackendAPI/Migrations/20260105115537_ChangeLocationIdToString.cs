using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLocationIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Drop FK constraint first
            migrationBuilder.DropForeignKey(
                name: "FK_Chargers_Locations_LocationId",
                table: "Chargers");

            // 2️⃣ Drop IDENTITY from Locations.Id (CRITICAL)
            migrationBuilder.Sql("""
        ALTER TABLE "Locations"
        ALTER COLUMN "Id" DROP IDENTITY IF EXISTS;
    """);

            // 3️⃣ Change Locations.Id from int → text
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Locations",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            // 4️⃣ Change Chargers.LocationId from int → text
            migrationBuilder.AlterColumn<string>(
                name: "LocationId",
                table: "Chargers",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            // 5️⃣ Re-add FK constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Chargers_Locations_LocationId",
                table: "Chargers",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // 6️⃣ Keep your existing column addition (UNCHANGED)
            //migrationBuilder.AddColumn<int>(
            //    name: "InitialCharge",
            //    table: "ChargingSessions",
            //    type: "integer",
            //    nullable: false,
            //    defaultValue: 0);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chargers_Locations_LocationId",
                table: "Chargers");

            //migrationBuilder.DropColumn(
            //    name: "InitialCharge",
            //    table: "ChargingSessions");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Locations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("Npgsql:Identity", "BY DEFAULT");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "Chargers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Chargers_Locations_LocationId",
                table: "Chargers",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
