using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_Survey_Module_And_Add_Seed_Data_For_SystemConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyDetails");

            migrationBuilder.CreateTable(
                name: "LearnerSurvey",
                columns: table => new
                {
                    LearnerId = table.Column<string>(type: "character varying(30)", nullable: false),
                    SurveyQuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnerEmail = table.Column<string>(type: "text", nullable: false),
                    QuestionContent = table.Column<string>(type: "text", nullable: false),
                    Answers = table.Column<List<string>>(type: "text[]", nullable: false),
                    Options = table.Column<List<string>>(type: "text[]", nullable: false),
                    AllowMultipleAnswers = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnerSurvey", x => new { x.LearnerId, x.SurveyQuestionId });
                    table.ForeignKey(
                        name: "FK_LearnerSurvey_Account_LearnerId",
                        column: x => x.LearnerId,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_LearnerSurvey_SurveyQuestion_SurveyQuestionId",
                        column: x => x.SurveyQuestionId,
                        principalTable: "SurveyQuestion",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearnerSurvey_SurveyQuestionId",
                table: "LearnerSurvey",
                column: "SurveyQuestionId");
            
            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[]
                {
                    "Id", "ConfigName", "ConfigValue", "CreatedAt", "DeletedAt", "RecordStatus", "Role", "UpdatedAt"
                },
                values: new object[,]
                {
                    {
                        Guid.NewGuid(), "Điểm yêu cầu của level 2", "2.5",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    },
                    {
                        Guid.NewGuid(), "Điểm yêu cầu của level 3", "5.0",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    },
                    {
                        Guid.NewGuid(), "Điểm yêu cầu của level 4", "7.5",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    },
                    {
                    Guid.NewGuid(), "Điểm yêu cầu của level 5", "9.5",
                    new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearnerSurvey");

            migrationBuilder.CreateTable(
                name: "SurveyDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnerId = table.Column<string>(type: "character varying(30)", nullable: true),
                    SurveyQuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllowMultipleAnswers = table.Column<bool>(type: "boolean", nullable: false),
                    AnswerAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Answers = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LearnerEmail = table.Column<string>(type: "text", nullable: true),
                    Options = table.Column<List<string>>(type: "text[]", nullable: false),
                    QuestionContent = table.Column<string>(type: "text", nullable: false),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false),
                    SurveyName = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "IX_SurveyDetails_LearnerId",
                table: "SurveyDetails",
                column: "LearnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetails_SurveyQuestionId",
                table: "SurveyDetails",
                column: "SurveyQuestionId");
        }
    }
}
