using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Level_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "EntranceTestStudent");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Account");

            migrationBuilder.AddColumn<Guid>(
                name: "LevelId",
                table: "EntranceTestStudent",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LevelId",
                table: "Class",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LevelId",
                table: "Account",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Level",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SkillsEarned = table.Column<List<string>>(type: "text[]", nullable: false),
                    SlotPerWeek = table.Column<int>(type: "integer", nullable: false),
                    TotalSlots = table.Column<int>(type: "integer", nullable: false),
                    PricePerSlot = table.Column<decimal>(type: "numeric", nullable: false),
                    MinimumScore = table.Column<double>(type: "double precision", nullable: false),
                    IsGenreDivided = table.Column<bool>(type: "boolean", nullable: false),
                    NextLevelId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Level_Level_NextLevelId",
                        column: x => x.NextLevelId,
                        principalTable: "Level",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            
            migrationBuilder.InsertData(
                table: "Level",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Description", "IsGenreDivided", "MinimumTheoreticalScore","MinimumPracticalScore", "Name", "NextLevelId", "PricePerSlot", "RecordStatus", "SkillsEarned", "SlotPerWeek", "TotalSlots", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("55974743-7c93-47ab-877e-eda4cb9f96c5"), new DateTime(2025, 3, 15, 1, 4, 37, 135, DateTimeKind.Utc).AddTicks(1814), null, "Học viên đạt đến trình độ chuyên nghiệp, có thể biểu diễn trên sân khấu và thể hiện cá tính âm nhạc của mình.\r\n\r\n", false,0.0,0.0, "Chuyên nghiệp (Professional/Master Level)", null, 500000m, 1, new List<string> { "Chơi những tác phẩm khó và yêu cầu kỹ thuật phức tạp như Rachmaninoff, Liszt, Debussy...", "Biểu diễn tự tin trên sân khấu với phong cách cá nhân.", "Ứng biến (improvisation) và sáng tạo trong cách chơi.", "Phối hợp với các nhạc cụ khác trong một dàn nhạc hoặc ban nhạc.", "Sáng tác và phối nhạc theo phong cách riêng.", "Kỹ năng giảng dạy piano cho người khác (nếu theo hướng sư phạm)." }, 2, 50, null },
                    { new Guid("0d232654-2ce8-4193-9bc3-acd3eddf2ff2"), new DateTime(2025, 3, 15, 1, 4, 37, 135, DateTimeKind.Utc).AddTicks(1808), null, "Học viên có thể chơi những tác phẩm phức tạp và yêu cầu kỹ thuật cao, đồng thời thể hiện cảm xúc qua từng giai điệu.", false, 2.0,2.0, "Cao cấp (Advanced)", new Guid("55974743-7c93-47ab-877e-eda4cb9f96c5"), 400000m, 1, new List<string> { "Chơi các tác phẩm cổ điển của các nhà soạn nhạc như Mozart, Beethoven, Chopin...", "Kỹ thuật legato, staccato, trill, tremolo ở mức độ cao.", "Kiểm soát lực đánh phím để tạo độ sâu và sắc thái âm nhạc phong phú.", "Thị tấu ở tốc độ cao hơn và có độ chính xác cao.", "Sử dụng pedal một cách chuyên nghiệp để nâng cao hiệu ứng âm thanh.", "Khả năng chơi piano với nhiều thể loại khác nhau (cổ điển, jazz, pop, đệm hát...)." }, 2, 40, null },
                    { new Guid("3db94220-15db-4880-9b57-b6064b27c11b"), new DateTime(2025, 3, 15, 1, 4, 37, 135, DateTimeKind.Utc).AddTicks(1801), null, "Học viên đã có nền tảng vững chắc và bắt đầu chơi các bản nhạc phức tạp hơn với nhiều sắc thái biểu cảm.", false, 4.0,4.0, "Trung cấp (Intermediate)", new Guid("0d232654-2ce8-4193-9bc3-acd3eddf2ff2"), 300000m, 1, new List<string> { "Chơi các bản nhạc có tiết tấu nhanh hơn và nhiều kỹ thuật hơn.", "Sử dụng pedal một cách linh hoạt để tạo hiệu ứng âm thanh tốt hơn.", "Thành thạo các quãng (intervals) và hợp âm 7.", "Chơi các gam (scale) và arpeggio ở nhiều tốc độ khác nhau.", "Đọc bản nhạc nhanh hơn và luyện tập khả năng thị tấu (sight-reading).", "Phát triển phong cách biểu diễn cá nhân." }, 2, 40, null },
                    { new Guid("ad04326c-4d91-4d67-bc76-2c1dfb87c2c5"), new DateTime(2025, 3, 15, 1, 4, 37, 135, DateTimeKind.Utc).AddTicks(1780), null, "Học viên có thể chơi những bài nhạc đơn giản và bắt đầu làm quen với cách sử dụng pedal.", false, 6.0,6.0, "Tiền trung cấp (Elementary/Pre-Intermediate)", new Guid("3db94220-15db-4880-9b57-b6064b27c11b"), 250000m, 1, new List<string> { "Chơi những bản nhạc có hai tay độc lập với các tiết tấu khác nhau.", "Hiểu và áp dụng dấu hóa (♯, ♭) vào bài nhạc.", "Chuyển đổi hợp âm cơ bản và đệm đàn đơn giản.", "Sử dụng pedal sustain một cách cơ bản.", "Phát triển khả năng cảm âm và điều chỉnh lực đánh phím." }, 2, 30, null },
                    { new Guid("8fb54d6e-315c-470d-825e-91b8314134f7"), new DateTime(2025, 3, 15, 1, 4, 37, 134, DateTimeKind.Utc).AddTicks(9354), null, "Học viên mới bắt đầu làm quen với piano, nhận biết các phím đàn, nốt nhạc và tư thế ngồi đúng cách", false, 8.0,8.0, "Sơ cấp (Beginner)", new Guid("ad04326c-4d91-4d67-bc76-2c1dfb87c2c5"), 200000m, 1, new List<string> { "Hiểu biết cơ bản về bàn phím piano và vị trí các nốt nhạc.", "Đọc nốt nhạc trên khóa Sol và khóa Fa.", "Rèn luyện ngón tay với các bài tập đơn giản.", "Chơi các giai điệu cơ bản bằng cả hai tay.", "Nhận biết nhịp điệu đơn giản (2/4, 3/4, 4/4).", "Áp dụng kỹ thuật legato (chơi liền tiếng) và staccato (chơi ngắt tiếng) cơ bản." }, 2, 30, null }
                });

            migrationBuilder.UpdateData(
                table: "Class",
                keyColumn: "Id",
                keyValue: "497028f7-992a-4f3a-91dc-236d333532e5",
                column: "LevelId",
                value: "55974743-7c93-47ab-877e-eda4cb9f96c5"
            );

            migrationBuilder.UpdateData(
                table: "Class",
                keyColumn: "Id",
                keyValue: "ac3a6dc1-55ad-485f-bcd6-c544c68f1491",
                column: "LevelId",
                value: "0d232654-2ce8-4193-9bc3-acd3eddf2ff2"
            );

            migrationBuilder.UpdateData(
                table: "Class",
                keyColumn: "Id",
                keyValue: "d9366e70-4f72-4684-9bac-a0922a78da5d",
                column: "LevelId",
                value: "3db94220-15db-4880-9b57-b6064b27c11b"
            );

            migrationBuilder.UpdateData(
                table: "Class",
                keyColumn: "Id",
                keyValue: "f2c1bdd2-d412-4abc-9311-162a28e12d82",
                column: "LevelId",
                value: "ad04326c-4d91-4d67-bc76-2c1dfb87c2c5"
            );

            migrationBuilder.UpdateData(
                table: "Class",
                keyColumn: "Id",
                keyValue: "f838e840-e4e5-48ae-aa09-fcb36638a698",
                column: "LevelId",
                value: "8fb54d6e-315c-470d-825e-91b8314134f7"
            );


            migrationBuilder.CreateIndex(
                name: "IX_EntranceTestStudent_LevelId",
                table: "EntranceTestStudent",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Class_LevelId",
                table: "Class",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_LevelId",
                table: "Account",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Level_NextLevelId",
                table: "Level",
                column: "NextLevelId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Level_LevelId",
                table: "Account",
                column: "LevelId",
                principalTable: "Level",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Class_Level_LevelId",
                table: "Class",
                column: "LevelId",
                principalTable: "Level",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntranceTestStudent_Level_LevelId",
                table: "EntranceTestStudent",
                column: "LevelId",
                principalTable: "Level",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Level_LevelId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Class_Level_LevelId",
                table: "Class");

            migrationBuilder.DropForeignKey(
                name: "FK_EntranceTestStudent_Level_LevelId",
                table: "EntranceTestStudent");

            migrationBuilder.DropTable(
                name: "Level");

            migrationBuilder.DropIndex(
                name: "IX_EntranceTestStudent_LevelId",
                table: "EntranceTestStudent");

            migrationBuilder.DropIndex(
                name: "IX_Class_LevelId",
                table: "Class");

            migrationBuilder.DropIndex(
                name: "IX_Account_LevelId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "EntranceTestStudent");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "Account");

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "EntranceTestStudent",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Class",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Account",
                type: "integer",
                nullable: true);
        }
    }
}
