using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorBravo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CambiodeNombres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "DixellXT111C",
                newName: "DixellXT");

            migrationBuilder.RenameTable(
                name: "DixellXR60CX",
                newName: "DixellXR");
            migrationBuilder.AddColumn<bool>(
                name: "ElectroValve",
                table: "DixellXT",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Cooling",
                table: "DixellXR",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Fan",
                table: "DixellXR",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectroValve",
                table: "DixellXT");

            migrationBuilder.DropColumn(
                name: "Cooling",
                table: "DixellXR");

            migrationBuilder.DropColumn(
                name: "Fan",
                table: "DixellXR");
            // Revertir los nombres de las tablas
            migrationBuilder.RenameTable(
                name: "DixellXT",
                newName: "DixellXT111C");

            migrationBuilder.RenameTable(
                name: "DixellXR",
                newName: "DixellXR60CX");
        }
    }
}
