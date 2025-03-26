using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Property_Schedule_Description_To_Class : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScheduleDescription",
                table: "Class",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "Id", "ConfigName", "ConfigValue", "Role", "CreatedAt", "RecordStatus" },
                values: new object[]
                {
                    Guid.NewGuid(),
                    "Deadline đổi lớp",
                    "1",
                    3,
                    DateTime.UtcNow.AddHours(7),
                    1
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduleDescription",
                table: "Class");
        }
    }
}
