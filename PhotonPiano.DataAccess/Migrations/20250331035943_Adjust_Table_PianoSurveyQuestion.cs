using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_Table_PianoSurveyQuestion : Migration
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_PianoSurveyQuestion",
                table: "PianoSurveyQuestion");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PianoSurveyQuestion",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PianoSurveyQuestion",
                table: "PianoSurveyQuestion",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PianoSurveyQuestion_SurveyId",
                table: "PianoSurveyQuestion",
                column: "SurveyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PianoSurveyQuestion_PianoSurvey_SurveyId",
                table: "PianoSurveyQuestion",
                column: "SurveyId",
                principalTable: "PianoSurvey",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PianoSurveyQuestion_SurveyQuestion_QuestionId",
                table: "PianoSurveyQuestion",
                column: "QuestionId",
                principalTable: "SurveyQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_PianoSurveyQuestion",
                table: "PianoSurveyQuestion");

            migrationBuilder.DropIndex(
                name: "IX_PianoSurveyQuestion_SurveyId",
                table: "PianoSurveyQuestion");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PianoSurveyQuestion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PianoSurveyQuestion",
                table: "PianoSurveyQuestion",
                columns: new[] { "SurveyId", "QuestionId" });

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
    }
}
