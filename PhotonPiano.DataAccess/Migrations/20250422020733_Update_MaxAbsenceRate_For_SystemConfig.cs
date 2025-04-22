using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_MaxAbsenceRate_For_SystemConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "ConfigName",
                keyValue: "Tỉ lệ điểm danh tối thiểu để qua",
                column: "ConfigName",
                value: "Tỷ lệ vắng mặt tối đa"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "ConfigName",
                keyValue: "Tỷ lệ vắng mặt tối đa",
                column: "ConfigName",
                value: "Tỉ lệ điểm danh tối thiểu để qua"
            );


        }
    }
}
