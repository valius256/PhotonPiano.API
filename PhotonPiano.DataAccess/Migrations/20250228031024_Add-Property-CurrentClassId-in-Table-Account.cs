using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyCurrentClassIdinTableAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentClassId",
                table: "Account",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_CurrentClassId",
                table: "Account",
                column: "CurrentClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Class_CurrentClassId",
                table: "Account",
                column: "CurrentClassId",
                principalTable: "Class",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Class_CurrentClassId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_CurrentClassId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "CurrentClassId",
                table: "Account");
        }
    }
}
