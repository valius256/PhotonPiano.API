using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Password_And_RefreshToken_To_Table_Account : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Transaction",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedById",
                table: "SurveyQuestion",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "SurveyQuestion",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "StudentClass",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StudentFirebaseId",
                table: "StudentClass",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "StudentClass",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "StudentClass",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "SlotStudent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "SlotStudent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "SlotStudent",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "StudentFirebaseId",
                table: "SlotStudent",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "Slot",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CancelById",
                table: "Slot",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "Room",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "Room",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Room",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedById",
                table: "PianoSurvey",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "PianoSurvey",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "AccountFirebaseId",
                table: "Notification",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LearnerId",
                table: "LearnerSurvey",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "FreeSlot",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "EntranceTestStudent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StudentFirebaseId",
                table: "EntranceTestStudent",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "EntranceTestStudent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "EntranceTestStudent",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "EntranceTestResult",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "EntranceTestResult",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "EntranceTestResult",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "EntranceTest",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InstructorId",
                table: "EntranceTest",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "EntranceTest",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "EntranceTest",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "DayOff",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "DayOff",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "DayOff",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "Criteria",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "Criteria",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Criteria",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "Class",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InstructorId",
                table: "Class",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "Class",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Class",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "Article",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "Article",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Article",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedById",
                table: "Application",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "Application",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Application",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedById",
                table: "Application",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountFirebaseId",
                table: "AccountNotification",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "AccountFirebaseId",
                table: "Account",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Account",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Account",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryDate",
                table: "Account",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryDate",
                table: "Account");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Transaction",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedById",
                table: "SurveyQuestion",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "SurveyQuestion",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "StudentClass",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StudentFirebaseId",
                table: "StudentClass",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "StudentClass",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "StudentClass",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "SlotStudent",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "SlotStudent",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "SlotStudent",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "StudentFirebaseId",
                table: "SlotStudent",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "Slot",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CancelById",
                table: "Slot",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "Room",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "Room",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Room",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedById",
                table: "PianoSurvey",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "PianoSurvey",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AccountFirebaseId",
                table: "Notification",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LearnerId",
                table: "LearnerSurvey",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "FreeSlot",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "EntranceTestStudent",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StudentFirebaseId",
                table: "EntranceTestStudent",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "EntranceTestStudent",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "EntranceTestStudent",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "EntranceTestResult",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "EntranceTestResult",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "EntranceTestResult",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "EntranceTest",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InstructorId",
                table: "EntranceTest",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "EntranceTest",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "EntranceTest",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "DayOff",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "DayOff",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "DayOff",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "Criteria",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "Criteria",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Criteria",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "Class",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InstructorId",
                table: "Class",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "Class",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Class",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateById",
                table: "Article",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "Article",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Article",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedById",
                table: "Application",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedById",
                table: "Application",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Application",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedById",
                table: "Application",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountFirebaseId",
                table: "AccountNotification",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AccountFirebaseId",
                table: "Account",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
