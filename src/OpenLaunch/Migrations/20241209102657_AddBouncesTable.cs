using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLaunch.Migrations
{
    /// <inheritdoc />
    public partial class AddBouncesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasBounced",
                table: "WaitlistSignups",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Bounces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BouncedSignupId = table.Column<int>(type: "INTEGER", nullable: true),
                    BouncedTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bounces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bounces_WaitlistSignups_BouncedSignupId",
                        column: x => x.BouncedSignupId,
                        principalTable: "WaitlistSignups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bounces_BouncedSignupId",
                table: "Bounces",
                column: "BouncedSignupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bounces");

            migrationBuilder.DropColumn(
                name: "HasBounced",
                table: "WaitlistSignups");
        }
    }
}
