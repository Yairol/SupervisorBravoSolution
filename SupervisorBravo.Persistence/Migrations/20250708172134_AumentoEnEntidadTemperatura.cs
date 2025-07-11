using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorBravo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AumentoEnEntidadTemperatura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CoolingMeasurement",
                table: "Temperature",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DefrostMeasurement",
                table: "Temperature",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ElectroValveMeasurement",
                table: "Temperature",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoolingMeasurement",
                table: "Temperature");

            migrationBuilder.DropColumn(
                name: "DefrostMeasurement",
                table: "Temperature");

            migrationBuilder.DropColumn(
                name: "ElectroValveMeasurement",
                table: "Temperature");
        }
    }
}
