using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Seed_For_Allow_Trial_Slot_SystemConfig_Table : Migration
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
                    "Số buổi học thử",
                    "2",
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
