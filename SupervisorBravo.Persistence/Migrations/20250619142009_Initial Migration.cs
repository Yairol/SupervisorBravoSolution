using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorBravo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alarm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    AlarmDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alarm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceAlarm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    AlarmDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeviceName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceAlarm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DixellBase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomName = table.Column<string>(type: "text", nullable: false),
                    ControlON_OFF = table.Column<bool>(type: "boolean", nullable: false),
                    ControlON_OFFWrite = table.Column<bool>(type: "boolean", nullable: false),
                    MoodbusId = table.Column<int>(type: "integer", nullable: false),
                    SetPoint = table.Column<double>(type: "double precision", nullable: false),
                    SetPointWrite = table.Column<bool>(type: "boolean", nullable: false),
                    AlarmEnable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DixellBase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DixellXR",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cooling = table.Column<bool>(type: "boolean", nullable: false),
                    Fan = table.Column<bool>(type: "boolean", nullable: false),
                    Thawing = table.Column<bool>(type: "boolean", nullable: false),
                    ThawingWrite = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DixellXR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DixellXR_DixellBase_Id",
                        column: x => x.Id,
                        principalTable: "DixellBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DixellXT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectroValve = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DixellXT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DixellXT_DixellBase_Id",
                        column: x => x.Id,
                        principalTable: "DixellBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Temperature",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemperatureMeasurement = table.Column<double>(type: "double precision", nullable: false),
                    MeasurementTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ControlEnable = table.Column<bool>(type: "boolean", nullable: false),
                    On_OffDixell = table.Column<bool>(type: "boolean", nullable: false),
                    DisconnectDixell = table.Column<bool>(type: "boolean", nullable: false),
                    DixellId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temperature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Temperature_DixellBase_DixellId",
                        column: x => x.DixellId,
                        principalTable: "DixellBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Temperature_DixellId",
                table: "Temperature",
                column: "DixellId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alarm");

            migrationBuilder.DropTable(
                name: "DeviceAlarm");

            migrationBuilder.DropTable(
                name: "DixellXR");

            migrationBuilder.DropTable(
                name: "DixellXT");

            migrationBuilder.DropTable(
                name: "Temperature");

            migrationBuilder.DropTable(
                name: "DixellBase");
        }
    }
}
