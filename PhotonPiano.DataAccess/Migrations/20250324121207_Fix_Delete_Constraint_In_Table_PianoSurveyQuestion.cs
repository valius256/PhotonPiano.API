using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Delete_Constraint_In_Table_PianoSurveyQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PianoSurveyQuestion_PianoSurvey_SurveyId",
                table: "PianoSurveyQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_PianoSurveyQuestion_SurveyQuestion_QuestionId",
                table: "PianoSurveyQuestion");

            migrationBuilder.AddForeignKey(
                name: "FK_PianoSurveyQuestion_PianoSurvey_SurveyId",
                table: "PianoSurveyQuestion",
                column: "SurveyId",
                principalTable: "PianoSurvey",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PianoSurveyQuestion_SurveyQuestion_QuestionId",
                table: "PianoSurveyQuestion",
                column: "QuestionId",
                principalTable: "SurveyQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PianoSurveyQuestion_PianoSurvey_SurveyId",
                table: "PianoSurveyQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_PianoSurveyQuestion_SurveyQuestion_QuestionId",
                table: "PianoSurveyQuestion");

            migrationBuilder.AddForeignKey(
                name: "FK_PianoSurveyQuestion_PianoSurvey_SurveyId",
                table: "PianoSurveyQuestion",
                column: "SurveyId",
                principalTable: "PianoSurvey",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PianoSurveyQuestion_SurveyQuestion_QuestionId",
                table: "PianoSurveyQuestion",
                column: "QuestionId",
                principalTable: "SurveyQuestion",
                principalColumn: "Id");
        }
    }
}
