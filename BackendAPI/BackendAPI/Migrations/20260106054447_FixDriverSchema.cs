using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixDriverSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Users");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Status",
            //    table: "Drivers",
            //    type: "text",
            //    nullable: false,
            //    defaultValue: "ACTIVE",
            //    oldClrType: typeof(string),
            //    oldType: "text");

            //migrationBuilder.AddColumn<int>(
            //    name: "InitialCharge",
            //    table: "ChargingSessions",
            //    type: "integer",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<int>(
            //    name: "SOC",
            //    table: "ChargingSessions",
            //    type: "integer",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<int>(
            //    name: "LocationId",
            //    table: "Chargers",
            //    type: "integer",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.CreateTable(
            //    name: "Admins",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        Username = table.Column<string>(type: "text", nullable: false),
            //        Email = table.Column<string>(type: "text", nullable: false),
            //        Password = table.Column<string>(type: "text", nullable: false),
            //        Status = table.Column<string>(type: "text", nullable: false),
            //        Company = table.Column<string>(type: "text", nullable: false),
            //        Department = table.Column<string>(type: "text", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        LastActiveAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        Tokenat = table.Column<long>(type: "bigint", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Admins", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Locations",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        Name = table.Column<string>(type: "text", nullable: false),
            //        Address = table.Column<string>(type: "text", nullable: false),
            //        Latitude = table.Column<decimal>(type: "numeric", nullable: false),
            //        Longitude = table.Column<decimal>(type: "numeric", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Locations", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Managers",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        Username = table.Column<string>(type: "text", nullable: false),
            //        Email = table.Column<string>(type: "text", nullable: false),
            //        Password = table.Column<string>(type: "text", nullable: false),
            //        Status = table.Column<string>(type: "text", nullable: false),
            //        Company = table.Column<string>(type: "text", nullable: false),
            //        Department = table.Column<string>(type: "text", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        LastActiveAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        Tokenat = table.Column<long>(type: "bigint", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Managers", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Supervisors",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        Username = table.Column<string>(type: "text", nullable: false),
            //        Email = table.Column<string>(type: "text", nullable: false),
            //        Password = table.Column<string>(type: "text", nullable: false),
            //        Status = table.Column<string>(type: "text", nullable: false),
            //        Company = table.Column<string>(type: "text", nullable: false),
            //        Department = table.Column<string>(type: "text", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        LastActiveAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        Tokenat = table.Column<long>(type: "bigint", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Supervisors", x => x.Id);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Drivers_Email",
            //    table: "Drivers",
            //    column: "Email",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Chargers_LocationId",
            //    table: "Chargers",
            //    column: "LocationId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Admins_Email",
            //    table: "Admins",
            //    column: "Email",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Admins_Username",
            //    table: "Admins",
            //    column: "Username",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Managers_Email",
            //    table: "Managers",
            //    column: "Email",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Managers_Username",
            //    table: "Managers",
            //    column: "Username",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Supervisors_Email",
            //    table: "Supervisors",
            //    column: "Email",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Supervisors_Username",
            //    table: "Supervisors",
            //    column: "Username",
            //    unique: true);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Chargers_Locations_LocationId",
            //    table: "Chargers",
            //    column: "LocationId",
            //    principalTable: "Locations",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Chargers_Locations_LocationId",
            //    table: "Chargers");

            //migrationBuilder.DropTable(
            //    name: "Admins");

            //migrationBuilder.DropTable(
            //    name: "Locations");

            //migrationBuilder.DropTable(
            //    name: "Managers");

            //migrationBuilder.DropTable(
            //    name: "Supervisors");

            //migrationBuilder.DropIndex(
            //    name: "IX_Drivers_Email",
            //    table: "Drivers");

            //migrationBuilder.DropIndex(
            //    name: "IX_Chargers_LocationId",
            //    table: "Chargers");

            //migrationBuilder.DropColumn(
            //    name: "InitialCharge",
            //    table: "ChargingSessions");

            //migrationBuilder.DropColumn(
            //    name: "SOC",
            //    table: "ChargingSessions");

            //migrationBuilder.DropColumn(
            //    name: "LocationId",
            //    table: "Chargers");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Status",
            //    table: "Drivers",
            //    type: "text",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "text",
            //    oldDefaultValue: "ACTIVE");

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        Name = table.Column<string>(type: "text", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.Id);
            //    });
        }
    }
}
