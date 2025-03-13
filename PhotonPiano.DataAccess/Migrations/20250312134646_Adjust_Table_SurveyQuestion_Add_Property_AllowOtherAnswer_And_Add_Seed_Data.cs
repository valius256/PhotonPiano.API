using Microsoft.EntityFrameworkCore.Migrations;
using PhotonPiano.DataAccess.Models.Enum;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_Table_SurveyQuestion_Add_Property_AllowOtherAnswer_And_Add_Seed_Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "AllowMultipleAnswers",
                table: "SurveyQuestion",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<bool>(
                name: "AllowOtherAnswer",
                table: "SurveyQuestion",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            
             migrationBuilder.InsertData(
                table: "SurveyQuestion",
                columns: new[]
                {
                    "Id", "QuestionContent", "Options", "CreatedById", "CreatedAt", "AllowMultipleAnswers", "AllowOtherAnswer", "RecordStatus"
                },
                values: new object[,]
                {
                    {
                        Guid.NewGuid(),
                        "Bạn có kinh nghiệm chơi piano chưa nhỉ?",
                        new List<string>()
                        {
                            "Chưa từng chạm đến piano",
                            "Từng chơi qua một chút",
                            "Chơi qua một thời gian",
                            "Chơi rất lâu rồi",
                            "Chuyên nghiệp",
                            "Đã từng biểu diễn trên sân khấu"
                        },
                        "gnRssA2sZHWnXB23oUuUxwz95Ln1",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189),
                        false,
                        false,
                        1
                    },
                    {
                        Guid.NewGuid(),
                        "Dòng nhạc yêu thích của bạn là gì?",
                        new List<string>()
                        {
                            "Cổ điển",
                            "Nhạc Pop",
                            "Jazz & Blue",
                            "Country & Folk",
                            "Nhạc chủ để phim hoặc game"
                        },
                        "gnRssA2sZHWnXB23oUuUxwz95Ln1",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189),
                        true,
                        true,
                        1
                    },
                    {
                        Guid.NewGuid(),
                        "Bạn ưa thích học theo phương pháp nào?",
                        new List<string>()
                        {
                            "Hướng dẫn 1-1",
                            "Học theo nhóm",
                            "Sao cũng được",
                        },
                        "gnRssA2sZHWnXB23oUuUxwz95Ln1",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189),
                        true,
                        true,
                        1
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowOtherAnswer",
                table: "SurveyQuestion");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowMultipleAnswers",
                table: "SurveyQuestion",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
