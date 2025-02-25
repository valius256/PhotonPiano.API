using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxRateIntableTransactionandsystemconfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TaxRate",
                table: "Transaction",
                type: "double precision",
                nullable: true);

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[]
                {
                    "Id", "ConfigName", "ConfigValue", "CreatedAt", "DeletedAt", "RecordStatus", "Role", "UpdatedAt"
                },
                values: new object[,]
                {
                    {
                        new Guid("e533aea8-e162-4c1f-a93a-33cf186ed4b1"), "Thuế suất năm 2025", "0.05",
                        new DateTime(2025, 2, 25, 16, 25, 36, 433, DateTimeKind.Utc).AddTicks(4189), null, 1, 3, null
                    }
                });


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}