using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Seed_For_TuitionDates_Config_For_SystemConfig_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "Id", "ConfigName", "ConfigValue", "Role", "CreatedAt", "RecordStatus" },
                values: new object[,]
                {
                    {
                        Guid.NewGuid(),
                        "Hạn chót thanh toán học phí",
                        "28",
                        3,
                        DateTime.UtcNow.AddHours(7),
                        1
                    },
                    {
                        Guid.NewGuid(),
                        "Ngày nhắc thanh toán học phí",
                        "25",
                        3,
                        DateTime.UtcNow.AddHours(7),
                        1
                    },
                    {
                        Guid.NewGuid(),
                        "Ngày xoá học sinh khi không đóng",
                        "29",
                        3,
                        DateTime.UtcNow.AddHours(7),
                        1
                    }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemConfig",
                keyColumn: "ConfigName",
                keyValues: new object[]
                {
                    "Hạn chót thanh toán học phí",
                    "Ngày nhắc thanh toán học phí",
                    "Ngày xoá học sinh khi không đóng"
                }
            );
        }
    }
}
