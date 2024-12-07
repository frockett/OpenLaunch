using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLaunch.Migrations
{
    /// <inheritdoc />
    public partial class AddDisplayNameToFromAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "FromAddresses",
                type: "TEXT",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "FromAddresses");
        }
    }
}
