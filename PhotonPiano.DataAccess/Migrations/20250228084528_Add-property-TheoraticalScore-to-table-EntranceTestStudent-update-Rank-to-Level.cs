using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddpropertyTheoraticalScoretotableEntranceTestStudentupdateRanktoLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rank",
                table: "EntranceTestStudent",
                newName: "Level");

            migrationBuilder.AddColumn<double>(
                name: "TheoraticalScore",
                table: "EntranceTestStudent",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TheoraticalScore",
                table: "EntranceTestStudent");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "EntranceTestStudent",
                newName: "Rank");
        }
    }
}
