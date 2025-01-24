using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemConfigSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "Id", "ConfigName", "ConfigValue", "CreatedAt", "DeletedAt", "RecordStatus", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0d54bb27-d2cb-4b5f-a0e2-49206fbfb27d"), "Số buổi học 1 tuần LEVEL 2", "2", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(3989), null, 1, 3, null },
                    { new Guid("148c2153-f499-4c69-b523-71fb797ac7b9"), "Tổng số buổi học LEVEL 3", "30", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(4009), null, 1, 3, null },
                    { new Guid("2684678d-b4c5-428a-99be-73a9bd2264b8"), "Tổng số buổi học LEVEL 1", "30", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(4004), null, 1, 3, null },
                    { new Guid("3584df2a-635c-4c1a-b9eb-69fdb3d8b318"), "Số buổi học 1 tuần LEVEL 3", "2", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(3996), null, 1, 3, null },
                    { new Guid("408772ae-6a21-4e7b-afb6-90fb1efb8471"), "Số buổi học 1 tuần LEVEL 4", "2", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(3998), null, 1, 3, null },
                    { new Guid("4f8714ee-65b7-4f4c-94cb-bd89bdcd874b"), "Tổng số buổi học LEVEL 4", "40", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(4011), null, 1, 3, null },
                    { new Guid("55c13d5f-d645-4142-8f1b-2d9962b26781"), "Số buổi học 1 tuần LEVEL 1", "2", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(3986), null, 1, 3, null },
                    { new Guid("6b1f7ad0-5997-4432-ae22-c55b5a2b37e8"), "Sĩ số lớp tối đa", "12", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(3968), null, 1, 3, null },
                    { new Guid("738f9e3c-d4e0-45d5-94c3-60735ef03806"), "Mức phí theo buổi LEVEL 4", "350000", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(3981), null, 1, 3, null },
                    { new Guid("8916d3a5-9d62-4057-94c3-e75f55fcf5e6"), "Mức phí theo buổi LEVEL 1", "200000", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(3973), null, 1, 3, null },
                    { new Guid("916156e5-6c14-49d5-a15a-61e49e988c04"), "Mức phí theo buổi LEVEL 3", "300000", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(3978), null, 1, 3, null },
                    { new Guid("92b27b87-fecd-4fc0-a89d-ed3498417e68"), "Tổng số buổi học LEVEL 2", "30", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(4006), null, 1, 3, null },
                    { new Guid("a1304324-ba26-4e5a-882b-42df57530906"), "Mức phí theo buổi LEVEL 5", "400000", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(3984), null, 1, 3, null },
                    { new Guid("ce1a7eae-d869-4d0f-99ef-d5ca659b6bfb"), "Mức phí theo buổi LEVEL 2", "250000", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(3976), null, 1, 3, null },
                    { new Guid("e95a6f70-6f22-4bfb-9bfa-3de2d04f8d7e"), "Sĩ số lớp tối thiểu", "8", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(3241), null, 1, 3, null },
                    { new Guid("eae5307a-87b4-4463-8447-2cb36114ae3d"), "Tổng số buổi học LEVEL 5", "50", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(4014), null, 1, 3, null },
                    { new Guid("fa240bf5-f7c6-4b60-8cdb-e1f3556f55c1"), "Số buổi học 1 tuần LEVEL 5", "2", new DateTime(2025, 1, 25, 0, 44, 39, 854, DateTimeKind.Utc).AddTicks(4001), null, 1, 3, null }
                });

           }
    }
}
