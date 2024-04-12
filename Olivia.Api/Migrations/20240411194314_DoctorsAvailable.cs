using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Olivia.Api.Migrations
{
    /// <inheritdoc />
    public partial class DoctorsAvailable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "Doctors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Available",
                table: "Doctors");
        }
    }
}
