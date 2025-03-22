using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_PianoSurveyQuestion_Table_And_Make_Survey_Age_Constriant_Optional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PianoSurveyQuestion (Dictionary<string, object>)");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "SurveyQuestion");

            migrationBuilder.AddColumn<int>(
                name: "MaxAge",
                table: "SurveyQuestion",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinAge",
                table: "SurveyQuestion",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxAge",
                table: "PianoSurvey",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinAge",
                table: "PianoSurvey",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PianoSurveyQuestion",
                columns: table => new
                {
                    SurveyId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PianoSurveyQuestion", x => new { x.SurveyId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_PianoSurveyQuestion_PianoSurvey_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "PianoSurvey",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PianoSurveyQuestion_SurveyQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "SurveyQuestion",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PianoSurveyQuestion_QuestionId",
                table: "PianoSurveyQuestion",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PianoSurveyQuestion");

            migrationBuilder.DropColumn(
                name: "MaxAge",
                table: "SurveyQuestion");

            migrationBuilder.DropColumn(
                name: "MinAge",
                table: "SurveyQuestion");

            migrationBuilder.DropColumn(
                name: "MaxAge",
                table: "PianoSurvey");

            migrationBuilder.DropColumn(
                name: "MinAge",
                table: "PianoSurvey");

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "SurveyQuestion",
                type: "boolean",
                nullable: false,
                defaultValue: true);

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
                name: "IX_PianoSurveyQuestion (Dictionary<string, object>)_SurveyId",
                table: "PianoSurveyQuestion (Dictionary<string, object>)",
                column: "SurveyId");
        }
    }
}
