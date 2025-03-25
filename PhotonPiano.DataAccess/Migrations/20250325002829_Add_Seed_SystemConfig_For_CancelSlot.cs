using Microsoft.EntityFrameworkCore.Migrations;
using PhotonPiano.DataAccess.Models.Enum;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Seed_SystemConfig_For_CancelSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SystemConfig",  
                columns: new[] { "Id", "ConfigName", "ConfigValue", "Role" , "CreatedAt", "RecordStatus" }, 
                values: new object[] 
                {
                    Guid.NewGuid(),
                    "Lý do hủy tiết", 
                    "[\"Giáo viên bận\", \"Diễn tập phòng cháy, chữa cháy\", \"Phòng bị hỏng\"]", 
                    3,
                    DateTime.UtcNow.AddHours(7),
                    1
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemConfig",
                keyColumn: "ConfigName",
                keyValue: "Lý do hủy tiết"
            );
        }
    }
}
