using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorBravo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateDataAnalysisTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnalysisDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataAnalyses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnalyzedData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Mean = table.Column<double>(type: "double precision", nullable: false),
                    Median = table.Column<double>(type: "double precision", nullable: false),
                    Minimum = table.Column<double>(type: "double precision", nullable: false),
                    Maximum = table.Column<double>(type: "double precision", nullable: false),
                    StandardDeviation = table.Column<double>(type: "double precision", nullable: false),
                    DiscardedByIQR = table.Column<int>(type: "integer", nullable: false),
                    DataAnalysisId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyzedData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalyzedData_DataAnalyses_DataAnalysisId",
                        column: x => x.DataAnalysisId,
                        principalTable: "DataAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyzedData_DataAnalysisId",
                table: "AnalyzedData",
                column: "DataAnalysisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalyzedData");

            migrationBuilder.DropTable(
                name: "DataAnalyses");
        }
    }
}
