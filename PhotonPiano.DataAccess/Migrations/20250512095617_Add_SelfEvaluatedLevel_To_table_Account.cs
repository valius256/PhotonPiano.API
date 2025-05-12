using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_SelfEvaluatedLevel_To_table_Account : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesiredLevel",
                table: "Account");

            migrationBuilder.AddColumn<Guid>(
                name: "SelfEvaluatedLevelId",
                table: "Account",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_SelfEvaluatedLevelId",
                table: "Account",
                column: "SelfEvaluatedLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Level_SelfEvaluatedLevelId",
                table: "Account",
                column: "SelfEvaluatedLevelId",
                principalTable: "Level",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Level_SelfEvaluatedLevelId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_SelfEvaluatedLevelId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "SelfEvaluatedLevelId",
                table: "Account");

            migrationBuilder.AddColumn<int>(
                name: "DesiredLevel",
                table: "Account",
                type: "integer",
                nullable: true);
        }
    }
}
