using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class changedDatatype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_name = 'Drivers'
                      AND column_name = 'DriverId'
                ) THEN
                    ALTER TABLE "Drivers" DROP COLUMN "DriverId";
                END IF;
            END $$;
            """);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Drivers",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Drivers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "DriverId",
                table: "Drivers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
