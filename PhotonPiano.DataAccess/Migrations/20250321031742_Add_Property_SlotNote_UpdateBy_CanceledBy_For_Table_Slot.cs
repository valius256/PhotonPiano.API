using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Property_SlotNote_UpdateBy_CanceledBy_For_Table_Slot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelById",
                table: "Slot",
                type: "character varying(30)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SlotNote",
                table: "Slot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateById",
                table: "Slot",
                type: "character varying(30)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slot_CancelById",
                table: "Slot",
                column: "CancelById");

            migrationBuilder.CreateIndex(
                name: "IX_Slot_UpdateById",
                table: "Slot",
                column: "UpdateById");

            migrationBuilder.AddForeignKey(
                name: "FK_Slot_Account_CancelById",
                table: "Slot",
                column: "CancelById",
                principalTable: "Account",
                principalColumn: "AccountFirebaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Slot_Account_UpdateById",
                table: "Slot",
                column: "UpdateById",
                principalTable: "Account",
                principalColumn: "AccountFirebaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slot_Account_CancelById",
                table: "Slot");

            migrationBuilder.DropForeignKey(
                name: "FK_Slot_Account_UpdateById",
                table: "Slot");

            migrationBuilder.DropIndex(
                name: "IX_Slot_CancelById",
                table: "Slot");

            migrationBuilder.DropIndex(
                name: "IX_Slot_UpdateById",
                table: "Slot");

            migrationBuilder.DropColumn(
                name: "CancelById",
                table: "Slot");

            migrationBuilder.DropColumn(
                name: "SlotNote",
                table: "Slot");

            migrationBuilder.DropColumn(
                name: "UpdateById",
                table: "Slot");
        }
    }
}
