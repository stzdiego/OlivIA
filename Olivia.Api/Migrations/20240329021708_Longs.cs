using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Olivia.Api.Migrations
{
    /// <inheritdoc />
    public partial class Longs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Identification",
                table: "Doctors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Identification",
                table: "Doctors");
        }
    }
}
