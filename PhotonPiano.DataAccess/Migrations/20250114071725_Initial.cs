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
                columns: new[] { "AccountFirebaseId", "Address", "AvatarUrl", "BankAccount", "DateOfBirth", "Email", "Gender", "IsEmailVerified", "JoinedDate", "Level", "Phone", "RecordStatus", "RegistrationDate", "Role", "ShortDescription", "Status", "StudentStatus", "Username" },
                values: new object[,]
                {
                    { "admin001", "", "", "", null, "admin001@gmail.com", null, false, new DateTime(2025, 1, 14, 14, 17, 22, 273, DateTimeKind.Utc).AddTicks(4756), 0, "", 1, null, 3, "", 0, null, null },
                    { "learner003", "", "", "", null, "learner003@gmail.com", null, false, new DateTime(2025, 1, 14, 14, 17, 22, 273, DateTimeKind.Utc).AddTicks(9676), 0, "", 1, null, 1, "", 0, null, null },
                    { "teacher002", "", "", "", null, "teacher002@gmail.com", null, false, new DateTime(2025, 1, 14, 14, 17, 22, 273, DateTimeKind.Utc).AddTicks(9623), 0, "", 1, null, 2, "", 0, null, null }
                });

            migrationBuilder.InsertData(
                table: "Criteria",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Description", "For", "Name", "RecordStatus", "UpdateById", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { new Guid("31c0d07b-8fec-4efa-81e3-482680d7d3ae"), new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(4029), "admin001", null, null, null, 1, "Kiểm tra nhỏ 2", 1, null, null, 5m },
                    { new Guid("37c7d950-525b-4141-a90d-2b7841cbcf38"), new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(4035), "admin001", null, null, null, 1, "Bài thi 1", 1, null, null, 10m },
                    { new Guid("5c4d2b35-107f-44bc-837b-18bfd71caf00"), new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(4083), "admin001", null, null, null, 1, "Thi cuối kỳ (Âm sắc)", 1, null, null, 15m },
                    { new Guid("6c9f2798-e2f4-4d7c-8b89-0de936f5fc09"), new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(4088), "admin001", null, null, null, 1, "Thi cuối kỳ (Phong thái)", 1, null, null, 15m },
                    { new Guid("75cf1df3-4245-4ea1-93ee-01f66610d1f6"), new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(4019), "admin001", null, null, null, 0, "Phong thái", 1, null, null, 10m },
                    { new Guid("78d60c14-bc73-4259-9233-0c9f825a6e39"), new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(4024), "admin001", null, null, null, 1, "Kiểm tra nhỏ 1", 1, null, null, 5m },
                    { new Guid("7b1d8d50-5fd7-4a42-a3b2-56d2f8a22471"), new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(3963), "admin001", null, null, null, 0, "Độ chính xác", 1, null, null, 10m },
                    { new Guid("880c74d2-3123-49c9-8849-77c3b6afa7ad"), new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(3977), "admin001", null, null, null, 0, "Âm Sắc", 1, null, null, 10m },
                    { new Guid("94b2ff90-98e5-4fff-85db-81ad3aea51c3"), new DateTime(2025, 1, 14, 14, 17, 22, 275, DateTimeKind.Utc).AddTicks(9653), "admin001", null, null, null, 0, "Nhịp điệu", 1, null, null, 10m },
                    { new Guid("a71538ad-0dc1-476f-a718-588298ec4dad"), new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(4046), "admin001", null, null, null, 1, "Điểm chuyên cần", 1, null, null, 5m },
                    { new Guid("e059f8b5-5ab2-4ef8-bad4-024faffec328"), new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(4072), "admin001", null, null, null, 1, "Thi cuối kỳ (Nhịp điệu)", 1, null, null, 15m },
                    { new Guid("e07419cf-3e26-4f57-a57b-f2ed1097a732"), new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(4077), "admin001", null, null, null, 1, "Thi cuối kỳ (Độ chính xác)", 1, null, null, 15m },
                    { new Guid("e21b8580-eb2a-4022-a86b-b4841ac32028"), new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(4040), "admin001", null, null, null, 1, "Bài thi 2", 1, null, null, 10m }
                });

            migrationBuilder.InsertData(
                table: "Room",
                columns: new[] { "Id", "Capacity", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Name", "RecordStatus", "Status", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("6d71c5ac-34e4-47ca-a40e-1b531ccd447a"), null, new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(6979), "admin001", null, null, "Room 1", 1, 0, null, null },
                    { new Guid("9b49a0f8-f4a2-442b-b92c-e1a7933989b5"), null, new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(8277), "admin001", null, null, "Room 3", 1, 0, null, null },
                    { new Guid("fc5bdcec-ecea-4a45-9bca-8d9303511078"), null, new DateTime(2025, 1, 14, 14, 17, 22, 276, DateTimeKind.Utc).AddTicks(8271), "admin001", null, null, "Room 2", 1, 0, null, null }
                });

            migrationBuilder.InsertData(
                table: "EntranceTest",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "Date", "DeletedAt", "DeletedById", "InstructorId", "InstructorName", "IsAnnouncedScore", "IsOpen", "RecordStatus", "RoomCapacity", "RoomId", "RoomName", "Shift", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1a7abe08-758a-431d-a088-119e27cf0c1d"), new DateTime(2025, 1, 14, 14, 17, 22, 277, DateTimeKind.Utc).AddTicks(5022), "admin001", new DateOnly(2025, 1, 14), null, null, null, null, false, true, 1, null, new Guid("fc5bdcec-ecea-4a45-9bca-8d9303511078"), "Room 2", 2, null, null },
                    { new Guid("2d0bf0b6-2e3c-454a-bb8d-d1370a58a817"), new DateTime(2025, 1, 14, 14, 17, 22, 277, DateTimeKind.Utc).AddTicks(5055), "admin001", new DateOnly(2025, 1, 14), null, null, "teacher002", null, false, true, 1, null, new Guid("9b49a0f8-f4a2-442b-b92c-e1a7933989b5"), null, 4, null, null },
                    { new Guid("72c2905c-437f-43c9-b812-c059824141f7"), new DateTime(2025, 1, 14, 14, 17, 22, 277, DateTimeKind.Utc).AddTicks(599), "admin001", new DateOnly(2025, 1, 14), null, null, "teacher002", null, false, true, 1, null, new Guid("6d71c5ac-34e4-47ca-a40e-1b531ccd447a"), "Room 1", 0, null, null }
                });

            migrationBuilder.InsertData(
                table: "EntranceTestStudent",
                columns: new[] { "Id", "BandScore", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "EntranceTestId", "InstructorComment", "IsScoreAnnounced", "Rank", "RecordStatus", "StudentFirebaseId", "UpdateById", "UpdatedAt", "Year" },
                values: new object[,]
                {
                    { new Guid("4737f7b3-2f18-4883-a477-e73b4d4b2d84"), 8m, new DateTime(2025, 1, 14, 14, 17, 22, 278, DateTimeKind.Utc).AddTicks(5476), "admin001", null, null, new Guid("72c2905c-437f-43c9-b812-c059824141f7"), null, true, 2, 1, "learner003", null, null, 2024 },
                    { new Guid("9096a950-16df-4a97-a472-fedc87a92308"), 3m, new DateTime(2025, 1, 14, 14, 17, 22, 278, DateTimeKind.Utc).AddTicks(5691), "admin001", null, null, new Guid("72c2905c-437f-43c9-b812-c059824141f7"), null, true, 3, 1, "learner003", null, null, 2024 },
                    { new Guid("c1ba379f-aa95-4d8b-80e2-10de374668f8"), 3m, new DateTime(2025, 1, 14, 14, 17, 22, 277, DateTimeKind.Utc).AddTicks(8513), "admin001", null, null, new Guid("72c2905c-437f-43c9-b812-c059824141f7"), null, true, 1, 1, "learner003", null, null, 2024 }
                });

            migrationBuilder.InsertData(
                table: "EntranceTestResult",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "CriteriaId", "CriteriaName", "DeletedAt", "DeletedById", "EntranceTestStudentId", "RecordStatus", "Score", "UpdateById", "UpdatedAt" },
                values: new object[] { new Guid("5b208b8a-cf91-429d-b6b9-5aaa9614006f"), new DateTime(2025, 1, 14, 14, 17, 22, 278, DateTimeKind.Utc).AddTicks(7242), "admin001", new Guid("7b1d8d50-5fd7-4a42-a3b2-56d2f8a22471"), null, null, null, new Guid("c1ba379f-aa95-4d8b-80e2-10de374668f8"), 1, null, null, null });

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
