using System;
using System.Collections.Generic;
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
                    Level = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StudentStatus = table.Column<int>(type: "integer", nullable: true),
                    DesiredLevel = table.Column<int>(type: "integer", nullable: true),
                    DesiredTargets = table.Column<List<string>>(type: "text[]", nullable: false),
                    FavoriteMusicGenres = table.Column<List<string>>(type: "text[]", nullable: false),
                    PreferredLearningMethods = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountFirebaseId);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigName = table.Column<string>(type: "text", nullable: false),
                    ConfigValue = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Class",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorId = table.Column<string>(type: "character varying(30)", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(30)", nullable: false),
                    IsScorePublished = table.Column<bool>(type: "boolean", nullable: false),
                    UpdateById = table.Column<string>(type: "character varying(30)", nullable: true),
                    DeletedById = table.Column<string>(type: "character varying(30)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Class", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Class_Account_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Class_Account_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_Class_Account_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_Class_Account_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId",
                        onDelete: ReferentialAction.Cascade);
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
                name: "DayOff",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_DayOff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DayOff_Account_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_DayOff_Account_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_DayOff_Account_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                });

            migrationBuilder.CreateTable(
                name: "New",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Thumbnail = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_New", x => x.Id);
                    table.ForeignKey(
                        name: "FK_New_Account_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_New_Account_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_New_Account_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Thumbnail = table.Column<string>(type: "text", nullable: false),
                    AccountFirebaseId = table.Column<string>(type: "character varying(30)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_Account_AccountFirebaseId",
                        column: x => x.AccountFirebaseId,
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
                name: "StudentClass",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentFirebaseId = table.Column<string>(type: "character varying(30)", nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(30)", nullable: false),
                    UpdateById = table.Column<string>(type: "character varying(30)", nullable: true),
                    DeletedById = table.Column<string>(type: "character varying(30)", nullable: true),
                    CertificateUrl = table.Column<string>(type: "text", nullable: true),
                    IsPassed = table.Column<bool>(type: "boolean", nullable: false),
                    GPA = table.Column<decimal>(type: "numeric", nullable: true),
                    InstructorComment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentClass_Account_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentClass_Account_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_StudentClass_Account_StudentFirebaseId",
                        column: x => x.StudentFirebaseId,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_StudentClass_Account_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentClass_Class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Class",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccountNotification",
                columns: table => new
                {
                    AccountFirebaseId = table.Column<string>(type: "character varying(30)", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsViewed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountNotification", x => new { x.AccountFirebaseId, x.NotificationId });
                    table.ForeignKey(
                        name: "FK_AccountNotification_Account_AccountFirebaseId",
                        column: x => x.AccountFirebaseId,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_AccountNotification_Notification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notification",
                        principalColumn: "Id");
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
                    CreatedById = table.Column<string>(type: "character varying(30)", nullable: false),
                    UpdateById = table.Column<string>(type: "character varying(30)", nullable: true),
                    DeletedById = table.Column<string>(type: "character varying(30)", nullable: true),
                    Fee = table.Column<decimal>(type: "numeric", nullable: false),
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
                name: "Slot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: true),
                    Shift = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Slot_Class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Class",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Slot_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentClassScore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<decimal>(type: "numeric", nullable: true),
                    CriteriaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentClassScore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentClassScore_Criteria_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "Criteria",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentClassScore_StudentClass_StudentClassId",
                        column: x => x.StudentClassId,
                        principalTable: "StudentClass",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tution",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tution_StudentClass_StudentClassId",
                        column: x => x.StudentClassId,
                        principalTable: "StudentClass",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EntranceTestStudent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentFirebaseId = table.Column<string>(type: "character varying(30)", nullable: false),
                    EntranceTestId = table.Column<Guid>(type: "uuid", nullable: true),
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
                name: "SlotStudent",
                columns: table => new
                {
                    SlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentFirebaseId = table.Column<string>(type: "character varying(30)", nullable: false),
                    AttendanceStatus = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_SlotStudent", x => new { x.SlotId, x.StudentFirebaseId });
                    table.ForeignKey(
                        name: "FK_SlotStudent_Account_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_SlotStudent_Account_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_SlotStudent_Account_StudentFirebaseId",
                        column: x => x.StudentFirebaseId,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_SlotStudent_Account_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_SlotStudent_Slot_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slot",
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

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    TransactionCode = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(30)", nullable: false),
                    CreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    TutionId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntranceTestStudentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transaction_Account_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Account",
                        principalColumn: "AccountFirebaseId");
                    table.ForeignKey(
                        name: "FK_Transaction_EntranceTestStudent_EntranceTestStudentId",
                        column: x => x.EntranceTestStudentId,
                        principalTable: "EntranceTestStudent",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transaction_Tution_TutionId",
                        column: x => x.TutionId,
                        principalTable: "Tution",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "AccountFirebaseId", "Address", "AvatarUrl", "BankAccount", "CreatedAt", "DateOfBirth", "DeletedAt", "DesiredLevel", "DesiredTargets", "Email", "FavoriteMusicGenres", "Gender", "IsEmailVerified", "JoinedDate", "Level", "Phone", "PreferredLearningMethods", "RecordStatus", "Role", "ShortDescription", "Status", "StudentStatus", "UpdatedAt", "UserName" },
                values: new object[,]
                {
                    { "admin001", "", "", "", new DateTime(2025, 1, 30, 12, 59, 10, 95, DateTimeKind.Utc).AddTicks(9530), null, null, null, new List<string>(), "admin001@gmail.com", new List<string>(), null, false, new DateTime(2025, 1, 30, 12, 59, 10, 95, DateTimeKind.Utc).AddTicks(7556), null, "", new List<string>(), 1, 3, "", 0, null, null, "admin001" },
                    { "learner003", "", "", "", new DateTime(2025, 1, 30, 12, 59, 10, 96, DateTimeKind.Utc).AddTicks(797), null, null, null, new List<string>(), "learner003@gmail.com", new List<string>(), null, false, new DateTime(2025, 1, 30, 12, 59, 10, 96, DateTimeKind.Utc).AddTicks(790), null, "", new List<string>(), 1, 1, "", 0, null, null, "learner003" },
                    { "teacher002", "", "", "", new DateTime(2025, 1, 30, 12, 59, 10, 96, DateTimeKind.Utc).AddTicks(786), null, null, null, new List<string>(), "teacher002@gmail.com", new List<string>(), null, false, new DateTime(2025, 1, 30, 12, 59, 10, 96, DateTimeKind.Utc).AddTicks(759), null, "", new List<string>(), 1, 2, "", 0, null, null, "teacher002" }
                });

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "Id", "ConfigName", "ConfigValue", "CreatedAt", "DeletedAt", "RecordStatus", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0303605d-8a35-488f-8b9d-d234820f6e0d"), "Mức phí theo buổi LEVEL 5", "400000", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2342), null, 1, 3, null },
                    { new Guid("155910d3-5cab-4834-815f-34c8d74b8685"), "Tổng số buổi học LEVEL 2", "30", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2367), null, 1, 3, null },
                    { new Guid("23d55201-72fa-432b-90bd-6e0ab46a434d"), "Mức phí theo buổi LEVEL 2", "250000", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2335), null, 1, 3, null },
                    { new Guid("34439bc9-1a31-4ac1-9488-80cdbca4c648"), "Mức phí theo buổi LEVEL 4", "350000", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2340), null, 1, 3, null },
                    { new Guid("401055d2-982b-49b1-9461-9f61e46344ad"), "Mức phí theo buổi LEVEL 3", "300000", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2338), null, 1, 3, null },
                    { new Guid("588a9356-a5c7-4abd-baa3-d0217e65aa34"), "Sĩ số lớp tối đa", "12", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2328), null, 1, 3, null },
                    { new Guid("759ac9be-29a6-483b-82ae-43dc76b62aee"), "Số buổi học 1 tuần LEVEL 5", "2", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2362), null, 1, 3, null },
                    { new Guid("7bf46634-8ada-4964-b349-243bc5d306f5"), "Số buổi học 1 tuần LEVEL 3", "2", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2357), null, 1, 3, null },
                    { new Guid("911bd95d-5bab-4755-9f96-a9f952017d3c"), "Số buổi học 1 tuần LEVEL 2", "2", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2355), null, 1, 3, null },
                    { new Guid("91e8bce6-d9fa-41ea-8e5a-89843d32eaa9"), "Sĩ số lớp tối thiểu", "8", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(1656), null, 1, 3, null },
                    { new Guid("9b92d0dc-f918-4bc6-99d0-f512428784c7"), "Tổng số buổi học LEVEL 1", "30", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2365), null, 1, 3, null },
                    { new Guid("ab8b8206-7dcf-4274-935a-8ab26d70b4b5"), "Mức phí theo buổi LEVEL 1", "200000", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2333), null, 1, 3, null },
                    { new Guid("b05cdfa9-de7e-48d1-978b-3de7a957f6ea"), "Tổng số buổi học LEVEL 4", "40", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2376), null, 1, 3, null },
                    { new Guid("d933049c-1c30-4485-bda0-498e6fb75f3b"), "Số buổi học 1 tuần LEVEL 1", "2", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2352), null, 1, 3, null },
                    { new Guid("f2aeb834-8cb0-44dc-aa21-aacbe9826b78"), "Số buổi học 1 tuần LEVEL 4", "2", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2360), null, 1, 3, null },
                    { new Guid("f378fab9-9048-47a9-9920-d12ebe2eede5"), "Tổng số buổi học LEVEL 5", "50", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2378), null, 1, 3, null },
                    { new Guid("f81776c4-191b-40bf-a047-ff11e6e7c341"), "Tổng số buổi học LEVEL 3", "30", new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(2370), null, 1, 3, null }
                });

            migrationBuilder.InsertData(
                table: "Class",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "InstructorId", "IsPublic", "IsScorePublished", "Level", "Name", "RecordStatus", "Status", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("44252e22-d30c-49e0-bab3-d71da7888e42"), new DateTime(2025, 1, 30, 12, 59, 10, 98, DateTimeKind.Utc).AddTicks(7108), "admin001", null, null, "teacher002", true, false, 2, "Class 1", 1, 1, null, null },
                    { new Guid("c2f05422-4011-4552-866a-4c57c6bc5e6b"), new DateTime(2025, 1, 30, 12, 59, 10, 98, DateTimeKind.Utc).AddTicks(8709), "admin001", null, null, "teacher002", true, false, 3, "Class 2", 1, 0, null, null }
                });

            migrationBuilder.InsertData(
                table: "Criteria",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Description", "For", "Name", "RecordStatus", "UpdateById", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { new Guid("0099e1c3-cdd3-4512-914d-d4d92b586282"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(3201), "admin001", null, null, null, 1, "Bài thi 1", 1, null, null, 10m },
                    { new Guid("182b7158-82e9-41a0-8f68-03443c87442f"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(3223), "admin001", null, null, null, 1, "Thi cuối kỳ (Phong thái)", 1, null, null, 15m },
                    { new Guid("2e209368-a835-443d-9ae4-be6ada302869"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(3209), "admin001", null, null, null, 1, "Điểm chuyên cần", 1, null, null, 5m },
                    { new Guid("30386aa3-a684-4861-b790-92829a861858"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(3161), "admin001", null, null, null, 0, "Âm Sắc", 1, null, null, 10m },
                    { new Guid("529d043d-f2b6-4943-8caf-472a6593babc"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(3220), "admin001", null, null, null, 1, "Thi cuối kỳ (Âm sắc)", 1, null, null, 15m },
                    { new Guid("5b43f584-6ce3-4151-b7c3-b764e17b7311"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(3176), "admin001", null, null, null, 1, "Kiểm tra nhỏ 2", 1, null, null, 5m },
                    { new Guid("715a6221-1406-4365-8572-5d33d9a749c4"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(3205), "admin001", null, null, null, 1, "Bài thi 2", 1, null, null, 10m },
                    { new Guid("95b3bb4b-5085-4b0e-8a56-bbe5368ef341"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(3172), "admin001", null, null, null, 1, "Kiểm tra nhỏ 1", 1, null, null, 5m },
                    { new Guid("a24e3d85-55d8-423e-b7d1-263c09733d44"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(3152), "admin001", null, null, null, 0, "Độ chính xác", 1, null, null, 10m },
                    { new Guid("ae185c70-de84-4a93-b516-14d4a793caf1"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(3214), "admin001", null, null, null, 1, "Thi cuối kỳ (Nhịp điệu)", 1, null, null, 15m },
                    { new Guid("b170b08c-cb80-4a94-80d3-2cd93948955f"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(3169), "admin001", null, null, null, 0, "Phong thái", 1, null, null, 10m },
                    { new Guid("def5cdb7-6019-42dd-9b5e-3f432fc14572"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(1765), "admin001", null, null, null, 0, "Nhịp điệu", 1, null, null, 10m },
                    { new Guid("ead79e25-7c2f-47a4-ad30-bc7ec9d21d12"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(3217), "admin001", null, null, null, 1, "Thi cuối kỳ (Độ chính xác)", 1, null, null, 15m }
                });

            migrationBuilder.InsertData(
                table: "Room",
                columns: new[] { "Id", "Capacity", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Name", "RecordStatus", "Status", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("30bda2b2-0663-4c26-bb0c-ef6f46c9f196"), null, new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(6028), "admin001", null, null, "Room 2", 1, 0, null, null },
                    { new Guid("a07832ea-c6b6-4f4e-814f-33ab1ccbaa0f"), null, new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(5485), "admin001", null, null, "Room 1", 1, 0, null, null },
                    { new Guid("ef8bc71f-aa3b-40a8-a060-41ffc8310d95"), null, new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(6054), "admin001", null, null, "Room 3", 1, 0, null, null }
                });

            migrationBuilder.InsertData(
                table: "EntranceTest",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "Date", "DeletedAt", "DeletedById", "Fee", "InstructorId", "InstructorName", "IsAnnouncedScore", "Name", "RecordStatus", "RoomCapacity", "RoomId", "RoomName", "Shift", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0a680cc3-6a0a-469f-98a3-d81b6104ba55"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(9150), "admin001", new DateOnly(2025, 1, 30), null, null, 0m, "teacher002", null, false, "EntranceTest 3", 1, null, new Guid("ef8bc71f-aa3b-40a8-a060-41ffc8310d95"), null, 4, null, null },
                    { new Guid("466ea66b-f04b-4d40-acb5-c8123c73f4c5"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(7140), "admin001", new DateOnly(2025, 1, 30), null, null, 0m, "teacher002", null, false, "EntranceTest 1", 1, null, new Guid("a07832ea-c6b6-4f4e-814f-33ab1ccbaa0f"), "Room 1", 0, null, null },
                    { new Guid("790defe7-8df2-42fe-aca1-b41b78a2ccb8"), new DateTime(2025, 1, 30, 12, 59, 10, 97, DateTimeKind.Utc).AddTicks(9136), "admin001", new DateOnly(2025, 1, 30), null, null, 0m, null, null, false, "EntranceTest 2", 1, null, new Guid("30bda2b2-0663-4c26-bb0c-ef6f46c9f196"), "Room 2", 2, null, null }
                });

            migrationBuilder.InsertData(
                table: "Slot",
                columns: new[] { "Id", "ClassId", "CreatedAt", "Date", "DeletedAt", "RecordStatus", "RoomId", "Shift", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0c54aca9-2c9c-4e95-9fa9-07aa804c785e"), new Guid("44252e22-d30c-49e0-bab3-d71da7888e42"), new DateTime(2025, 1, 30, 12, 59, 10, 98, DateTimeKind.Utc).AddTicks(9848), new DateOnly(2025, 1, 30), null, 1, new Guid("a07832ea-c6b6-4f4e-814f-33ab1ccbaa0f"), 0, 1, null },
                    { new Guid("463ebc9c-be4a-4725-9e85-164e06be03b2"), new Guid("44252e22-d30c-49e0-bab3-d71da7888e42"), new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(1031), new DateOnly(2025, 2, 1), null, 1, new Guid("a07832ea-c6b6-4f4e-814f-33ab1ccbaa0f"), 0, 1, null },
                    { new Guid("88ec7b30-0459-4b58-931c-3952406d8cff"), new Guid("44252e22-d30c-49e0-bab3-d71da7888e42"), new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(1063), new DateOnly(2025, 2, 3), null, 1, new Guid("a07832ea-c6b6-4f4e-814f-33ab1ccbaa0f"), 0, 1, null },
                    { new Guid("a3a26349-221a-4948-a4cf-254c262b9d85"), new Guid("44252e22-d30c-49e0-bab3-d71da7888e42"), new DateTime(2025, 1, 30, 12, 59, 10, 99, DateTimeKind.Utc).AddTicks(1067), new DateOnly(2025, 2, 5), null, 1, new Guid("a07832ea-c6b6-4f4e-814f-33ab1ccbaa0f"), 0, 1, null }
                });

            migrationBuilder.InsertData(
                table: "EntranceTestStudent",
                columns: new[] { "Id", "BandScore", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "EntranceTestId", "InstructorComment", "IsScoreAnnounced", "Rank", "RecordStatus", "StudentFirebaseId", "UpdateById", "UpdatedAt", "Year" },
                values: new object[,]
                {
                    { new Guid("00c77746-4bbe-4fb1-886b-ea457c20eb56"), 8m, new DateTime(2025, 1, 30, 12, 59, 10, 98, DateTimeKind.Utc).AddTicks(393), "admin001", null, null, new Guid("466ea66b-f04b-4d40-acb5-c8123c73f4c5"), null, true, 1, 1, "learner003", null, null, 2024 },
                    { new Guid("2ff97b07-f959-4f57-821c-970c16fd49c1"), 9m, new DateTime(2025, 1, 30, 12, 59, 10, 98, DateTimeKind.Utc).AddTicks(4171), "admin001", null, null, new Guid("466ea66b-f04b-4d40-acb5-c8123c73f4c5"), null, true, 3, 1, "learner003", null, null, 2024 },
                    { new Guid("38265f58-6aa7-469d-98c0-08278d5c2cb8"), 4m, new DateTime(2025, 1, 30, 12, 59, 10, 98, DateTimeKind.Utc).AddTicks(4135), "admin001", null, null, new Guid("466ea66b-f04b-4d40-acb5-c8123c73f4c5"), null, true, 2, 1, "learner003", null, null, 2024 }
                });

            migrationBuilder.InsertData(
                table: "EntranceTestResult",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "CriteriaId", "CriteriaName", "DeletedAt", "DeletedById", "EntranceTestStudentId", "RecordStatus", "Score", "UpdateById", "UpdatedAt" },
                values: new object[] { new Guid("9c5a3274-af29-4d8c-8d23-3572c5c0e33b"), new DateTime(2025, 1, 30, 12, 59, 10, 98, DateTimeKind.Utc).AddTicks(5139), "admin001", new Guid("a24e3d85-55d8-423e-b7d1-263c09733d44"), null, null, null, new Guid("00c77746-4bbe-4fb1-886b-ea457c20eb56"), 1, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_AccountNotification_NotificationId",
                table: "AccountNotification",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Class_CreatedById",
                table: "Class",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Class_DeletedById",
                table: "Class",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Class_Id",
                table: "Class",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Class_InstructorId",
                table: "Class",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Class_UpdateById",
                table: "Class",
                column: "UpdateById");

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
                name: "IX_DayOff_CreatedById",
                table: "DayOff",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DayOff_DeletedById",
                table: "DayOff",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_DayOff_UpdateById",
                table: "DayOff",
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
                name: "IX_New_CreatedById",
                table: "New",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_New_DeletedById",
                table: "New",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_New_Id",
                table: "New",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_New_UpdateById",
                table: "New",
                column: "UpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AccountFirebaseId",
                table: "Notification",
                column: "AccountFirebaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Id",
                table: "Notification",
                column: "Id");

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

            migrationBuilder.CreateIndex(
                name: "IX_Slot_ClassId",
                table: "Slot",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Slot_RoomId",
                table: "Slot",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotStudent_CreatedById",
                table: "SlotStudent",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SlotStudent_DeletedById",
                table: "SlotStudent",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SlotStudent_StudentFirebaseId",
                table: "SlotStudent",
                column: "StudentFirebaseId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotStudent_UpdateById",
                table: "SlotStudent",
                column: "UpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClass_ClassId",
                table: "StudentClass",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClass_CreatedById",
                table: "StudentClass",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClass_DeletedById",
                table: "StudentClass",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClass_StudentFirebaseId",
                table: "StudentClass",
                column: "StudentFirebaseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClass_UpdateById",
                table: "StudentClass",
                column: "UpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassScore_CriteriaId",
                table: "StudentClassScore",
                column: "CriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassScore_StudentClassId",
                table: "StudentClassScore",
                column: "StudentClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CreatedById",
                table: "Transaction",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_EntranceTestStudentId",
                table: "Transaction",
                column: "EntranceTestStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_Id",
                table: "Transaction",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TutionId",
                table: "Transaction",
                column: "TutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Tution_StudentClassId",
                table: "Tution",
                column: "StudentClassId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountNotification");

            migrationBuilder.DropTable(
                name: "DayOff");

            migrationBuilder.DropTable(
                name: "EntranceTestResult");

            migrationBuilder.DropTable(
                name: "New");

            migrationBuilder.DropTable(
                name: "SlotStudent");

            migrationBuilder.DropTable(
                name: "StudentClassScore");

            migrationBuilder.DropTable(
                name: "SystemConfig");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Slot");

            migrationBuilder.DropTable(
                name: "Criteria");

            migrationBuilder.DropTable(
                name: "EntranceTestStudent");

            migrationBuilder.DropTable(
                name: "Tution");

            migrationBuilder.DropTable(
                name: "EntranceTest");

            migrationBuilder.DropTable(
                name: "StudentClass");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Class");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
