using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Table_Survey_And_Table_Learner_Surver_Adjust_Staff_Confirm_Note_Table_Application_To_String : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StaffConfirmNote",
                table: "Application",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "json",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SurveyQuestion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionContent = table.Column<string>(type: "text", nullable: false),
                    Options = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(30)", nullable: false),
                    UpdatedById = table.Column<string>(type: "character varying(30)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyQuestion_Account_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_SurveyQuestion_Account_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                });

            migrationBuilder.CreateTable(
                name: "LearnerSurvey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<string>(type: "character varying(30)", nullable: false),
                    SurveyQuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionContent = table.Column<string>(type: "text", nullable: false),
                    Answers = table.Column<List<string>>(type: "text[]", nullable: false),
                    Options = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_SurveyQuestion_CreatedById",
                table: "SurveyQuestion",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyQuestion_UpdatedById",
                table: "SurveyQuestion",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearnerSurvey");

            migrationBuilder.DropTable(
                name: "SurveyQuestion");

            migrationBuilder.AlterColumn<string>(
                name: "StaffConfirmNote",
                table: "Application",
                type: "json",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
