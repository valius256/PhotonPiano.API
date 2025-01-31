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
                    { "admin001", "", "", "", new DateTime(2025, 1, 31, 9, 7, 57, 871, DateTimeKind.Utc).AddTicks(8453), null, null, null, new List<string>(), "admin001@gmail.com", new List<string>(), null, false, new DateTime(2025, 1, 31, 9, 7, 57, 871, DateTimeKind.Utc).AddTicks(7583), null, "", new List<string>(), 1, 3, "", 0, null, null, "admin001" },
                    { "gnRssA2sZHWnXB23oUuUxwz95Ln1", "", "", "", new DateTime(2025, 1, 31, 9, 7, 57, 871, DateTimeKind.Utc).AddTicks(9194), null, null, null, new List<string>(), "staff123@gmail.com", new List<string>(), null, false, new DateTime(2025, 1, 31, 9, 7, 57, 871, DateTimeKind.Utc).AddTicks(9191), null, "", new List<string>(), 1, 4, "", 0, null, null, "staff 123" },
                    { "learner003", "", "", "", new DateTime(2025, 1, 31, 9, 7, 57, 871, DateTimeKind.Utc).AddTicks(9190), null, null, null, new List<string>(), "learner003@gmail.com", new List<string>(), null, false, new DateTime(2025, 1, 31, 9, 7, 57, 871, DateTimeKind.Utc).AddTicks(9173), null, "", new List<string>(), 1, 1, "", 0, null, null, "learner003" },
                    { "teacher002", "", "", "", new DateTime(2025, 1, 31, 9, 7, 57, 871, DateTimeKind.Utc).AddTicks(9172), null, null, null, new List<string>(), "teacher002@gmail.com", new List<string>(), null, false, new DateTime(2025, 1, 31, 9, 7, 57, 871, DateTimeKind.Utc).AddTicks(9162), null, "", new List<string>(), 1, 2, "", 0, null, null, "teacher002" }
                });

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "Id", "ConfigName", "ConfigValue", "CreatedAt", "DeletedAt", "RecordStatus", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0a674fb0-cb37-4b75-916f-c6ba109bbdd3"), "Số buổi học 1 tuần LEVEL 3", "2", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4478), null, 1, 3, null },
                    { new Guid("0b24b502-e7c3-4b9d-bbf9-f5e47330142b"), "Sĩ số lớp tối thiểu", "8", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4086), null, 1, 3, null },
                    { new Guid("0bc9587d-b133-47eb-8ff3-f09fe5048df2"), "Tổng số buổi học LEVEL 4", "40", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4488), null, 1, 3, null },
                    { new Guid("16280470-7064-4122-b899-7bf5c91099ab"), "Tổng số buổi học LEVEL 3", "30", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4487), null, 1, 3, null },
                    { new Guid("1b028ebf-49c1-4acd-8558-36ef9e4d8429"), "Tổng số buổi học LEVEL 2", "30", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4486), null, 1, 3, null },
                    { new Guid("1cfb8865-f729-4701-ba47-40a44b1f3605"), "Mức phí theo buổi LEVEL 3", "300000", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4471), null, 1, 3, null },
                    { new Guid("504067fb-d30b-456a-940f-9e3f673c968f"), "Số buổi học 1 tuần LEVEL 4", "2", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4482), null, 1, 3, null },
                    { new Guid("5d717046-d87b-4be9-bea8-1ce81e9cf058"), "Mức phí theo buổi LEVEL 2", "250000", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4470), null, 1, 3, null },
                    { new Guid("5d992461-d08e-4e05-b798-527a55133d81"), "Mức phí theo buổi LEVEL 1", "200000", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4468), null, 1, 3, null },
                    { new Guid("7a5c8678-32cc-4bd0-a2a6-ae130b22dfcf"), "Tổng số buổi học LEVEL 5", "50", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4490), null, 1, 3, null },
                    { new Guid("8959f179-69d1-428c-bc26-be6d6fe5586f"), "Tổng số buổi học LEVEL 1", "30", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4484), null, 1, 3, null },
                    { new Guid("8dcbc46a-4913-4166-bc76-eae7d1fad57f"), "Số buổi học 1 tuần LEVEL 5", "2", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4483), null, 1, 3, null },
                    { new Guid("916d9924-5c07-4bda-a624-e55907a224a1"), "Mức phí theo buổi LEVEL 4", "350000", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4473), null, 1, 3, null },
                    { new Guid("c1151cb7-b0a4-4b6b-8737-43e76a70b119"), "Số buổi học 1 tuần LEVEL 2", "2", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4477), null, 1, 3, null },
                    { new Guid("d0aebc82-8cb8-4379-80c3-55f24aae8017"), "Sĩ số lớp tối đa", "12", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4461), null, 1, 3, null },
                    { new Guid("ed4e47fc-1506-4a0d-a672-f5525a74562f"), "Mức phí theo buổi LEVEL 5", "400000", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4474), null, 1, 3, null },
                    { new Guid("f4e7d205-20e7-4406-87c7-b54645cc4497"), "Số buổi học 1 tuần LEVEL 1", "2", new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(4475), null, 1, 3, null }
                });

            migrationBuilder.InsertData(
                table: "Class",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "InstructorId", "IsPublic", "IsScorePublished", "Level", "Name", "RecordStatus", "Status", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("344122a9-50aa-4e5c-b95a-abda904c0de2"), new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(1766), "admin001", null, null, "teacher002", true, false, 2, "Class 1", 1, 1, null, null },
                    { new Guid("97ddf810-f672-4e85-9440-97913c2c7509"), new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(2676), "admin001", null, null, "teacher002", true, false, 3, "Class 2", 1, 0, null, null }
                });

            migrationBuilder.InsertData(
                table: "Criteria",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Description", "For", "Name", "RecordStatus", "UpdateById", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { new Guid("0d08ea4a-64ae-4999-9799-b0b835af75a0"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(5196), "admin001", null, null, null, 1, "Thi cuối kỳ (Phong thái)", 1, null, null, 15m },
                    { new Guid("315599b5-4a8d-4698-9182-449a7761eae7"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(5157), "admin001", null, null, null, 0, "Độ chính xác", 1, null, null, 10m },
                    { new Guid("4f846c3f-4ae7-4efa-88f7-0538402dec1b"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(4231), "admin001", null, null, null, 0, "Nhịp điệu", 1, null, null, 10m },
                    { new Guid("50c66354-db13-4d4f-9352-836fedde29f0"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(5168), "admin001", null, null, null, 1, "Kiểm tra nhỏ 2", 1, null, null, 5m },
                    { new Guid("5eb77d0b-993f-47af-90a4-5d928c16ae6d"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(5170), "admin001", null, null, null, 1, "Bài thi 1", 1, null, null, 10m },
                    { new Guid("6e1e4892-7486-43f4-b67a-45e2d6501c22"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(5180), "admin001", null, null, null, 1, "Thi cuối kỳ (Nhịp điệu)", 1, null, null, 15m },
                    { new Guid("73e91e4d-2037-4d4e-babe-f37a783c0067"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(5172), "admin001", null, null, null, 1, "Bài thi 2", 1, null, null, 10m },
                    { new Guid("8f9ffccb-7905-4c1d-a3f1-214be1ce7368"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(5164), "admin001", null, null, null, 0, "Phong thái", 1, null, null, 10m },
                    { new Guid("acfa91c0-b9c4-4d2c-a882-0ddee908d6ff"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(5173), "admin001", null, null, null, 1, "Điểm chuyên cần", 1, null, null, 5m },
                    { new Guid("b1cef04e-356d-422c-8abe-007d496c314f"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(5166), "admin001", null, null, null, 1, "Kiểm tra nhỏ 1", 1, null, null, 5m },
                    { new Guid("b2274cea-4202-4a3c-986b-e6e686c12bfe"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(5194), "admin001", null, null, null, 1, "Thi cuối kỳ (Âm sắc)", 1, null, null, 15m },
                    { new Guid("c632e746-1dad-43a0-af6b-82d9a2c18579"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(5161), "admin001", null, null, null, 0, "Âm Sắc", 1, null, null, 10m },
                    { new Guid("db9fcc3a-2b8e-4784-ae10-6db690753c5b"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(5192), "admin001", null, null, null, 1, "Thi cuối kỳ (Độ chính xác)", 1, null, null, 15m }
                });

            migrationBuilder.InsertData(
                table: "Room",
                columns: new[] { "Id", "Capacity", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Name", "RecordStatus", "Status", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("47c85c7b-8b0d-4a38-95f4-55fb02fe9553"), null, new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(6297), "admin001", null, null, "Room 3", 1, 0, null, null },
                    { new Guid("ce4a4685-e47f-4183-bff6-4a905a7d2aea"), null, new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(6006), "admin001", null, null, "Room 1", 1, 0, null, null },
                    { new Guid("ceb11168-a50a-4480-9220-a7911e7f40f6"), null, new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(6295), "admin001", null, null, "Room 2", 1, 0, null, null }
                });

            migrationBuilder.InsertData(
                table: "EntranceTest",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "Date", "DeletedAt", "DeletedById", "Fee", "InstructorId", "InstructorName", "IsAnnouncedScore", "Name", "RecordStatus", "RoomCapacity", "RoomId", "RoomName", "Shift", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("39bfdc00-5888-4e3b-a93d-64a287ee7ec9"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(7752), "admin001", new DateOnly(2025, 1, 31), null, null, 0m, "teacher002", null, false, "EntranceTest 3", 1, null, new Guid("47c85c7b-8b0d-4a38-95f4-55fb02fe9553"), null, 4, null, null },
                    { new Guid("7836a33c-905d-4e2a-8532-dc3537f3ec0e"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(7746), "admin001", new DateOnly(2025, 1, 31), null, null, 0m, null, null, false, "EntranceTest 2", 1, null, new Guid("ceb11168-a50a-4480-9220-a7911e7f40f6"), "Room 2", 2, null, null },
                    { new Guid("aa7d1aa5-54fa-440f-840b-15f54d9c48ee"), new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(6785), "admin001", new DateOnly(2025, 1, 31), null, null, 0m, "teacher002", null, false, "EntranceTest 1", 1, null, new Guid("ce4a4685-e47f-4183-bff6-4a905a7d2aea"), "Room 1", 0, null, null }
                });

            migrationBuilder.InsertData(
                table: "Slot",
                columns: new[] { "Id", "ClassId", "CreatedAt", "Date", "DeletedAt", "RecordStatus", "RoomId", "Shift", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("6f7170e3-e576-4397-912f-6f1590c5c41f"), new Guid("344122a9-50aa-4e5c-b95a-abda904c0de2"), new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(3753), new DateOnly(2025, 2, 6), null, 1, new Guid("ce4a4685-e47f-4183-bff6-4a905a7d2aea"), 0, 1, null },
                    { new Guid("9e13a813-19b1-4081-a0e0-6ecbacc25d6c"), new Guid("344122a9-50aa-4e5c-b95a-abda904c0de2"), new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(3102), new DateOnly(2025, 1, 31), null, 1, new Guid("ce4a4685-e47f-4183-bff6-4a905a7d2aea"), 0, 1, null },
                    { new Guid("c8b1a605-5662-4ec0-b63c-8145e4eb6bc4"), new Guid("344122a9-50aa-4e5c-b95a-abda904c0de2"), new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(3751), new DateOnly(2025, 2, 4), null, 1, new Guid("ce4a4685-e47f-4183-bff6-4a905a7d2aea"), 0, 1, null },
                    { new Guid("df327c7a-5cad-488e-b742-42923a0788bc"), new Guid("344122a9-50aa-4e5c-b95a-abda904c0de2"), new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(3738), new DateOnly(2025, 2, 2), null, 1, new Guid("ce4a4685-e47f-4183-bff6-4a905a7d2aea"), 0, 1, null }
                });

            migrationBuilder.InsertData(
                table: "EntranceTestStudent",
                columns: new[] { "Id", "BandScore", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "EntranceTestId", "InstructorComment", "IsScoreAnnounced", "Rank", "RecordStatus", "StudentFirebaseId", "UpdateById", "UpdatedAt", "Year" },
                values: new object[,]
                {
                    { new Guid("01393f8f-f24f-46b3-9b19-cd577558b4c8"), 6m, new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(153), "admin001", null, null, new Guid("aa7d1aa5-54fa-440f-840b-15f54d9c48ee"), null, true, 3, 1, "learner003", null, null, 2024 },
                    { new Guid("02e83e9d-a419-4b23-be79-8950ab769507"), 9m, new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(133), "admin001", null, null, new Guid("aa7d1aa5-54fa-440f-840b-15f54d9c48ee"), null, true, 2, 1, "learner003", null, null, 2024 },
                    { new Guid("9d87daca-2fcc-479b-a5b3-ff12d2346a78"), 8m, new DateTime(2025, 1, 31, 9, 7, 57, 872, DateTimeKind.Utc).AddTicks(8354), "admin001", null, null, new Guid("aa7d1aa5-54fa-440f-840b-15f54d9c48ee"), null, true, 1, 1, "learner003", null, null, 2024 }
                });

            migrationBuilder.InsertData(
                table: "EntranceTestResult",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "CriteriaId", "CriteriaName", "DeletedAt", "DeletedById", "EntranceTestStudentId", "RecordStatus", "Score", "UpdateById", "UpdatedAt" },
                values: new object[] { new Guid("a8407adb-7dcd-4b32-b23f-ecb9337bfe3d"), new DateTime(2025, 1, 31, 9, 7, 57, 873, DateTimeKind.Utc).AddTicks(619), "admin001", new Guid("315599b5-4a8d-4698-9182-449a7761eae7"), null, null, null, new Guid("9d87daca-2fcc-479b-a5b3-ff12d2346a78"), 1, null, null, null });

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
