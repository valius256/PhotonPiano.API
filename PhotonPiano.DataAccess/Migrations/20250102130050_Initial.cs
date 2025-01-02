using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "Age", "CreatedAt", "DeletedAt", "Name", "RecordStatus", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("6767b38a-4e8c-4c14-bcab-9d100708c329"), 25, new DateTime(2025, 1, 2, 20, 0, 48, 574, DateTimeKind.Utc).AddTicks(6070), null, "Jane Doe", 1, null },
                    { new Guid("ea3135f6-d123-4f76-8c0c-8ef2951be18c"), 30, new DateTime(2025, 1, 2, 20, 0, 48, 574, DateTimeKind.Utc).AddTicks(4667), null, "John Doe", 1, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
