using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Seed_For_Calculate_Attendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "Id", "ConfigName", "ConfigValue", "Role", "Type", "CreatedAt", "RecordStatus" },
                values: new object[]
                {
                    Guid.NewGuid(),
                    "Tỉ lệ điểm danh tối thiểu để qua",
                    "0.2",
                    3,
                    5,
                    DateTime.UtcNow.AddHours(7),
                    1
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
