// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace Olivia.Api.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class ModificationChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Patients_PatientId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_PatientId",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Chats",
                newName: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Chats",
                newName: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_PatientId",
                table: "Chats",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Patients_PatientId",
                table: "Chats",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }
    }
}
