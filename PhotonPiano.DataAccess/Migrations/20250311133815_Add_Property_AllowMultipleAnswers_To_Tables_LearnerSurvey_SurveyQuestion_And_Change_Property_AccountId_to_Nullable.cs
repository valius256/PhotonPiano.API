using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Property_AllowMultipleAnswers_To_Tables_LearnerSurvey_SurveyQuestion_And_Change_Property_AccountId_to_Nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowMultipleAnswers",
                table: "SurveyQuestion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "LearnerSurvey",
                type: "character varying(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AddColumn<bool>(
                name: "AllowMultipleAnswers",
                table: "LearnerSurvey",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowMultipleAnswers",
                table: "SurveyQuestion");

            migrationBuilder.DropColumn(
                name: "AllowMultipleAnswers",
                table: "LearnerSurvey");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "LearnerSurvey",
                type: "character varying(30)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldNullable: true);
        }
    }
}
