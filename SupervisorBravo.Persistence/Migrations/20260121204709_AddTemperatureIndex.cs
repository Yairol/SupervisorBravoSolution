using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorBravo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTemperatureIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Temperature_DixellId",
                table: "Temperature");

            migrationBuilder.CreateIndex(
                name: "IX_Temperatures_DixellId_MeasurementTime",
                table: "Temperature",
                columns: new[] { "DixellId", "MeasurementTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Temperatures_DixellId_MeasurementTime",
                table: "Temperature");

            migrationBuilder.CreateIndex(
                name: "IX_Temperature_DixellId",
                table: "Temperature",
                column: "DixellId");
        }
    }
}
