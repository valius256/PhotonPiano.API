using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Additional_Properties_For_Table_SlotStudent_To_Tutor_Flow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TeacherNote",
                table: "SlotStudent",
                newName: "PedalComment");

            migrationBuilder.AddColumn<string>(
                name: "AttendanceComment",
                table: "SlotStudent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FingerNoteComment",
                table: "SlotStudent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GestureComment",
                table: "SlotStudent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GestureUrl",
                table: "SlotStudent",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttendanceComment",
                table: "SlotStudent");

            migrationBuilder.DropColumn(
                name: "FingerNoteComment",
                table: "SlotStudent");

            migrationBuilder.DropColumn(
                name: "GestureComment",
                table: "SlotStudent");

            migrationBuilder.DropColumn(
                name: "GestureUrl",
                table: "SlotStudent");

            migrationBuilder.RenameColumn(
                name: "PedalComment",
                table: "SlotStudent",
                newName: "TeacherNote");
        }
    }
}
