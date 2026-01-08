using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class SyncSnapshotOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
            //        Id = table.Column<string>(type: "text", nullable: false),
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
            //    name: "Logs",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "uuid", nullable: false),
            //        Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        Source = table.Column<string>(type: "text", nullable: false),
            //        EventType = table.Column<string>(type: "text", nullable: false),
            //        Message = table.Column<string>(type: "text", nullable: false),
            //        ChargerId = table.Column<string>(type: "text", nullable: true),
            //        SessionId = table.Column<string>(type: "text", nullable: true),
            //        DriverId = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Logs", x => x.Id);
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
            //    name: "Reservations",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        ChargerId = table.Column<string>(type: "text", nullable: false),
            //        DriverId = table.Column<string>(type: "text", nullable: false),
            //        ConnectorId = table.Column<int>(type: "integer", nullable: false),
            //        Status = table.Column<string>(type: "text", nullable: false),
            //        CreatedBy = table.Column<string>(type: "text", nullable: false),
            //        CancelledBy = table.Column<string>(type: "text", nullable: true),
            //        StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Reservations", x => x.Id);
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

            //migrationBuilder.CreateTable(
            //    name: "SupportTickets",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        Status = table.Column<string>(type: "text", nullable: false),
            //        Category = table.Column<string>(type: "text", nullable: false),
            //        Priority = table.Column<string>(type: "text", nullable: false),
            //        Title = table.Column<string>(type: "text", nullable: false),
            //        Description = table.Column<string>(type: "text", nullable: true),
            //        ChargerId = table.Column<string>(type: "text", nullable: true),
            //        VehicleId = table.Column<string>(type: "text", nullable: true),
            //        CreatedByUserId = table.Column<string>(type: "text", nullable: false),
            //        CreatedByName = table.Column<string>(type: "text", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SupportTickets", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Vehicles",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        VehicleName = table.Column<string>(type: "text", nullable: false),
            //        VIN = table.Column<string>(type: "text", nullable: false),
            //        MakeandModel = table.Column<string>(type: "text", nullable: false),
            //        RegistrationNumber = table.Column<string>(type: "text", nullable: false),
            //        RangeKm = table.Column<int>(type: "integer", nullable: true),
            //        BatteryCapacityKwh = table.Column<double>(type: "double precision", nullable: false),
            //        MaxChargeRateKw = table.Column<double>(type: "double precision", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Vehicles", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Chargers",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        Status = table.Column<string>(type: "text", nullable: false),
            //        LastSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        LocationId = table.Column<string>(type: "text", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Chargers", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Chargers_Locations_LocationId",
            //            column: x => x.LocationId,
            //            principalTable: "Locations",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TicketScreenshots",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        ImageUrl = table.Column<string>(type: "text", nullable: false),
            //        SupportTicketId = table.Column<string>(type: "text", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TicketScreenshots", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_TicketScreenshots_SupportTickets_SupportTicketId",
            //            column: x => x.SupportTicketId,
            //            principalTable: "SupportTickets",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Drivers",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        FullName = table.Column<string>(type: "text", nullable: false),
            //        Email = table.Column<string>(type: "text", nullable: false),
            //        Password = table.Column<string>(type: "text", nullable: false),
            //        Gender = table.Column<string>(type: "text", nullable: true),
            //        DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        Status = table.Column<string>(type: "text", nullable: false, defaultValue: "ACTIVE"),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        LastActiveAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        VehicleId = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Drivers", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Drivers_Vehicles_VehicleId",
            //            column: x => x.VehicleId,
            //            principalTable: "Vehicles",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Faults",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        ChargerId = table.Column<string>(type: "text", nullable: false),
            //        FaultCode = table.Column<string>(type: "text", nullable: false),
            //        Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Faults", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Faults_Chargers_ChargerId",
            //            column: x => x.ChargerId,
            //            principalTable: "Chargers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ChargingSessions",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        ChargerId = table.Column<string>(type: "text", nullable: false),
            //        DriverId = table.Column<string>(type: "text", nullable: false),
            //        StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        InitialCharge = table.Column<int>(type: "integer", nullable: false),
            //        SOC = table.Column<int>(type: "integer", nullable: false),
            //        EnergyConsumedKwh = table.Column<decimal>(type: "numeric", nullable: true),
            //        Status = table.Column<string>(type: "text", nullable: false),
            //        LastMeterUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ChargingSessions", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_ChargingSessions_Chargers_ChargerId",
            //            column: x => x.ChargerId,
            //            principalTable: "Chargers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_ChargingSessions_Drivers_DriverId",
            //            column: x => x.DriverId,
            //            principalTable: "Drivers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

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
            //    name: "IX_Chargers_LocationId",
            //    table: "Chargers",
            //    column: "LocationId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ChargingSessions_ChargerId",
            //    table: "ChargingSessions",
            //    column: "ChargerId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ChargingSessions_DriverId",
            //    table: "ChargingSessions",
            //    column: "DriverId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Drivers_Email",
            //    table: "Drivers",
            //    column: "Email",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Drivers_VehicleId",
            //    table: "Drivers",
            //    column: "VehicleId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Faults_ChargerId",
            //    table: "Faults",
            //    column: "ChargerId");

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

            //migrationBuilder.CreateIndex(
            //    name: "IX_TicketScreenshots_SupportTicketId",
            //    table: "TicketScreenshots",
            //    column: "SupportTicketId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Vehicles_RegistrationNumber",
            //    table: "Vehicles",
            //    column: "RegistrationNumber",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Vehicles_VIN",
            //    table: "Vehicles",
            //    column: "VIN",
            //    unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        //    migrationBuilder.DropTable(
        //        name: "Admins");

        //    migrationBuilder.DropTable(
        //        name: "ChargingSessions");

        //    migrationBuilder.DropTable(
        //        name: "Faults");

        //    migrationBuilder.DropTable(
        //        name: "Logs");

        //    migrationBuilder.DropTable(
        //        name: "Managers");

        //    migrationBuilder.DropTable(
        //        name: "Reservations");

        //    migrationBuilder.DropTable(
        //        name: "Supervisors");

        //    migrationBuilder.DropTable(
        //        name: "TicketScreenshots");

        //    migrationBuilder.DropTable(
        //        name: "Drivers");

        //    migrationBuilder.DropTable(
        //        name: "Chargers");

        //    migrationBuilder.DropTable(
        //        name: "SupportTickets");

        //    migrationBuilder.DropTable(
        //        name: "Vehicles");

        //    migrationBuilder.DropTable(
        //        name: "Locations");
        //
        }
    }
}
