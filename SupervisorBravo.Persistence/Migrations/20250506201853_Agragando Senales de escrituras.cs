using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorBravo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgragandoSenalesdeescrituras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ControlON_OFF",
                table: "DixellXT111C");

            migrationBuilder.RenameColumn(
                name: "ControlON_OFF",
                table: "DixellXR60CX",
                newName: "ThawingWrite");

            migrationBuilder.AddColumn<bool>(
                name: "ControlEnable",
                table: "Temperature",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ControlON_OFF",
                table: "DixellBase",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ControlON_OFFWrite",
                table: "DixellBase",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SetPointWrite",
                table: "DixellBase",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ControlEnable",
                table: "Temperature");

            migrationBuilder.DropColumn(
                name: "ControlON_OFF",
                table: "DixellBase");

            migrationBuilder.DropColumn(
                name: "ControlON_OFFWrite",
                table: "DixellBase");

            migrationBuilder.DropColumn(
                name: "SetPointWrite",
                table: "DixellBase");

            migrationBuilder.RenameColumn(
                name: "ThawingWrite",
                table: "DixellXR60CX",
                newName: "ControlON_OFF");

            migrationBuilder.AddColumn<bool>(
                name: "ControlON_OFF",
                table: "DixellXT111C",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
