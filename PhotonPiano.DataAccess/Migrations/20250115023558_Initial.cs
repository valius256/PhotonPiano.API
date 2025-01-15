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
                    UserName = table.Column<string>(type: "text", nullable: true),
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
                    RecordStatus = table.Column<int>(type: "integer", nullable: false),
                    StudentStatus = table.Column<int>(type: "integer", nullable: true)
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
                    For = table.Column<int>(type: "integer", nullable: false),
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
                    Name = table.Column<string>(type: "text", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomName = table.Column<string>(type: "text", nullable: true),
                    RoomCapacity = table.Column<int>(type: "integer", nullable: true),
                    Shift = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsAnnouncedScore = table.Column<bool>(type: "boolean", nullable: false),
                    InstructorId = table.Column<string>(type: "character varying(30)", nullable: true),
                    InstructorName = table.Column<string>(type: "text", nullable: true),
                    IsOpen = table.Column<bool>(type: "boolean", nullable: false),
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
                        name: "FK_EntranceTest_Account_InstructorId",
                        column: x => x.InstructorId,
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
                columns: new[] { "AccountFirebaseId", "Address", "AvatarUrl", "BankAccount", "DateOfBirth", "Email", "Gender", "IsEmailVerified", "JoinedDate", "Level", "Phone", "RecordStatus", "RegistrationDate", "Role", "ShortDescription", "Status", "StudentStatus", "UserName" },
                values: new object[,]
                {
                    { "admin001", "", "", "", null, "admin001@gmail.com", null, false, new DateTime(2025, 1, 15, 9, 35, 57, 579, DateTimeKind.Utc).AddTicks(6411), 0, "", 1, null, 3, "", 0, null, "admin001" },
                    { "learner003", "", "", "", null, "learner003@gmail.com", null, false, new DateTime(2025, 1, 15, 9, 35, 57, 579, DateTimeKind.Utc).AddTicks(7887), 0, "", 1, null, 1, "", 0, null, "learner003" },
                    { "teacher002", "", "", "", null, "teacher002@gmail.com", null, false, new DateTime(2025, 1, 15, 9, 35, 57, 579, DateTimeKind.Utc).AddTicks(7877), 0, "", 1, null, 2, "", 0, null, "teacher002" }
                });

            migrationBuilder.InsertData(
                table: "Criteria",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Description", "For", "Name", "RecordStatus", "UpdateById", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { new Guid("0d4c9d85-411b-4ce8-af36-f2a7ad45796f"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(6331), "admin001", null, null, null, 1, "Bài thi 2", 1, null, null, 10m },
                    { new Guid("33a95c01-b2b3-4011-ac36-bce8d6def12a"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(6344), "admin001", null, null, null, 1, "Điểm chuyên cần", 1, null, null, 5m },
                    { new Guid("44c43e52-2d45-4141-b3c8-69d5fc5f385c"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(6353), "admin001", null, null, null, 1, "Thi cuối kỳ (Phong thái)", 1, null, null, 15m },
                    { new Guid("59576612-6ef4-4f92-9861-0a00e954de2e"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(6346), "admin001", null, null, null, 1, "Thi cuối kỳ (Nhịp điệu)", 1, null, null, 15m },
                    { new Guid("92f79298-3332-4067-8078-36ed895042a0"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(6321), "admin001", null, null, null, 0, "Phong thái", 1, null, null, 10m },
                    { new Guid("9ee3f7cb-1ae0-4413-9c32-509fc0b0447c"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(6316), "admin001", null, null, null, 0, "Âm Sắc", 1, null, null, 10m },
                    { new Guid("afd8c65d-3709-4b4e-bcbb-6a6cfbca32ce"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(6349), "admin001", null, null, null, 1, "Thi cuối kỳ (Độ chính xác)", 1, null, null, 15m },
                    { new Guid("c133b6a0-1494-42d6-bb72-0b7dabf085bd"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(5373), "admin001", null, null, null, 0, "Nhịp điệu", 1, null, null, 10m },
                    { new Guid("d4ccc7e4-c000-42bd-adfa-4d0d713d004f"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(6351), "admin001", null, null, null, 1, "Thi cuối kỳ (Âm sắc)", 1, null, null, 15m },
                    { new Guid("d7d8f75d-8ba3-4e5e-ba75-dc8aa96f475e"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(6323), "admin001", null, null, null, 1, "Kiểm tra nhỏ 1", 1, null, null, 5m },
                    { new Guid("db785b8a-3700-47a0-9a43-a5e5879e8edb"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(6313), "admin001", null, null, null, 0, "Độ chính xác", 1, null, null, 10m },
                    { new Guid("df4f95ce-479b-405d-933d-4417cf52b66a"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(6326), "admin001", null, null, null, 1, "Kiểm tra nhỏ 2", 1, null, null, 5m },
                    { new Guid("f3bfe508-ecc1-4f70-aeb0-05703e0659c4"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(6328), "admin001", null, null, null, 1, "Bài thi 1", 1, null, null, 10m }
                });

            migrationBuilder.InsertData(
                table: "Room",
                columns: new[] { "Id", "Capacity", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Name", "RecordStatus", "Status", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("14c8bc81-36dd-453f-965e-35689a14440d"), null, new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(7505), "admin001", null, null, "Room 2", 1, 0, null, null },
                    { new Guid("76b82166-934c-48b8-adec-b3086ce2b07f"), null, new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(7508), "admin001", null, null, "Room 3", 1, 0, null, null },
                    { new Guid("ca296036-eef2-4432-a08e-b83bdf1996ef"), null, new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(7055), "admin001", null, null, "Room 1", 1, 0, null, null }
                });

            migrationBuilder.InsertData(
                table: "EntranceTest",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "Date", "DeletedAt", "DeletedById", "InstructorId", "InstructorName", "IsAnnouncedScore", "IsOpen", "Name", "RecordStatus", "RoomCapacity", "RoomId", "RoomName", "Shift", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1862182d-aa57-4460-8d27-616daf3f5f51"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(9622), "admin001", new DateOnly(2025, 1, 15), null, null, "teacher002", null, false, true, "EntranceTest 3", 1, null, new Guid("76b82166-934c-48b8-adec-b3086ce2b07f"), null, 4, null, null },
                    { new Guid("689b927a-1cb4-4e0a-9c27-2361994e8e7a"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(9614), "admin001", new DateOnly(2025, 1, 15), null, null, null, null, false, true, "EntranceTest 2", 1, null, new Guid("14c8bc81-36dd-453f-965e-35689a14440d"), "Room 2", 2, null, null },
                    { new Guid("6cde3dd8-3dce-4616-9a76-a4d4885568fb"), new DateTime(2025, 1, 15, 9, 35, 57, 580, DateTimeKind.Utc).AddTicks(8089), "admin001", new DateOnly(2025, 1, 15), null, null, "teacher002", null, false, true, "EntranceTest 1", 1, null, new Guid("ca296036-eef2-4432-a08e-b83bdf1996ef"), "Room 1", 0, null, null }
                });

            migrationBuilder.InsertData(
                table: "EntranceTestStudent",
                columns: new[] { "Id", "BandScore", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "EntranceTestId", "InstructorComment", "IsScoreAnnounced", "Rank", "RecordStatus", "StudentFirebaseId", "UpdateById", "UpdatedAt", "Year" },
                values: new object[,]
                {
                    { new Guid("23fed99c-f516-490e-84b2-84d4b5b0c121"), 7m, new DateTime(2025, 1, 15, 9, 35, 57, 581, DateTimeKind.Utc).AddTicks(2647), "admin001", null, null, new Guid("6cde3dd8-3dce-4616-9a76-a4d4885568fb"), null, true, 3, 1, "learner003", null, null, 2024 },
                    { new Guid("61668dca-aa88-4bca-be91-8455d940d96e"), 7m, new DateTime(2025, 1, 15, 9, 35, 57, 581, DateTimeKind.Utc).AddTicks(2634), "admin001", null, null, new Guid("6cde3dd8-3dce-4616-9a76-a4d4885568fb"), null, true, 2, 1, "learner003", null, null, 2024 },
                    { new Guid("d1b32be0-3eb9-449d-bb8e-308dd338a6b0"), 8m, new DateTime(2025, 1, 15, 9, 35, 57, 581, DateTimeKind.Utc).AddTicks(317), "admin001", null, null, new Guid("6cde3dd8-3dce-4616-9a76-a4d4885568fb"), null, true, 1, 1, "learner003", null, null, 2024 }
                });

            migrationBuilder.InsertData(
                table: "EntranceTestResult",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "CriteriaId", "CriteriaName", "DeletedAt", "DeletedById", "EntranceTestStudentId", "RecordStatus", "Score", "UpdateById", "UpdatedAt" },
                values: new object[] { new Guid("441198e2-9baf-46bc-8742-0a34cb126703"), new DateTime(2025, 1, 15, 9, 35, 57, 581, DateTimeKind.Utc).AddTicks(3184), "admin001", new Guid("db785b8a-3700-47a0-9a43-a5e5879e8edb"), null, null, null, new Guid("d1b32be0-3eb9-449d-bb8e-308dd338a6b0"), 1, null, null, null });

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
                name: "IX_EntranceTest_InstructorId",
                table: "EntranceTest",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceTest_RoomId",
                table: "EntranceTest",
                column: "RoomId");

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
