using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_TeacherId_Prop_For_Slot_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeacherId",
                table: "Slot",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slot_TeacherId",
                table: "Slot",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Slot_Account_TeacherId",
                table: "Slot",
                column: "TeacherId",
                principalTable: "Account",
                principalColumn: "AccountFirebaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slot_Account_TeacherId",
                table: "Slot");

            migrationBuilder.DropIndex(
                name: "IX_Slot_TeacherId",
                table: "Slot");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Slot");
            
        }
    }
}
