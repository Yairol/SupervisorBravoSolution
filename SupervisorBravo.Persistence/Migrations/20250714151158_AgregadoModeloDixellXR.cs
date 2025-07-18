using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorBravo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregadoModeloDixellXR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "modelName",
                table: "DixellXR",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "modelName",
                table: "DixellXR");
        }
    }
}
