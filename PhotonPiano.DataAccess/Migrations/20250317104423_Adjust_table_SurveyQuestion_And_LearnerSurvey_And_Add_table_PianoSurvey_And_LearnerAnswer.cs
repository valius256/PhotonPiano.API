using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_table_SurveyQuestion_And_LearnerSurvey_And_Add_table_PianoSurvey_And_LearnerAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearnerSurvey_SurveyQuestion_SurveyQuestionId",
                table: "LearnerSurvey");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearnerSurvey",
                table: "LearnerSurvey");

            migrationBuilder.DropColumn(
                name: "AllowMultipleAnswers",
                table: "SurveyQuestion");

            migrationBuilder.DropColumn(
                name: "AllowMultipleAnswers",
                table: "LearnerSurvey");

            migrationBuilder.DropColumn(
                name: "Answers",
                table: "LearnerSurvey");

            migrationBuilder.DropColumn(
                name: "Options",
                table: "LearnerSurvey");

            migrationBuilder.DropColumn(
                name: "QuestionContent",
                table: "LearnerSurvey");

            migrationBuilder.RenameColumn(
                name: "SurveyQuestionId",
                table: "LearnerSurvey",
                newName: "PianoSurveyId");

            migrationBuilder.RenameIndex(
                name: "IX_LearnerSurvey_SurveyQuestionId",
                table: "LearnerSurvey",
                newName: "IX_LearnerSurvey_PianoSurveyId");

            migrationBuilder.AddColumn<bool>(
                name: "AllowOtherAnswer",
                table: "SurveyQuestion",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "SurveyQuestion",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderIndex",
                table: "SurveyQuestion",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "SurveyQuestion",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "LearnerSurvey",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearnerSurvey",
                table: "LearnerSurvey",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LearnerAnswer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnerSurveyId = table.Column<Guid>(type: "uuid", nullable: false),
                    SurveyQuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Answers = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnerAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearnerAnswer_LearnerSurvey_LearnerSurveyId",
                        column: x => x.LearnerSurveyId,
                        principalTable: "LearnerSurvey",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LearnerAnswer_SurveyQuestion_SurveyQuestionId",
                        column: x => x.SurveyQuestionId,
                        principalTable: "SurveyQuestion",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PianoSurvey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(30)", nullable: false),
                    UpdatedById = table.Column<string>(type: "character varying(30)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PianoSurvey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PianoSurvey_Account_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_PianoSurvey_Account_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                });

            migrationBuilder.CreateTable(
                name: "PianoSurveyQuestion (Dictionary<string, object>)",
                columns: table => new
                {
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SurveyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PianoSurveyQuestion (Dictionary<string, object>)", x => new { x.QuestionId, x.SurveyId });
                    table.ForeignKey(
                        name: "FK_PianoSurveyQuestion (Dictionary<string, object>)_PianoSurve~",
                        column: x => x.SurveyId,
                        principalTable: "PianoSurvey",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PianoSurveyQuestion (Dictionary<string, object>)_SurveyQues~",
                        column: x => x.QuestionId,
                        principalTable: "SurveyQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearnerSurvey_LearnerId",
                table: "LearnerSurvey",
                column: "LearnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LearnerAnswer_LearnerSurveyId",
                table: "LearnerAnswer",
                column: "LearnerSurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_LearnerAnswer_SurveyQuestionId",
                table: "LearnerAnswer",
                column: "SurveyQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PianoSurvey_CreatedById",
                table: "PianoSurvey",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PianoSurvey_UpdatedById",
                table: "PianoSurvey",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PianoSurveyQuestion (Dictionary<string, object>)_SurveyId",
                table: "PianoSurveyQuestion (Dictionary<string, object>)",
                column: "SurveyId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearnerSurvey_PianoSurvey_PianoSurveyId",
                table: "LearnerSurvey",
                column: "PianoSurveyId",
                principalTable: "PianoSurvey",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearnerSurvey_PianoSurvey_PianoSurveyId",
                table: "LearnerSurvey");

            migrationBuilder.DropTable(
                name: "LearnerAnswer");

            migrationBuilder.DropTable(
                name: "PianoSurveyQuestion (Dictionary<string, object>)");

            migrationBuilder.DropTable(
                name: "PianoSurvey");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearnerSurvey",
                table: "LearnerSurvey");

            migrationBuilder.DropIndex(
                name: "IX_LearnerSurvey_LearnerId",
                table: "LearnerSurvey");

            migrationBuilder.DropColumn(
                name: "AllowOtherAnswer",
                table: "SurveyQuestion");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "SurveyQuestion");

            migrationBuilder.DropColumn(
                name: "OrderIndex",
                table: "SurveyQuestion");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SurveyQuestion");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LearnerSurvey");

            migrationBuilder.RenameColumn(
                name: "PianoSurveyId",
                table: "LearnerSurvey",
                newName: "SurveyQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_LearnerSurvey_PianoSurveyId",
                table: "LearnerSurvey",
                newName: "IX_LearnerSurvey_SurveyQuestionId");

            migrationBuilder.AddColumn<bool>(
                name: "AllowMultipleAnswers",
                table: "SurveyQuestion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowMultipleAnswers",
                table: "LearnerSurvey",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "Answers",
                table: "LearnerSurvey",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "Options",
                table: "LearnerSurvey",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "QuestionContent",
                table: "LearnerSurvey",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearnerSurvey",
                table: "LearnerSurvey",
                columns: new[] { "LearnerId", "SurveyQuestionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_LearnerSurvey_SurveyQuestion_SurveyQuestionId",
                table: "LearnerSurvey",
                column: "SurveyQuestionId",
                principalTable: "SurveyQuestion",
                principalColumn: "Id");
        }
    }
}
