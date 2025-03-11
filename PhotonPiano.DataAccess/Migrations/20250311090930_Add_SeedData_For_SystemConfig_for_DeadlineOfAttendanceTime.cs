using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_SeedData_For_SystemConfig_for_DeadlineOfAttendanceTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[]
                {
                    "Id", "ConfigName", "ConfigValue", "CreatedAt", "DeletedAt", "RecordStatus", "Role", "UpdatedAt"
                },
                values: new object[,]
                {
                    {
                        new Guid("e533aea8-e162-4c1f-a93a-33cf186ed4b6"), "Deadline cho điểm danh 2025", "23:59:59",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
