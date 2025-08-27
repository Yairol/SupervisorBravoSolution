using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorBravo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPLC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PLCDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ModbusId = table.Column<byte>(type: "smallint", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PLCDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PLCVariable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address = table.Column<int>(type: "integer", nullable: false),
                    IsWritable = table.Column<bool>(type: "boolean", nullable: false),
                    PLCDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    BitIndex = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PLCVariable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PLCVariable_PLCDevices_PLCDeviceId",
                        column: x => x.PLCDeviceId,
                        principalTable: "PLCDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnalogMeasurements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasurementValue = table.Column<double>(type: "double precision", nullable: false),
                    PLCAnalogVariableId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasurementTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalogMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalogMeasurements_PLCVariable_PLCAnalogVariableId",
                        column: x => x.PLCAnalogVariableId,
                        principalTable: "PLCVariable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DigitalMeasurements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasurementValue = table.Column<bool>(type: "boolean", nullable: false),
                    PLCDigitalVariableId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasurementTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DigitalMeasurements_PLCVariable_PLCDigitalVariableId",
                        column: x => x.PLCDigitalVariableId,
                        principalTable: "PLCVariable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalogMeasurements_PLCAnalogVariableId",
                table: "AnalogMeasurements",
                column: "PLCAnalogVariableId");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalMeasurements_PLCDigitalVariableId",
                table: "DigitalMeasurements",
                column: "PLCDigitalVariableId");

            migrationBuilder.CreateIndex(
                name: "IX_PLCVariable_PLCDeviceId",
                table: "PLCVariable",
                column: "PLCDeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalogMeasurements");

            migrationBuilder.DropTable(
                name: "DigitalMeasurements");

            migrationBuilder.DropTable(
                name: "PLCVariable");

            migrationBuilder.DropTable(
                name: "PLCDevices");
        }
    }
}
