using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_Table_Level_And_Fix_Seed_Data_For_Table_Level_And_SystemConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "MinimumScore",
                table: "Level");

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumPracticalScore",
                table: "Level",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumTheoreticalScore",
                table: "Level",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
            
            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[]
                {
                    "Id", "ConfigName", "ConfigValue", "CreatedAt", "DeletedAt", "RecordStatus", "Role", "UpdatedAt"
                },
                values: new object[,]
                {
                    {
                        Guid.NewGuid(), "Số học viên tối đa của ca thi", "10",
                        new DateTime(2025, 3, 11, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumPracticalScore",
                table: "Level");

            migrationBuilder.DropColumn(
                name: "MinimumTheoreticalScore",
                table: "Level");

            migrationBuilder.AddColumn<double>(
                name: "MinimumScore",
                table: "Level",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
            
        }
    }
}
