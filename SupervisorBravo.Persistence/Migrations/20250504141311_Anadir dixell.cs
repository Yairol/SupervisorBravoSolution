using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorBravo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Anadirdixell : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DixellXT111C",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ControlON_OFF = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DixellXT111C", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DixellXT111C_DixellBase_Id",
                        column: x => x.Id,
                        principalTable: "DixellBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DixellXT111C");
        }
    }
}
