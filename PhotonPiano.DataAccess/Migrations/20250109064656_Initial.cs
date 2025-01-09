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
                    AccountFirebaseId = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    BankAccount = table.Column<string>(type: "text", nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShortDescription = table.Column<string>(type: "text", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountFirebaseId);
                });

            migrationBuilder.CreateTable(
                name: "Criteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(30)", nullable: false),
                    UpdateById = table.Column<string>(type: "character varying(30)", nullable: true),
                    DeletedById = table.Column<string>(type: "character varying(30)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Criteria_Account_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Criteria_Account_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_Criteria_Account_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(30)", nullable: false),
                    UpdateById = table.Column<string>(type: "character varying(30)", nullable: true),
                    DeletedById = table.Column<string>(type: "character varying(30)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Room_Account_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Room_Account_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_Room_Account_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                });

            migrationBuilder.CreateTable(
                name: "EntranceTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomName = table.Column<string>(type: "text", nullable: true),
                    RoomCapacity = table.Column<int>(type: "integer", nullable: true),
                    Shift = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAnnouncedTime = table.Column<bool>(type: "boolean", nullable: false),
                    AnnouncedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAnnouncedScore = table.Column<bool>(type: "boolean", nullable: false),
                    TeacherFirebaseId = table.Column<string>(type: "character varying(30)", nullable: true),
                    TeacherName = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(30)", nullable: false),
                    UpdateById = table.Column<string>(type: "character varying(30)", nullable: true),
                    DeletedById = table.Column<string>(type: "character varying(30)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntranceTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntranceTest_Account_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntranceTest_Account_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_EntranceTest_Account_TeacherFirebaseId",
                        column: x => x.TeacherFirebaseId,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntranceTest_Account_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_EntranceTest_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EntranceTestStudent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentFirebaseId = table.Column<string>(type: "character varying(30)", nullable: false),
                    EntranceTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    BandScore = table.Column<decimal>(type: "numeric", nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    IsScoreAnnounced = table.Column<bool>(type: "boolean", nullable: false),
                    InstructorComment = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(30)", nullable: false),
                    UpdateById = table.Column<string>(type: "character varying(30)", nullable: true),
                    DeletedById = table.Column<string>(type: "character varying(30)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntranceTestStudent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntranceTestStudent_Account_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntranceTestStudent_Account_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_EntranceTestStudent_Account_StudentFirebaseId",
                        column: x => x.StudentFirebaseId,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_EntranceTestStudent_Account_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_EntranceTestStudent_EntranceTest_EntranceTestId",
                        column: x => x.EntranceTestId,
                        principalTable: "EntranceTest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EntranceTestResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntranceTestStudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<decimal>(type: "numeric", nullable: true),
                    CriteriaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CriteriaName = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(30)", nullable: false),
                    UpdateById = table.Column<string>(type: "character varying(30)", nullable: true),
                    DeletedById = table.Column<string>(type: "character varying(30)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntranceTestResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntranceTestResult_Account_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntranceTestResult_Account_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_EntranceTestResult_Account_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_EntranceTestResult_Criteria_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "Criteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntranceTestResult_EntranceTestStudent_EntranceTestStudentId",
                        column: x => x.EntranceTestStudentId,
                        principalTable: "EntranceTestStudent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "AccountFirebaseId", "Address", "AvatarUrl", "BankAccount", "DateOfBirth", "Email", "Gender", "IsEmailVerified", "JoinedDate", "Level", "Phone", "RecordStatus", "RegistrationDate", "Role", "ShortDescription", "Status", "Username" },
                values: new object[,]
                {
                    { "admin001", "", "", "", null, "admin001@gmail.com", null, false, new DateTime(2025, 1, 9, 13, 46, 55, 498, DateTimeKind.Utc).AddTicks(200), 0, "", 1, null, 3, "", 0, null },
                    { "learner003", "", "", "", null, "learner003@gmail.com", null, false, new DateTime(2025, 1, 9, 13, 46, 55, 498, DateTimeKind.Utc).AddTicks(1397), 0, "", 1, null, 1, "", 0, null },
                    { "teacher002", "", "", "", null, "teacher002@gmail.com", null, false, new DateTime(2025, 1, 9, 13, 46, 55, 498, DateTimeKind.Utc).AddTicks(1387), 0, "", 1, null, 2, "", 0, null }
                });

            migrationBuilder.InsertData(
                table: "Criteria",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Description", "Name", "RecordStatus", "UpdateById", "UpdatedAt", "Weight" },
                values: new object[] { new Guid("f1009878-ac37-424b-a9cf-8ce235755cc5"), new DateTime(2025, 1, 9, 13, 46, 55, 500, DateTimeKind.Utc).AddTicks(385), "admin001", null, null, "criterialTest Description", "", 1, null, null, 0m });

            migrationBuilder.InsertData(
                table: "Room",
                columns: new[] { "Id", "Capacity", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Name", "RecordStatus", "Status", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("7bb2d750-e271-4bb3-bdc2-07e8fc98704c"), null, new DateTime(2025, 1, 9, 13, 46, 55, 500, DateTimeKind.Utc).AddTicks(2157), "admin001", null, null, "Room 2", 1, 0, null, null },
                    { new Guid("80100937-dca8-486d-bbde-5b24887dc82b"), null, new DateTime(2025, 1, 9, 13, 46, 55, 500, DateTimeKind.Utc).AddTicks(1588), "admin001", null, null, "Room 1", 1, 0, null, null },
                    { new Guid("a3980a5d-0446-4296-b483-52c0248a324b"), null, new DateTime(2025, 1, 9, 13, 46, 55, 500, DateTimeKind.Utc).AddTicks(2159), "admin001", null, null, "Room 3", 1, 0, null, null }
                });

            migrationBuilder.InsertData(
                table: "EntranceTest",
                columns: new[] { "Id", "AnnouncedTime", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "IsAnnouncedScore", "IsAnnouncedTime", "RecordStatus", "RoomCapacity", "RoomId", "RoomName", "Shift", "StartTime", "TeacherFirebaseId", "TeacherName", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("45316ded-caa7-4f95-aad1-3c7c25f5fb56"), new DateTime(2025, 1, 9, 7, 46, 55, 500, DateTimeKind.Utc).AddTicks(4021), new DateTime(2025, 1, 9, 13, 46, 55, 500, DateTimeKind.Utc).AddTicks(4019), "admin001", null, null, false, false, 1, null, new Guid("7bb2d750-e271-4bb3-bdc2-07e8fc98704c"), "Room 2", 2, new DateTime(2025, 1, 9, 10, 46, 55, 500, DateTimeKind.Utc).AddTicks(4022), null, null, null, null },
                    { new Guid("bb3cd729-8096-461e-a0bc-809495426a2a"), new DateTime(2025, 1, 9, 7, 46, 55, 500, DateTimeKind.Utc).AddTicks(3008), new DateTime(2025, 1, 9, 13, 46, 55, 500, DateTimeKind.Utc).AddTicks(2764), "admin001", null, null, false, false, 1, null, new Guid("80100937-dca8-486d-bbde-5b24887dc82b"), "Room 1", 0, new DateTime(2025, 1, 9, 10, 46, 55, 500, DateTimeKind.Utc).AddTicks(3422), "teacher002", null, null, null },
                    { new Guid("f724fb51-b58e-460c-b96e-6675650d3824"), new DateTime(2025, 1, 9, 7, 46, 55, 500, DateTimeKind.Utc).AddTicks(4024), new DateTime(2025, 1, 9, 13, 46, 55, 500, DateTimeKind.Utc).AddTicks(4023), "admin001", null, null, false, false, 1, null, new Guid("a3980a5d-0446-4296-b483-52c0248a324b"), null, 4, new DateTime(2025, 1, 9, 10, 46, 55, 500, DateTimeKind.Utc).AddTicks(4024), "teacher002", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "EntranceTestStudent",
                columns: new[] { "Id", "BandScore", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "EntranceTestId", "InstructorComment", "IsScoreAnnounced", "Rank", "RecordStatus", "StudentFirebaseId", "UpdateById", "UpdatedAt", "Year" },
                values: new object[,]
                {
                    { new Guid("02d9b1a6-bbe0-42b9-b49d-83ffe43b9312"), 8m, new DateTime(2025, 1, 9, 13, 46, 55, 500, DateTimeKind.Utc).AddTicks(6671), "admin001", null, null, new Guid("bb3cd729-8096-461e-a0bc-809495426a2a"), null, true, 2, 1, "learner003", null, null, 2024 },
                    { new Guid("7d34346a-f861-455a-8fc7-097806cab95c"), 8m, new DateTime(2025, 1, 9, 13, 46, 55, 500, DateTimeKind.Utc).AddTicks(6681), "admin001", null, null, new Guid("bb3cd729-8096-461e-a0bc-809495426a2a"), null, true, 3, 1, "learner003", null, null, 2024 },
                    { new Guid("c89b9cb0-7504-4489-ab66-a26268bcf291"), 9m, new DateTime(2025, 1, 9, 13, 46, 55, 500, DateTimeKind.Utc).AddTicks(4579), "admin001", null, null, new Guid("bb3cd729-8096-461e-a0bc-809495426a2a"), null, true, 1, 1, "learner003", null, null, 2024 }
                });

            migrationBuilder.InsertData(
                table: "EntranceTestResult",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "CriteriaId", "CriteriaName", "DeletedAt", "DeletedById", "EntranceTestStudentId", "RecordStatus", "Score", "UpdateById", "UpdatedAt" },
                values: new object[] { new Guid("334b3a34-fa74-4cfc-aac8-93f5facbba16"), new DateTime(2025, 1, 9, 13, 46, 55, 500, DateTimeKind.Utc).AddTicks(7091), "admin001", new Guid("f1009878-ac37-424b-a9cf-8ce235755cc5"), null, null, null, new Guid("c89b9cb0-7504-4489-ab66-a26268bcf291"), 1, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Criteria_CreatedById",
                table: "Criteria",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Criteria_DeletedById",
                table: "Criteria",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Criteria_Id",
                table: "Criteria",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Criteria_UpdateById",
                table: "Criteria",
                column: "UpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTest_CreatedById",
                table: "EntranceTest",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTest_DeletedById",
                table: "EntranceTest",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTest_RoomId",
                table: "EntranceTest",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTest_TeacherFirebaseId",
                table: "EntranceTest",
                column: "TeacherFirebaseId");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTest_UpdateById",
                table: "EntranceTest",
                column: "UpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTestResult_CreatedById",
                table: "EntranceTestResult",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTestResult_CriteriaId",
                table: "EntranceTestResult",
                column: "CriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTestResult_DeletedById",
                table: "EntranceTestResult",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTestResult_EntranceTestStudentId",
                table: "EntranceTestResult",
                column: "EntranceTestStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTestResult_UpdateById",
                table: "EntranceTestResult",
                column: "UpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTestStudent_CreatedById",
                table: "EntranceTestStudent",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTestStudent_DeletedById",
                table: "EntranceTestStudent",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTestStudent_EntranceTestId",
                table: "EntranceTestStudent",
                column: "EntranceTestId");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTestStudent_StudentFirebaseId",
                table: "EntranceTestStudent",
                column: "StudentFirebaseId");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTestStudent_UpdateById",
                table: "EntranceTestStudent",
                column: "UpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_Room_CreatedById",
                table: "Room",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Room_DeletedById",
                table: "Room",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Room_UpdateById",
                table: "Room",
                column: "UpdateById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntranceTestResult");

            migrationBuilder.DropTable(
                name: "Criteria");

            migrationBuilder.DropTable(
                name: "EntranceTestStudent");

            migrationBuilder.DropTable(
                name: "EntranceTest");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
