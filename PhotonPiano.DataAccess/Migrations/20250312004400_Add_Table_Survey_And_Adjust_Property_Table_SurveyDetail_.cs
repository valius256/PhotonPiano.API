using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Table_Survey_And_Adjust_Property_Table_SurveyDetail_ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearnerSurvey");

            migrationBuilder.CreateTable(
                name: "Survey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreateById = table.Column<string>(type: "character varying(30)", nullable: false),
                    UpdateById = table.Column<string>(type: "character varying(30)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Survey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Survey_Account_CreateById",
                        column: x => x.CreateById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Survey_Account_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                });

            migrationBuilder.CreateTable(
                name: "SurveyDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnerId = table.Column<string>(type: "character varying(30)", nullable: true),
                    LearnerEmail = table.Column<string>(type: "text", nullable: true),
                    SurveyName = table.Column<string>(type: "text", nullable: false),
                    SurveyQuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionContent = table.Column<string>(type: "text", nullable: false),
                    Answers = table.Column<List<string>>(type: "text[]", nullable: false),
                    Options = table.Column<List<string>>(type: "text[]", nullable: false),
                    AllowMultipleAnswers = table.Column<bool>(type: "boolean", nullable: false),
                    AnswerAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyDetails_Account_LearnerId",
                        column: x => x.LearnerId,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_SurveyDetails_SurveyQuestion_SurveyQuestionId",
                        column: x => x.SurveyQuestionId,
                        principalTable: "SurveyQuestion",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Survey_CreateById",
                table: "Survey",
                column: "CreateById");

            migrationBuilder.CreateIndex(
                name: "IX_Survey_UpdateById",
                table: "Survey",
                column: "UpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetails_LearnerId",
                table: "SurveyDetails",
                column: "LearnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetails_SurveyQuestionId",
                table: "SurveyDetails",
                column: "SurveyQuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Survey");

            migrationBuilder.DropTable(
                name: "SurveyDetails");

            migrationBuilder.CreateTable(
                name: "LearnerSurvey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<string>(type: "character varying(30)", nullable: true),
                    SurveyQuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllowMultipleAnswers = table.Column<bool>(type: "boolean", nullable: false),
                    Answers = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Options = table.Column<List<string>>(type: "text[]", nullable: false),
                    QuestionContent = table.Column<string>(type: "text", nullable: false),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnerSurvey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearnerSurvey_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_LearnerSurvey_SurveyQuestion_SurveyQuestionId",
                        column: x => x.SurveyQuestionId,
                        principalTable: "SurveyQuestion",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearnerSurvey_AccountId",
                table: "LearnerSurvey",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_LearnerSurvey_SurveyQuestionId",
                table: "LearnerSurvey",
                column: "SurveyQuestionId");
        }
    }
}
