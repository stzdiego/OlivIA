using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Olivia.Api.Migrations
{
    /// <inheritdoc />
    public partial class DoctorsStartEnd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "End",
                table: "Doctors",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Start",
                table: "Doctors",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "End",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Start",
                table: "Doctors");
        }
    }
}
