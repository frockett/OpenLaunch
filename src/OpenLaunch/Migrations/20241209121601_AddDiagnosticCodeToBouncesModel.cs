using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLaunch.Migrations
{
    /// <inheritdoc />
    public partial class AddDiagnosticCodeToBouncesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiagnosticCode",
                table: "Bounces",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiagnosticCode",
                table: "Bounces");
        }
    }
}
