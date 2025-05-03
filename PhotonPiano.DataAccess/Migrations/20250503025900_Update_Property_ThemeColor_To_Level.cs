using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_Property_ThemeColor_To_Level : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "0d232654-2ce8-4193-9bc3-acd3eddf2ff2",
                column: "ThemeColor",
                value: "#c48321"
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "3db94220-15db-4880-9b57-b6064b27c11b",
                column: "ThemeColor",
                value: "#c1c421"
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "55974743-7c93-47ab-877e-eda4cb9f96c5",
                column: "ThemeColor",
                value: "#c43921"
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "8fb54d6e-315c-470d-825e-91b8314134f7",
                column: "ThemeColor",
                value: "#21c44d"
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "ad04326c-4d91-4d67-bc76-2c1dfb87c2c5",
                column: "ThemeColor",
                value: "#7bc421"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
