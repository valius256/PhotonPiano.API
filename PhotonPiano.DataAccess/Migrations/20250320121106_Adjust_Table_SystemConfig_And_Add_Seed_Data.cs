using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_Table_SystemConfig_And_Add_Seed_Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "SystemConfig",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            
            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[]
                {
                    "Id", "ConfigName", "Type", "ConfigValue", "CreatedAt", "DeletedAt", "RecordStatus", "Role", "UpdatedAt"
                },
                values: new object[,]
                {
                    {
                        Guid.NewGuid(), "Số học viên tối đa của ca thi",2, "10",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    },
                    {
                        Guid.NewGuid(), "Số phím của nhạc cụ",2, "88",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    },
                    {
                        Guid.NewGuid(), "Tên nhạc cụ",0, "Piano",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    },
                    {
                        Guid.NewGuid(), "Số câu hỏi tối đa trong 1 bài khảo sát",2, "5",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    },
                    {
                        Guid.NewGuid(), "Sỉ số lớp",2, "20",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    },
                    {
                        Guid.NewGuid(), "Số học viên tối thiểu để mở lớp",2, "5",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    },
                    {
                        Guid.NewGuid(), "Cho phép học vượt level",5, "1",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "SystemConfig");
        }
    }
}
