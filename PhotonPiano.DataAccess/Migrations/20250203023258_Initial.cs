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
                    { "1axRN4fG0ybZyDOYvO8wnkm5lHJ3", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8759), null, null, null, new List<string>(), "teacherphatlord@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8755), 2, "", new List<string>(), 1, 2, "", 0, null, null, "teacherphatlord@gmail.com" },
                    { "admin001", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(7104), null, null, null, new List<string>(), "admin001@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(5940), null, "", new List<string>(), 1, 3, "", 0, null, null, "admin001" },
                    { "buiducnamTest@gmail.com", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8642), null, null, null, new List<string>(), "teacher003@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8638), null, "", new List<string>(), 1, 2, "", 0, null, null, "buiducnamTest@gmail.com" },
                    { "gnRssA2sZHWnXB23oUuUxwz95Ln1", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8619), null, null, null, new List<string>(), "staff123@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8611), null, "", new List<string>(), 1, 4, "", 0, null, null, "staff 123" },
                    { "learner001", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8648), null, null, null, new List<string>(), "learner001@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8644), 0, "", new List<string>(), 1, 1, "", 0, 0, null, "learner001" },
                    { "learner002", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8654), null, null, null, new List<string>(), "learner002@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8650), 0, "", new List<string>(), 1, 1, "", 0, 0, null, "learner002" },
                    { "learner003", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8660), null, null, null, new List<string>(), "learner003@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8656), 0, "", new List<string>(), 1, 1, "", 0, 0, null, "learner003" },
                    { "learner004", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8673), null, null, null, new List<string>(), "learner004@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8662), 0, "", new List<string>(), 1, 1, "", 0, 0, null, "learner004" },
                    { "learner005", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8679), null, null, null, new List<string>(), "learner005@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8675), 0, "", new List<string>(), 1, 1, "", 0, 0, null, "learner005" },
                    { "learner006", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8685), null, null, null, new List<string>(), "learner006@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8681), 0, "", new List<string>(), 1, 1, "", 0, 0, null, "learner006" },
                    { "learner007", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8691), null, null, null, new List<string>(), "learner007@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8687), 0, "", new List<string>(), 1, 1, "", 0, 0, null, "learner007" },
                    { "learner008", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8707), null, null, null, new List<string>(), "learner008@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8693), 0, "", new List<string>(), 1, 1, "", 0, 0, null, "learner008" },
                    { "learner009", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8747), null, null, null, new List<string>(), "learner009@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8743), 0, "", new List<string>(), 1, 1, "", 0, 0, null, "learner009" },
                    { "learner010", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8753), null, null, null, new List<string>(), "learner010@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8749), 0, "", new List<string>(), 1, 1, "", 0, 0, null, "learner010" },
                    { "lymytranTest@gmail.com", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8636), null, null, null, new List<string>(), "teacher002@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(8621), null, "", new List<string>(), 1, 2, "", 0, null, null, "lymytranTest@gmail.com" },
                    { "nQhzMDSe8aW5RLerTaHa6yvh8c23", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(7856), null, null, null, new List<string>(), "minh@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(7852), 0, "", new List<string>(), 1, 1, "", 0, 0, null, "minh@gmail.com" },
                    { "quachthemyTest@gmail.com", "", "", "", new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(7851), null, null, null, new List<string>(), "teacher002@gmail.com", new List<string>(), null, false, new DateTime(2025, 2, 3, 9, 32, 56, 741, DateTimeKind.Utc).AddTicks(7839), null, "", new List<string>(), 1, 2, "", 0, null, null, "quachthemyTest@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "Id", "ConfigName", "ConfigValue", "CreatedAt", "DeletedAt", "RecordStatus", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0af45abc-3958-4ec2-94c6-8a0706750544"), "Sĩ số lớp tối đa", "12", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2933), null, 1, 3, null },
                    { new Guid("0d2411c5-7531-4278-b51a-6c67e12dfa51"), "Sĩ số lớp tối thiểu", "8", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2463), null, 1, 3, null },
                    { new Guid("1407552a-eefd-4f70-9384-3b40dc2d2f91"), "Số buổi học 1 tuần LEVEL 1", "2", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2947), null, 1, 3, null },
                    { new Guid("246f22f8-d150-47f0-85d4-70619698232e"), "Mức phí theo buổi LEVEL 3", "300000", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2941), null, 1, 3, null },
                    { new Guid("2eb7e7d9-b409-4b86-a816-9082c38ac334"), "Mức phí theo buổi LEVEL 4", "350000", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2943), null, 1, 3, null },
                    { new Guid("3798a332-eceb-45ea-bab6-d87030799bae"), "Tổng số buổi học LEVEL 2", "30", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2962), null, 1, 3, null },
                    { new Guid("463ba9b7-cb59-4391-857c-d993e891c1fa"), "Tổng số buổi học LEVEL 5", "50", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2980), null, 1, 3, null },
                    { new Guid("4c58bdd1-fd42-4ab4-a0fe-542906ee4f99"), "Mức phí theo buổi LEVEL 1", "200000", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2937), null, 1, 3, null },
                    { new Guid("56f7364d-e463-496f-80d0-aa11a454f48e"), "Số buổi học 1 tuần LEVEL 2", "2", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2953), null, 1, 3, null },
                    { new Guid("60cbb29f-ca2e-4e6a-b6e3-0378f101ca2d"), "Số buổi học 1 tuần LEVEL 5", "2", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2959), null, 1, 3, null },
                    { new Guid("705a1748-872e-41c1-8d39-b1338550d889"), "Mức phí theo buổi LEVEL 2", "250000", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2939), null, 1, 3, null },
                    { new Guid("73a3cb14-9509-4769-9a16-8729a115a8f3"), "Tổng số buổi học LEVEL 3", "30", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2974), null, 1, 3, null },
                    { new Guid("77ed50da-a4ae-444c-8201-0ce108b38c00"), "Mức phí theo buổi LEVEL 5", "400000", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2945), null, 1, 3, null },
                    { new Guid("989877a3-fe9e-4acd-ac48-5bc11f9e1340"), "Số buổi học 1 tuần LEVEL 3", "2", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2955), null, 1, 3, null },
                    { new Guid("bda30db5-4f9e-439d-add4-763e1348bb3c"), "Tổng số buổi học LEVEL 4", "40", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2976), null, 1, 3, null },
                    { new Guid("d576317c-78d2-48a5-a1a9-df8b698def50"), "Tổng số buổi học LEVEL 1", "30", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2961), null, 1, 3, null },
                    { new Guid("f1e8ca51-9dbe-4bba-86e5-2d5b61852cdc"), "Số buổi học 1 tuần LEVEL 4", "2", new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(2957), null, 1, 3, null }
                });

            migrationBuilder.InsertData(
                table: "Class",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "InstructorId", "IsPublic", "IsScorePublished", "Level", "Name", "RecordStatus", "Status", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00334d0c-0e4e-4b96-97f6-22e3eb60e642"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(9697), "admin001", null, null, "buiducnamTest@gmail.com", true, false, 0, "Class 8", 1, 1, null, null },
                    { new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(9682), "admin001", null, null, "quachthemyTest@gmail.com", true, false, 3, "Class 2", 1, 0, null, null },
                    { new Guid("2a31f9fa-b927-4b52-95ac-e534ddb229a6"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(9693), "admin001", null, null, "lymytranTest@gmail.com", true, false, 2, "Class 6", 1, 1, null, null },
                    { new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(9691), "admin001", null, null, "1axRN4fG0ybZyDOYvO8wnkm5lHJ3", true, false, 3, "Class Teacher Phat", 1, 1, null, null },
                    { new Guid("51f1cf5f-0d80-4c1d-85a2-0afdab7300c3"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(9695), "admin001", null, null, "quachthemyTest@gmail.com", true, false, 3, "Class 7", 1, 0, null, null },
                    { new Guid("8af29ceb-c2f3-4357-9aab-936b754e4880"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(9699), "admin001", null, null, "quachthemyTest@gmail.com", true, false, 2, "Class 9", 1, 1, null, null },
                    { new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(8762), "admin001", null, null, "lymytranTest@gmail.com", true, false, 2, "Class 1", 1, 1, null, null },
                    { new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(9685), "admin001", null, null, "buiducnamTest@gmail.com", true, false, 0, "Class 3", 1, 1, null, null },
                    { new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(9689), "admin001", null, null, "buiducnamTest@gmail.com", true, false, 3, "Class 5", 1, 0, null, null },
                    { new Guid("f24bd24e-5899-4fd3-94b1-b68689aaaf44"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(9701), "admin001", null, null, "buiducnamTest@gmail.com", true, false, 3, "Class 10", 1, 0, null, null },
                    { new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(9687), "admin001", null, null, "quachthemyTest@gmail.com", true, false, 2, "Class 4", 1, 1, null, null }
                });

            migrationBuilder.InsertData(
                table: "Criteria",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Description", "For", "Name", "RecordStatus", "UpdateById", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { new Guid("0ec5f2ea-3f3e-4924-860b-2546b25bdee5"), new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(8370), "admin001", null, null, null, 1, "Điểm chuyên cần", 1, null, null, 5m },
                    { new Guid("2091a9cc-e885-40b3-aa22-2fed7fc54005"), new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(8375), "admin001", null, null, null, 1, "Thi cuối kỳ (Độ chính xác)", 1, null, null, 15m },
                    { new Guid("253fddc7-c1c4-4ece-a601-a72f0d89408d"), new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(8368), "admin001", null, null, null, 1, "Bài thi 2", 1, null, null, 10m },
                    { new Guid("442bb8f7-b831-47c9-ad63-558ed6b604fc"), new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(7406), "admin001", null, null, null, 0, "Nhịp điệu", 1, null, null, 10m },
                    { new Guid("491bb4f1-f284-415c-a691-561700d24592"), new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(8377), "admin001", null, null, null, 1, "Thi cuối kỳ (Âm sắc)", 1, null, null, 15m },
                    { new Guid("55befe1c-0e55-49c4-8c94-5470905c2f45"), new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(8331), "admin001", null, null, null, 1, "Kiểm tra nhỏ 1", 1, null, null, 5m },
                    { new Guid("695401bb-c035-4c0b-a016-116dfa9d5e7e"), new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(8372), "admin001", null, null, null, 1, "Thi cuối kỳ (Nhịp điệu)", 1, null, null, 15m },
                    { new Guid("744b5522-e130-4097-b5b3-8becc0e349d0"), new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(8419), "admin001", null, null, null, 1, "Thi cuối kỳ (Phong thái)", 1, null, null, 15m },
                    { new Guid("8455165b-eae2-4132-803a-7c192390d44e"), new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(8321), "admin001", null, null, null, 0, "Độ chính xác", 1, null, null, 10m },
                    { new Guid("918fa0cd-c580-4c00-a3dc-8ffa40bae3b0"), new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(8323), "admin001", null, null, null, 0, "Âm Sắc", 1, null, null, 10m },
                    { new Guid("9d464762-907f-4a01-8f4f-49b8eb9d6978"), new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(8346), "admin001", null, null, null, 1, "Kiểm tra nhỏ 2", 1, null, null, 5m },
                    { new Guid("e7be5c1e-2216-49b3-9019-df0289f57689"), new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(8328), "admin001", null, null, null, 0, "Phong thái", 1, null, null, 10m },
                    { new Guid("f6f95547-870f-4dd7-a076-c6fddf13bc5b"), new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(8365), "admin001", null, null, null, 1, "Bài thi 1", 1, null, null, 10m }
                });

            migrationBuilder.InsertData(
                table: "DayOff",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "EndTime", "Name", "RecordStatus", "StartTime", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("03529570-2c3b-4d0f-967e-e96a66839536"), new DateTime(2025, 2, 3, 9, 32, 56, 745, DateTimeKind.Utc).AddTicks(1782), "admin001", null, null, new DateTime(2025, 9, 2, 23, 59, 59, 0, DateTimeKind.Utc), "Ngày Quốc Khánh", 1, new DateTime(2025, 9, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, null },
                    { new Guid("0e9926ea-becb-45bf-9a35-d543e7fd4afd"), new DateTime(2025, 2, 3, 9, 32, 56, 745, DateTimeKind.Utc).AddTicks(1762), "admin001", null, null, new DateTime(2025, 4, 21, 23, 59, 59, 0, DateTimeKind.Utc), "Giỗ Tổ Hùng Vương", 1, new DateTime(2025, 4, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null },
                    { new Guid("31d07151-c056-474b-9c5a-6ecf7c8df6c7"), new DateTime(2025, 2, 3, 9, 32, 56, 745, DateTimeKind.Utc).AddTicks(1754), "admin001", null, null, new DateTime(2025, 2, 16, 23, 59, 59, 0, DateTimeKind.Utc), "Tết Nguyên Đán", 1, new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, null },
                    { new Guid("55b91700-c3cc-4c3e-9dc2-81c1e27e88f2"), new DateTime(2025, 2, 3, 9, 32, 56, 745, DateTimeKind.Utc).AddTicks(1765), "admin001", null, null, new DateTime(2025, 4, 30, 23, 59, 59, 0, DateTimeKind.Utc), "Ngày Giải Phóng Miền Nam", 1, new DateTime(2025, 4, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, null },
                    { new Guid("977e0abe-2034-47d9-9e38-db84e64dd634"), new DateTime(2025, 2, 3, 9, 32, 56, 745, DateTimeKind.Utc).AddTicks(1769), "admin001", null, null, new DateTime(2025, 5, 1, 23, 59, 59, 0, DateTimeKind.Utc), "Ngày Quốc Tế Lao Động", 1, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null },
                    { new Guid("dcf44144-e87a-4169-925a-54dac3de805a"), new DateTime(2025, 2, 3, 9, 32, 56, 745, DateTimeKind.Utc).AddTicks(893), "admin001", null, null, new DateTime(2025, 1, 1, 23, 59, 59, 0, DateTimeKind.Utc), "Tết Dương Lịch", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null }
                });

            migrationBuilder.InsertData(
                table: "Room",
                columns: new[] { "Id", "Capacity", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Name", "RecordStatus", "Status", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0855d926-ebe2-4b26-873a-ca9629308615"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(595), "admin001", null, null, "Room 7", 1, 0, null, null },
                    { new Guid("10abe3db-678a-4e25-8baa-0db70840a71e"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(588), "admin001", null, null, "Room 4", 1, 0, null, null },
                    { new Guid("1ac38e12-c060-4a9e-80c7-aff5417fa5c8"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(598), "admin001", null, null, "Room 9", 1, 0, null, null },
                    { new Guid("1bdf901f-6242-4e8b-9bf8-aeddedab2816"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(612), "admin001", null, null, "Room 16", 1, 0, null, null },
                    { new Guid("3d60ed18-8f46-412d-a8cf-5fb6a09b4f6f"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(594), "admin001", null, null, "Room 6", 1, 0, null, null },
                    { new Guid("3ee45d1e-a3f8-4e00-8fe1-add0dc2a4f56"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(610), "admin001", null, null, "Room 15", 1, 0, null, null },
                    { new Guid("53cfb467-d82f-4f52-8a0c-7e95e169d441"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(613), "admin001", null, null, "Room 17", 1, 0, null, null },
                    { new Guid("6f3ca661-e46c-4bce-8c93-88132a8bd591"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(587), "admin001", null, null, "Room 3", 1, 0, null, null },
                    { new Guid("7b4bcd62-2fb3-41c6-bb1e-f672a6ffe4b6"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(606), "admin001", null, null, "Room 12", 1, 0, null, null },
                    { new Guid("81d84b3b-9191-437f-b2e8-1fa832fda1ad"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(601), "admin001", null, null, "Room 10", 1, 0, null, null },
                    { new Guid("9a52dbd7-8ffc-4107-af9a-c353274c8daa"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(609), "admin001", null, null, "Room 14", 1, 0, null, null },
                    { new Guid("9a8881eb-8665-4ca4-88ac-695067744b89"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(579), "admin001", null, null, "Room 2", 1, 0, null, null },
                    { new Guid("a5873185-9299-4227-93d9-467871515725"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(630), "admin001", null, null, "Room 20", 1, 0, null, null },
                    { new Guid("c2faa8f7-27f8-4945-a8f8-c4bde3ec98f0"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(627), "admin001", null, null, "Room 18", 1, 0, null, null },
                    { new Guid("c38290c7-a3b7-4588-9508-147e07b86bda"), null, new DateTime(2025, 2, 3, 9, 32, 56, 742, DateTimeKind.Utc).AddTicks(9527), "admin001", null, null, "Room 1", 1, 0, null, null },
                    { new Guid("ca793127-ebb7-48e9-a1f1-4a85514232f9"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(629), "admin001", null, null, "Room 19", 1, 0, null, null },
                    { new Guid("e2670299-269d-409f-ba72-244e1394a451"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(607), "admin001", null, null, "Room 13", 1, 0, null, null },
                    { new Guid("e2cb1e7c-8d40-4378-917f-9da722f29bd4"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(597), "admin001", null, null, "Room 8", 1, 0, null, null },
                    { new Guid("f966ac34-6c22-4b93-b5e4-314c93cf3dfc"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(604), "admin001", null, null, "Room 11", 1, 0, null, null },
                    { new Guid("fdee01f3-16c2-4fe9-ac6e-230a86fed218"), null, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(590), "admin001", null, null, "Room 5", 1, 0, null, null }
                });

            migrationBuilder.InsertData(
                table: "EntranceTest",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "Date", "DeletedAt", "DeletedById", "Fee", "InstructorId", "InstructorName", "IsAnnouncedScore", "Name", "RecordStatus", "RoomCapacity", "RoomId", "RoomName", "Shift", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("3d99606e-92fb-441a-bb8a-c8d2c5fc5ff2"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(2084), "admin001", new DateOnly(2025, 2, 3), null, null, 0m, "quachthemyTest@gmail.com", null, false, "EntranceTest 1", 1, null, new Guid("c38290c7-a3b7-4588-9508-147e07b86bda"), "Room 1", 0, null, null },
                    { new Guid("8b583e33-6c8e-49cc-9dc5-ccaaac44492f"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(3588), "admin001", new DateOnly(2025, 2, 3), null, null, 0m, null, null, false, "EntranceTest 2", 1, null, new Guid("9a8881eb-8665-4ca4-88ac-695067744b89"), "Room 2", 2, null, null },
                    { new Guid("c5b0c2d3-8980-4c9c-ab2f-4b535510c4e3"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(3596), "admin001", new DateOnly(2025, 2, 3), null, null, 0m, "quachthemyTest@gmail.com", null, false, "EntranceTest 3", 1, null, new Guid("6f3ca661-e46c-4bce-8c93-88132a8bd591"), null, 4, null, null }
                });

            migrationBuilder.InsertData(
                table: "Slot",
                columns: new[] { "Id", "ClassId", "CreatedAt", "Date", "DeletedAt", "RecordStatus", "RoomId", "Shift", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("011ae32f-8574-4f78-98d0-96a19fc484cb"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1483), new DateOnly(2025, 2, 6), null, 1, new Guid("10abe3db-678a-4e25-8baa-0db70840a71e"), 3, 1, null },
                    { new Guid("01264359-3a2d-450c-822d-6c14e4ca37d3"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1556), new DateOnly(2025, 2, 11), null, 1, new Guid("1ac38e12-c060-4a9e-80c7-aff5417fa5c8"), 3, 1, null },
                    { new Guid("01b06b42-183c-46dc-9647-252cbd4ae82d"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1476), new DateOnly(2025, 2, 22), null, 1, new Guid("a5873185-9299-4227-93d9-467871515725"), 4, 1, null },
                    { new Guid("039fc807-20f1-48ae-beb3-b8145bbb45f6"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1529), new DateOnly(2025, 2, 19), null, 1, new Guid("53cfb467-d82f-4f52-8a0c-7e95e169d441"), 1, 1, null },
                    { new Guid("0d13ab1c-a99b-4a9b-9337-74987e881ff4"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1504), new DateOnly(2025, 2, 21), null, 1, new Guid("ca793127-ebb7-48e9-a1f1-4a85514232f9"), 3, 1, null },
                    { new Guid("0f1fd977-a5cb-483b-baed-1be5ea2f83fd"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1381), new DateOnly(2025, 2, 8), null, 1, new Guid("3d60ed18-8f46-412d-a8cf-5fb6a09b4f6f"), 0, 1, null },
                    { new Guid("11026381-2cf1-4560-a48c-c5477dca1744"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1525), new DateOnly(2025, 2, 16), null, 1, new Guid("9a52dbd7-8ffc-4107-af9a-c353274c8daa"), 3, 1, null },
                    { new Guid("11c8d104-8cad-467c-90ab-9ee343af32aa"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1424), new DateOnly(2025, 2, 11), null, 1, new Guid("1ac38e12-c060-4a9e-80c7-aff5417fa5c8"), 3, 1, null },
                    { new Guid("145e426c-4a4f-45ee-bc96-0c4d65c150c4"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1560), new DateOnly(2025, 2, 14), null, 1, new Guid("7b4bcd62-2fb3-41c6-bb1e-f672a6ffe4b6"), 1, 1, null },
                    { new Guid("154542d1-1d70-4679-847a-6a0cfed78fa1"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1418), new DateOnly(2025, 2, 6), null, 1, new Guid("10abe3db-678a-4e25-8baa-0db70840a71e"), 3, 1, null },
                    { new Guid("1ade82a7-8916-48fb-add9-46029b987661"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1415), new DateOnly(2025, 2, 4), null, 1, new Guid("9a8881eb-8665-4ca4-88ac-695067744b89"), 1, 1, null },
                    { new Guid("2260d31c-075b-48e9-aedb-8ac9d1cf32d0"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1505), new DateOnly(2025, 2, 22), null, 1, new Guid("a5873185-9299-4227-93d9-467871515725"), 4, 1, null },
                    { new Guid("22edf58f-bf3f-4b04-a2f0-56304e8dae80"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1377), new DateOnly(2025, 2, 7), null, 1, new Guid("fdee01f3-16c2-4fe9-ac6e-230a86fed218"), 4, 1, null },
                    { new Guid("27641c12-fc9c-491c-8355-f292df006482"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1439), new DateOnly(2025, 2, 21), null, 1, new Guid("ca793127-ebb7-48e9-a1f1-4a85514232f9"), 3, 1, null },
                    { new Guid("2820b31f-f228-4c48-9ff8-0c5f7e75f0e1"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1496), new DateOnly(2025, 2, 15), null, 1, new Guid("e2670299-269d-409f-ba72-244e1394a451"), 2, 1, null },
                    { new Guid("2f731439-0c60-410a-add5-a84373cf0cdd"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1550), new DateOnly(2025, 2, 6), null, 1, new Guid("10abe3db-678a-4e25-8baa-0db70840a71e"), 3, 1, null },
                    { new Guid("31cf6296-8eb1-409e-8126-803a00c9be72"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1454), new DateOnly(2025, 2, 11), null, 1, new Guid("1ac38e12-c060-4a9e-80c7-aff5417fa5c8"), 3, 1, null },
                    { new Guid("348bdcee-c6bc-4de3-bb5a-ef3bd1c4a767"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1511), new DateOnly(2025, 2, 5), null, 1, new Guid("6f3ca661-e46c-4bce-8c93-88132a8bd591"), 2, 1, null },
                    { new Guid("36903caa-4406-43e6-b456-874049ca9c27"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1474), new DateOnly(2025, 2, 21), null, 1, new Guid("ca793127-ebb7-48e9-a1f1-4a85514232f9"), 3, 1, null },
                    { new Guid("36fa002d-ddef-44c2-952d-f8a8dc92cb13"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1554), new DateOnly(2025, 2, 9), null, 1, new Guid("0855d926-ebe2-4b26-873a-ca9629308615"), 1, 1, null },
                    { new Guid("3e2531f0-3d8b-4be7-b3e0-57f075c80d5f"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1522), new DateOnly(2025, 2, 14), null, 1, new Guid("7b4bcd62-2fb3-41c6-bb1e-f672a6ffe4b6"), 1, 1, null },
                    { new Guid("4acf0f53-e7fc-4120-be15-5ecb266139ed"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1477), new DateOnly(2025, 2, 3), null, 1, new Guid("c38290c7-a3b7-4588-9508-147e07b86bda"), 0, 1, null },
                    { new Guid("4e147947-1d48-4252-8c61-7628ac51418b"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(505), new DateOnly(2025, 2, 3), null, 1, new Guid("c38290c7-a3b7-4588-9508-147e07b86bda"), 0, 1, null },
                    { new Guid("4ed5ea2e-a3fe-4d35-a23f-b1f26fc34b3e"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1446), new DateOnly(2025, 2, 5), null, 1, new Guid("6f3ca661-e46c-4bce-8c93-88132a8bd591"), 2, 1, null },
                    { new Guid("4f1444a3-f152-47b1-bc3d-5ea20898483a"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1567), new DateOnly(2025, 2, 19), null, 1, new Guid("53cfb467-d82f-4f52-8a0c-7e95e169d441"), 1, 1, null },
                    { new Guid("50e4ce08-e62d-4faf-a85f-e0b20e7f7fa6"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1419), new DateOnly(2025, 2, 7), null, 1, new Guid("fdee01f3-16c2-4fe9-ac6e-230a86fed218"), 4, 1, null },
                    { new Guid("555a4523-171b-49c4-8da0-34873d510cec"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1542), new DateOnly(2025, 2, 21), null, 1, new Guid("ca793127-ebb7-48e9-a1f1-4a85514232f9"), 3, 1, null },
                    { new Guid("58187303-8b99-4e57-b190-2edf770bc193"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1395), new DateOnly(2025, 2, 18), null, 1, new Guid("1bdf901f-6242-4e8b-9bf8-aeddedab2816"), 0, 1, null },
                    { new Guid("5a305858-dbbb-4b49-a6b5-7f8c03205a9a"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1562), new DateOnly(2025, 2, 15), null, 1, new Guid("e2670299-269d-409f-ba72-244e1394a451"), 2, 1, null },
                    { new Guid("5a873ca8-f57b-400a-8670-9de8b34fa0dc"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1553), new DateOnly(2025, 2, 8), null, 1, new Guid("3d60ed18-8f46-412d-a8cf-5fb6a09b4f6f"), 0, 1, null },
                    { new Guid("5b4923a9-a6ca-49bc-b385-8cbb4aa8156b"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1371), new DateOnly(2025, 2, 4), null, 1, new Guid("9a8881eb-8665-4ca4-88ac-695067744b89"), 1, 1, null },
                    { new Guid("5cefa8be-2da0-4acf-b91d-bf6e47d640b2"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1420), new DateOnly(2025, 2, 8), null, 1, new Guid("3d60ed18-8f46-412d-a8cf-5fb6a09b4f6f"), 0, 1, null },
                    { new Guid("5e068985-1b4e-472a-9e69-dd6188677e04"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1516), new DateOnly(2025, 2, 9), null, 1, new Guid("0855d926-ebe2-4b26-873a-ca9629308615"), 1, 1, null },
                    { new Guid("5ef767e1-67cf-42bb-89b5-e55d422439d8"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1389), new DateOnly(2025, 2, 13), null, 1, new Guid("f966ac34-6c22-4b93-b5e4-314c93cf3dfc"), 0, 1, null },
                    { new Guid("683b5749-bd3a-4e20-8ed3-22dc15209195"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1456), new DateOnly(2025, 2, 13), null, 1, new Guid("f966ac34-6c22-4b93-b5e4-314c93cf3dfc"), 0, 1, null },
                    { new Guid("68690226-74a0-419a-aa02-aba402517db4"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1482), new DateOnly(2025, 2, 5), null, 1, new Guid("6f3ca661-e46c-4bce-8c93-88132a8bd591"), 2, 1, null },
                    { new Guid("68c6cca4-458f-4cbd-b373-0641d50e709e"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1442), new DateOnly(2025, 2, 3), null, 1, new Guid("c38290c7-a3b7-4588-9508-147e07b86bda"), 0, 1, null },
                    { new Guid("6df8ad01-7386-4116-aaae-30a03e142962"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1460), new DateOnly(2025, 2, 16), null, 1, new Guid("9a52dbd7-8ffc-4107-af9a-c353274c8daa"), 3, 1, null },
                    { new Guid("706ed9de-dfc7-45c3-a9ee-638c66603143"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1521), new DateOnly(2025, 2, 13), null, 1, new Guid("f966ac34-6c22-4b93-b5e4-314c93cf3dfc"), 0, 1, null },
                    { new Guid("7364f791-5f62-4858-a80f-01c96eaf55cc"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1376), new DateOnly(2025, 2, 6), null, 1, new Guid("10abe3db-678a-4e25-8baa-0db70840a71e"), 3, 1, null },
                    { new Guid("7690f536-eed6-41ce-9344-f147e48128d1"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1564), new DateOnly(2025, 2, 17), null, 1, new Guid("3ee45d1e-a3f8-4e00-8fe1-add0dc2a4f56"), 4, 1, null },
                    { new Guid("77cc660a-2af6-4982-96a4-64a85f68698f"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1436), new DateOnly(2025, 2, 19), null, 1, new Guid("53cfb467-d82f-4f52-8a0c-7e95e169d441"), 1, 1, null },
                    { new Guid("79ce53d0-9756-40dd-a4f5-86efc16cd4ed"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1501), new DateOnly(2025, 2, 19), null, 1, new Guid("53cfb467-d82f-4f52-8a0c-7e95e169d441"), 1, 1, null },
                    { new Guid("7b14ad04-7499-4000-a053-3fd7d87ca132"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1440), new DateOnly(2025, 2, 22), null, 1, new Guid("a5873185-9299-4227-93d9-467871515725"), 4, 1, null },
                    { new Guid("7c84f59a-4712-4d12-b244-e4230812f874"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1429), new DateOnly(2025, 2, 15), null, 1, new Guid("e2670299-269d-409f-ba72-244e1394a451"), 2, 1, null },
                    { new Guid("7ca153c1-4079-4a92-be13-6fe64607f0de"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1555), new DateOnly(2025, 2, 10), null, 1, new Guid("e2cb1e7c-8d40-4378-917f-9da722f29bd4"), 2, 1, null },
                    { new Guid("7d033af7-bdd9-4855-8d09-82629f2a303b"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1543), new DateOnly(2025, 2, 22), null, 1, new Guid("a5873185-9299-4227-93d9-467871515725"), 4, 1, null },
                    { new Guid("80940ef4-8bef-48d0-9299-be07d0f95782"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1489), new DateOnly(2025, 2, 10), null, 1, new Guid("e2cb1e7c-8d40-4378-917f-9da722f29bd4"), 2, 1, null },
                    { new Guid("83a2b881-1346-49c8-8680-b5d445fc0641"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1487), new DateOnly(2025, 2, 8), null, 1, new Guid("3d60ed18-8f46-412d-a8cf-5fb6a09b4f6f"), 0, 1, null },
                    { new Guid("840fc1d6-628d-4bb6-aeef-937ac85b8b01"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1408), new DateOnly(2025, 2, 20), null, 1, new Guid("c2faa8f7-27f8-4945-a8f8-c4bde3ec98f0"), 2, 1, null },
                    { new Guid("87a93920-4b52-4b64-8d52-975a9d5035c5"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1463), new DateOnly(2025, 2, 18), null, 1, new Guid("1bdf901f-6242-4e8b-9bf8-aeddedab2816"), 0, 1, null },
                    { new Guid("882ae160-fb8b-4ff2-8552-8057237b1330"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1519), new DateOnly(2025, 2, 11), null, 1, new Guid("1ac38e12-c060-4a9e-80c7-aff5417fa5c8"), 3, 1, null },
                    { new Guid("8b8514eb-1c9e-4cc1-bc58-17b520fe9716"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1524), new DateOnly(2025, 2, 15), null, 1, new Guid("e2670299-269d-409f-ba72-244e1394a451"), 2, 1, null },
                    { new Guid("915a7cff-22ca-4242-8038-9c0cd91af6f8"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1498), new DateOnly(2025, 2, 17), null, 1, new Guid("3ee45d1e-a3f8-4e00-8fe1-add0dc2a4f56"), 4, 1, null },
                    { new Guid("9200a3f4-5955-4505-914d-b0eb66f9dc7e"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1515), new DateOnly(2025, 2, 8), null, 1, new Guid("3d60ed18-8f46-412d-a8cf-5fb6a09b4f6f"), 0, 1, null },
                    { new Guid("9202f9ab-7844-4823-ac27-71b3b6782f88"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1495), new DateOnly(2025, 2, 14), null, 1, new Guid("7b4bcd62-2fb3-41c6-bb1e-f672a6ffe4b6"), 1, 1, null },
                    { new Guid("92d61cbc-fbf3-41ac-85fc-11f0c1a28186"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1568), new DateOnly(2025, 2, 20), null, 1, new Guid("c2faa8f7-27f8-4945-a8f8-c4bde3ec98f0"), 2, 1, null },
                    { new Guid("9347cab2-df18-4caa-a94c-19191678e881"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1517), new DateOnly(2025, 2, 10), null, 1, new Guid("e2cb1e7c-8d40-4378-917f-9da722f29bd4"), 2, 1, null },
                    { new Guid("958f872d-0221-4e6d-bc27-93bdf2f18adf"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1391), new DateOnly(2025, 2, 15), null, 1, new Guid("e2670299-269d-409f-ba72-244e1394a451"), 2, 1, null },
                    { new Guid("99d8eb9c-2343-4baf-b417-fd0ebf1dd45c"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1452), new DateOnly(2025, 2, 10), null, 1, new Guid("e2cb1e7c-8d40-4378-917f-9da722f29bd4"), 2, 1, null },
                    { new Guid("9b57d396-d566-4074-93a4-709c7f88665c"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1432), new DateOnly(2025, 2, 16), null, 1, new Guid("9a52dbd7-8ffc-4107-af9a-c353274c8daa"), 3, 1, null },
                    { new Guid("9c765e6e-c77c-4e4f-b2bc-e9397b3d4373"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1434), new DateOnly(2025, 2, 17), null, 1, new Guid("3ee45d1e-a3f8-4e00-8fe1-add0dc2a4f56"), 4, 1, null },
                    { new Guid("a3c22019-836b-41b6-ba5b-fab1ac0ad1b0"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1417), new DateOnly(2025, 2, 5), null, 1, new Guid("6f3ca661-e46c-4bce-8c93-88132a8bd591"), 2, 1, null },
                    { new Guid("a67092c6-9810-4f29-b622-5696d48bb229"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1500), new DateOnly(2025, 2, 18), null, 1, new Guid("1bdf901f-6242-4e8b-9bf8-aeddedab2816"), 0, 1, null },
                    { new Guid("a6ce80e3-0009-49db-95ce-f141fac3cfaf"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1459), new DateOnly(2025, 2, 15), null, 1, new Guid("e2670299-269d-409f-ba72-244e1394a451"), 2, 1, null },
                    { new Guid("a8406847-bf38-4b0f-a3fd-be17679ef7a8"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1520), new DateOnly(2025, 2, 12), null, 1, new Guid("81d84b3b-9191-437f-b2e8-1fa832fda1ad"), 4, 1, null },
                    { new Guid("a9f2d799-6ddb-4e52-bea4-bcd7c9344c98"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1492), new DateOnly(2025, 2, 12), null, 1, new Guid("81d84b3b-9191-437f-b2e8-1fa832fda1ad"), 4, 1, null },
                    { new Guid("af882899-0393-4094-abd3-d01f1dbd591a"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1569), new DateOnly(2025, 2, 21), null, 1, new Guid("ca793127-ebb7-48e9-a1f1-4a85514232f9"), 3, 1, null },
                    { new Guid("b5ff1be4-457a-4531-8fc0-731585033d65"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1528), new DateOnly(2025, 2, 18), null, 1, new Guid("1bdf901f-6242-4e8b-9bf8-aeddedab2816"), 0, 1, null },
                    { new Guid("b6269d88-ff91-410f-977b-09987052789b"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1393), new DateOnly(2025, 2, 16), null, 1, new Guid("9a52dbd7-8ffc-4107-af9a-c353274c8daa"), 3, 1, null },
                    { new Guid("b694ee05-7bc0-42c8-935c-14ed4bfbd394"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1397), new DateOnly(2025, 2, 19), null, 1, new Guid("53cfb467-d82f-4f52-8a0c-7e95e169d441"), 1, 1, null },
                    { new Guid("b78d1d22-8806-455a-9a45-fac21714167e"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1438), new DateOnly(2025, 2, 20), null, 1, new Guid("c2faa8f7-27f8-4945-a8f8-c4bde3ec98f0"), 2, 1, null },
                    { new Guid("b9bb7c92-5a04-46d9-a881-0ed8a9c6ecb8"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1461), new DateOnly(2025, 2, 17), null, 1, new Guid("3ee45d1e-a3f8-4e00-8fe1-add0dc2a4f56"), 4, 1, null },
                    { new Guid("bd482162-7de6-45b8-86a2-0e8dc397d1c9"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1509), new DateOnly(2025, 2, 4), null, 1, new Guid("9a8881eb-8665-4ca4-88ac-695067744b89"), 1, 1, null },
                    { new Guid("c1c91dc7-6e70-4eb6-ba06-548cfe15aaed"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1549), new DateOnly(2025, 2, 5), null, 1, new Guid("6f3ca661-e46c-4bce-8c93-88132a8bd591"), 2, 1, null },
                    { new Guid("c4c36d25-79da-46ce-9e69-3774545e7c3e"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1455), new DateOnly(2025, 2, 12), null, 1, new Guid("81d84b3b-9191-437f-b2e8-1fa832fda1ad"), 4, 1, null },
                    { new Guid("c70910c0-c236-4d3e-8df5-cc6d4647642f"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1563), new DateOnly(2025, 2, 16), null, 1, new Guid("9a52dbd7-8ffc-4107-af9a-c353274c8daa"), 3, 1, null },
                    { new Guid("c7a26b9e-da22-4a2b-9b51-7d52d99baa2a"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1465), new DateOnly(2025, 2, 20), null, 1, new Guid("c2faa8f7-27f8-4945-a8f8-c4bde3ec98f0"), 2, 1, null },
                    { new Guid("c7d9abc3-b445-4da6-b5ab-e45fcdc49738"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1558), new DateOnly(2025, 2, 12), null, 1, new Guid("81d84b3b-9191-437f-b2e8-1fa832fda1ad"), 4, 1, null },
                    { new Guid("c985f6f5-83d0-4e5a-9a53-401de650823a"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1559), new DateOnly(2025, 2, 13), null, 1, new Guid("f966ac34-6c22-4b93-b5e4-314c93cf3dfc"), 0, 1, null },
                    { new Guid("c9d66d2d-acf3-4f68-a7ad-7d578c089f8b"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1383), new DateOnly(2025, 2, 9), null, 1, new Guid("0855d926-ebe2-4b26-873a-ca9629308615"), 1, 1, null },
                    { new Guid("cc82920a-ae3b-4a58-b16f-f5af1f73a9e7"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1447), new DateOnly(2025, 2, 6), null, 1, new Guid("10abe3db-678a-4e25-8baa-0db70840a71e"), 3, 1, null },
                    { new Guid("ccc910aa-32d3-441c-a29f-05855bbcf6b6"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1507), new DateOnly(2025, 2, 3), null, 1, new Guid("c38290c7-a3b7-4588-9508-147e07b86bda"), 0, 1, null },
                    { new Guid("d011b65e-f51b-424f-a579-c72fee84c12b"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1409), new DateOnly(2025, 2, 21), null, 1, new Guid("ca793127-ebb7-48e9-a1f1-4a85514232f9"), 3, 1, null },
                    { new Guid("d0b511fc-4a44-48a1-8a6d-db0c9f96aeeb"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1565), new DateOnly(2025, 2, 18), null, 1, new Guid("1bdf901f-6242-4e8b-9bf8-aeddedab2816"), 0, 1, null },
                    { new Guid("d143bfa9-575c-4d80-8b39-e43972325e09"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1491), new DateOnly(2025, 2, 11), null, 1, new Guid("1ac38e12-c060-4a9e-80c7-aff5417fa5c8"), 3, 1, null },
                    { new Guid("d188b497-0aca-48c5-9d09-004575c66af4"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1513), new DateOnly(2025, 2, 7), null, 1, new Guid("fdee01f3-16c2-4fe9-ac6e-230a86fed218"), 4, 1, null },
                    { new Guid("d2e25264-a384-4383-8d15-2d97323a0ee3"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1422), new DateOnly(2025, 2, 9), null, 1, new Guid("0855d926-ebe2-4b26-873a-ca9629308615"), 1, 1, null },
                    { new Guid("d75d29d4-6fd8-4be5-8554-07153f4de0e8"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1445), new DateOnly(2025, 2, 4), null, 1, new Guid("9a8881eb-8665-4ca4-88ac-695067744b89"), 1, 1, null },
                    { new Guid("d88f257b-f579-46f8-8a8a-181addb8a338"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1480), new DateOnly(2025, 2, 4), null, 1, new Guid("9a8881eb-8665-4ca4-88ac-695067744b89"), 1, 1, null },
                    { new Guid("d902a254-bdc4-42b5-8127-72b196b3e585"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1427), new DateOnly(2025, 2, 13), null, 1, new Guid("f966ac34-6c22-4b93-b5e4-314c93cf3dfc"), 0, 1, null },
                    { new Guid("da411a92-ed81-4853-937f-f2484df044ed"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1451), new DateOnly(2025, 2, 9), null, 1, new Guid("0855d926-ebe2-4b26-873a-ca9629308615"), 1, 1, null },
                    { new Guid("daa57055-5788-41d6-a9d6-c765b55f974b"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1540), new DateOnly(2025, 2, 20), null, 1, new Guid("c2faa8f7-27f8-4945-a8f8-c4bde3ec98f0"), 2, 1, null },
                    { new Guid("dafeacb2-ae96-47d8-9c08-ed79d6b31b05"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1547), new DateOnly(2025, 2, 4), null, 1, new Guid("9a8881eb-8665-4ca4-88ac-695067744b89"), 1, 1, null },
                    { new Guid("dafec166-b3c2-4241-b264-fba119ede85c"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1551), new DateOnly(2025, 2, 7), null, 1, new Guid("fdee01f3-16c2-4fe9-ac6e-230a86fed218"), 4, 1, null },
                    { new Guid("db81778d-7e8a-4da3-9ce6-78fd3d261ff1"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1464), new DateOnly(2025, 2, 19), null, 1, new Guid("53cfb467-d82f-4f52-8a0c-7e95e169d441"), 1, 1, null },
                    { new Guid("dc1246db-98cc-41cd-915a-93adc3bf49fd"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1423), new DateOnly(2025, 2, 10), null, 1, new Guid("e2cb1e7c-8d40-4378-917f-9da722f29bd4"), 2, 1, null },
                    { new Guid("de84f2a8-9446-4cf5-b61c-195cefb1c655"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1526), new DateOnly(2025, 2, 17), null, 1, new Guid("3ee45d1e-a3f8-4e00-8fe1-add0dc2a4f56"), 4, 1, null },
                    { new Guid("e1397403-dfe9-44ed-9777-c653de54fb73"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1390), new DateOnly(2025, 2, 14), null, 1, new Guid("7b4bcd62-2fb3-41c6-bb1e-f672a6ffe4b6"), 1, 1, null },
                    { new Guid("e2d8be78-06fa-4fcc-9337-a7161805ec41"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1428), new DateOnly(2025, 2, 14), null, 1, new Guid("7b4bcd62-2fb3-41c6-bb1e-f672a6ffe4b6"), 1, 1, null },
                    { new Guid("e3f24681-c4be-48e0-aee5-b182d685eb82"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1384), new DateOnly(2025, 2, 10), null, 1, new Guid("e2cb1e7c-8d40-4378-917f-9da722f29bd4"), 2, 1, null },
                    { new Guid("e3f7bc6d-6d2f-4b0c-b6a2-0587f14e6a9f"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1503), new DateOnly(2025, 2, 20), null, 1, new Guid("c2faa8f7-27f8-4945-a8f8-c4bde3ec98f0"), 2, 1, null },
                    { new Guid("e53bcecc-8e4f-4d89-8421-d007352456c7"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1426), new DateOnly(2025, 2, 12), null, 1, new Guid("81d84b3b-9191-437f-b2e8-1fa832fda1ad"), 4, 1, null },
                    { new Guid("e7c1e5b9-7038-49bc-84ba-4ad0892be078"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1571), new DateOnly(2025, 2, 22), null, 1, new Guid("a5873185-9299-4227-93d9-467871515725"), 4, 1, null },
                    { new Guid("e8a2a9e8-6c2b-490d-91ec-f796a50615f8"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1411), new DateOnly(2025, 2, 22), null, 1, new Guid("a5873185-9299-4227-93d9-467871515725"), 4, 1, null },
                    { new Guid("e916b054-c7c8-4fcb-b482-f0f81196f947"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1484), new DateOnly(2025, 2, 7), null, 1, new Guid("fdee01f3-16c2-4fe9-ac6e-230a86fed218"), 4, 1, null },
                    { new Guid("e97936ed-5bce-4f87-9601-eb125b6af1a5"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1388), new DateOnly(2025, 2, 12), null, 1, new Guid("81d84b3b-9191-437f-b2e8-1fa832fda1ad"), 4, 1, null },
                    { new Guid("eda90344-680d-442b-903d-2d210f59e110"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1450), new DateOnly(2025, 2, 8), null, 1, new Guid("3d60ed18-8f46-412d-a8cf-5fb6a09b4f6f"), 0, 1, null },
                    { new Guid("edb2f1f6-b93d-49bb-8fb1-04d9796238cb"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1385), new DateOnly(2025, 2, 11), null, 1, new Guid("1ac38e12-c060-4a9e-80c7-aff5417fa5c8"), 3, 1, null },
                    { new Guid("ee3261b3-18e4-48ca-80cd-5f6e8fb49195"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1493), new DateOnly(2025, 2, 13), null, 1, new Guid("f966ac34-6c22-4b93-b5e4-314c93cf3dfc"), 0, 1, null },
                    { new Guid("f129d533-7c12-4779-a40b-9614ddb98eaa"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1375), new DateOnly(2025, 2, 5), null, 1, new Guid("6f3ca661-e46c-4bce-8c93-88132a8bd591"), 2, 1, null },
                    { new Guid("f5d2e9b4-3b91-4761-b695-02a2d39275b5"), new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1545), new DateOnly(2025, 2, 3), null, 1, new Guid("c38290c7-a3b7-4588-9508-147e07b86bda"), 0, 1, null },
                    { new Guid("f68aed0b-a60e-4c64-882a-16e429d5927b"), new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1394), new DateOnly(2025, 2, 17), null, 1, new Guid("3ee45d1e-a3f8-4e00-8fe1-add0dc2a4f56"), 4, 1, null },
                    { new Guid("f81437d4-be69-4218-b195-d6eb07835a80"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1435), new DateOnly(2025, 2, 18), null, 1, new Guid("1bdf901f-6242-4e8b-9bf8-aeddedab2816"), 0, 1, null },
                    { new Guid("fa9db598-0c8c-4027-a24d-84b66cb0c584"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1449), new DateOnly(2025, 2, 7), null, 1, new Guid("fdee01f3-16c2-4fe9-ac6e-230a86fed218"), 4, 1, null },
                    { new Guid("fb256ab8-108b-405a-bf0b-79a87c95567b"), new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1413), new DateOnly(2025, 2, 3), null, 1, new Guid("c38290c7-a3b7-4588-9508-147e07b86bda"), 0, 1, null },
                    { new Guid("fbac33ea-3bad-4271-9112-893420674318"), new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1512), new DateOnly(2025, 2, 6), null, 1, new Guid("10abe3db-678a-4e25-8baa-0db70840a71e"), 3, 1, null },
                    { new Guid("fd1ae84e-81bf-436c-9659-17e28251e71a"), new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1458), new DateOnly(2025, 2, 14), null, 1, new Guid("7b4bcd62-2fb3-41c6-bb1e-f672a6ffe4b6"), 1, 1, null },
                    { new Guid("fd910afd-b100-4fd9-822a-36b0d1c9f731"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1488), new DateOnly(2025, 2, 9), null, 1, new Guid("0855d926-ebe2-4b26-873a-ca9629308615"), 1, 1, null },
                    { new Guid("fe0f508b-7377-44ed-be42-a70a95468050"), new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(1497), new DateOnly(2025, 2, 16), null, 1, new Guid("9a52dbd7-8ffc-4107-af9a-c353274c8daa"), 3, 1, null }
                });

            migrationBuilder.InsertData(
                table: "StudentClass",
                columns: new[] { "Id", "CertificateUrl", "ClassId", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "GPA", "InstructorComment", "IsPassed", "RecordStatus", "StudentFirebaseId", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00bbb88d-6b49-4bf2-9b3e-9716ef592b9a"), null, new Guid("a40d7075-e6af-4d5a-b6e9-032776decbdd"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(9653), "admin001", null, null, null, null, false, 1, "learner003", null, null },
                    { new Guid("1dd8130e-1f69-4ba2-bc12-d5437c71ccfa"), null, new Guid("2a31f9fa-b927-4b52-95ac-e534ddb229a6"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(9695), "admin001", null, null, null, null, false, 1, "learner007", null, null },
                    { new Guid("267d89ac-c40b-4da7-a19b-d4f7da889b12"), null, new Guid("0f7ac943-db86-4262-b532-0f2ba5a5632c"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(9649), "admin001", null, null, null, null, false, 1, "learner002", null, null },
                    { new Guid("807692f2-831f-4203-b22d-d9dad2a3dc53"), null, new Guid("51f1cf5f-0d80-4c1d-85a2-0afdab7300c3"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(9698), "admin001", null, null, null, null, false, 1, "learner008", null, null },
                    { new Guid("84e348ca-45a6-4ac3-972b-28d6482d9b0b"), null, new Guid("00334d0c-0e4e-4b96-97f6-22e3eb60e642"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(9700), "admin001", null, null, null, null, false, 1, "learner009", null, null },
                    { new Guid("91569967-de2e-4c2d-a413-6b41752c1ca2"), null, new Guid("f235ac19-0aa3-44bd-a7b5-fdd26ecd316a"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(9657), "admin001", null, null, null, null, false, 1, "learner005", null, null },
                    { new Guid("98438701-8dce-4146-a559-c6e5c74a4aca"), null, new Guid("35dda968-db4f-4983-9317-b76edeb115f5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(9665), "admin001", null, null, null, null, false, 1, "learner006", null, null },
                    { new Guid("affcfecd-570b-4749-9368-5f7eb1bce5ac"), null, new Guid("9a2dc5dd-ff82-46c7-856f-4a28460a71a8"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(8018), "admin001", null, null, null, null, false, 1, "learner001", null, null },
                    { new Guid("c775437e-061a-4d86-9232-51161f746638"), null, new Guid("f2c4e762-9d85-41b2-845b-6166226d8dc5"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(9655), "admin001", null, null, null, null, false, 1, "learner004", null, null },
                    { new Guid("e3e07ec9-fbe9-4ef2-94b8-60711583f12a"), null, new Guid("8af29ceb-c2f3-4357-9aab-936b754e4880"), new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(9703), "admin001", null, null, null, null, false, 1, "learner010", null, null }
                });

            migrationBuilder.InsertData(
                table: "EntranceTestStudent",
                columns: new[] { "Id", "BandScore", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "EntranceTestId", "InstructorComment", "IsScoreAnnounced", "Rank", "RecordStatus", "StudentFirebaseId", "UpdateById", "UpdatedAt", "Year" },
                values: new object[,]
                {
                    { new Guid("281f138c-896c-4317-b87c-55565951e6d5"), 9m, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(6909), "admin001", null, null, new Guid("3d99606e-92fb-441a-bb8a-c8d2c5fc5ff2"), null, true, 3, 1, "learner001", null, null, 2024 },
                    { new Guid("363853bc-8134-4ff0-b017-7b920d916555"), 6m, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(4377), "admin001", null, null, new Guid("3d99606e-92fb-441a-bb8a-c8d2c5fc5ff2"), null, true, 1, 1, "learner001", null, null, 2024 },
                    { new Guid("4230b196-947e-4cb7-8b07-a5841b847503"), 5m, new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(6897), "admin001", null, null, new Guid("3d99606e-92fb-441a-bb8a-c8d2c5fc5ff2"), null, true, 2, 1, "learner001", null, null, 2024 }
                });

            migrationBuilder.InsertData(
                table: "SlotStudent",
                columns: new[] { "SlotId", "StudentFirebaseId", "AttendanceStatus", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "RecordStatus", "UpdateById", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("011ae32f-8574-4f78-98d0-96a19fc484cb"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4713), "admin001", null, null, 1, null, null },
                    { new Guid("011ae32f-8574-4f78-98d0-96a19fc484cb"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4713), "admin001", null, null, 1, null, null },
                    { new Guid("011ae32f-8574-4f78-98d0-96a19fc484cb"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4714), "admin001", null, null, 1, null, null },
                    { new Guid("011ae32f-8574-4f78-98d0-96a19fc484cb"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4715), "admin001", null, null, 1, null, null },
                    { new Guid("011ae32f-8574-4f78-98d0-96a19fc484cb"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4715), "admin001", null, null, 1, null, null },
                    { new Guid("011ae32f-8574-4f78-98d0-96a19fc484cb"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4716), "admin001", null, null, 1, null, null },
                    { new Guid("011ae32f-8574-4f78-98d0-96a19fc484cb"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4717), "admin001", null, null, 1, null, null },
                    { new Guid("011ae32f-8574-4f78-98d0-96a19fc484cb"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4717), "admin001", null, null, 1, null, null },
                    { new Guid("01264359-3a2d-450c-822d-6c14e4ca37d3"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5008), "admin001", null, null, 1, null, null },
                    { new Guid("01264359-3a2d-450c-822d-6c14e4ca37d3"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5009), "admin001", null, null, 1, null, null },
                    { new Guid("01264359-3a2d-450c-822d-6c14e4ca37d3"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5009), "admin001", null, null, 1, null, null },
                    { new Guid("01264359-3a2d-450c-822d-6c14e4ca37d3"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5010), "admin001", null, null, 1, null, null },
                    { new Guid("01264359-3a2d-450c-822d-6c14e4ca37d3"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5011), "admin001", null, null, 1, null, null },
                    { new Guid("01264359-3a2d-450c-822d-6c14e4ca37d3"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5011), "admin001", null, null, 1, null, null },
                    { new Guid("01264359-3a2d-450c-822d-6c14e4ca37d3"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5012), "admin001", null, null, 1, null, null },
                    { new Guid("01264359-3a2d-450c-822d-6c14e4ca37d3"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5012), "admin001", null, null, 1, null, null },
                    { new Guid("01b06b42-183c-46dc-9647-252cbd4ae82d"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4684), "admin001", null, null, 1, null, null },
                    { new Guid("01b06b42-183c-46dc-9647-252cbd4ae82d"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4685), "admin001", null, null, 1, null, null },
                    { new Guid("01b06b42-183c-46dc-9647-252cbd4ae82d"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4685), "admin001", null, null, 1, null, null },
                    { new Guid("01b06b42-183c-46dc-9647-252cbd4ae82d"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4686), "admin001", null, null, 1, null, null },
                    { new Guid("01b06b42-183c-46dc-9647-252cbd4ae82d"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4687), "admin001", null, null, 1, null, null },
                    { new Guid("01b06b42-183c-46dc-9647-252cbd4ae82d"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4687), "admin001", null, null, 1, null, null },
                    { new Guid("01b06b42-183c-46dc-9647-252cbd4ae82d"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4695), "admin001", null, null, 1, null, null },
                    { new Guid("01b06b42-183c-46dc-9647-252cbd4ae82d"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4695), "admin001", null, null, 1, null, null },
                    { new Guid("039fc807-20f1-48ae-beb3-b8145bbb45f6"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4931), "admin001", null, null, 1, null, null },
                    { new Guid("039fc807-20f1-48ae-beb3-b8145bbb45f6"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4932), "admin001", null, null, 1, null, null },
                    { new Guid("039fc807-20f1-48ae-beb3-b8145bbb45f6"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4933), "admin001", null, null, 1, null, null },
                    { new Guid("039fc807-20f1-48ae-beb3-b8145bbb45f6"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4933), "admin001", null, null, 1, null, null },
                    { new Guid("039fc807-20f1-48ae-beb3-b8145bbb45f6"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4934), "admin001", null, null, 1, null, null },
                    { new Guid("039fc807-20f1-48ae-beb3-b8145bbb45f6"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4934), "admin001", null, null, 1, null, null },
                    { new Guid("039fc807-20f1-48ae-beb3-b8145bbb45f6"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4935), "admin001", null, null, 1, null, null },
                    { new Guid("039fc807-20f1-48ae-beb3-b8145bbb45f6"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4936), "admin001", null, null, 1, null, null },
                    { new Guid("0d13ab1c-a99b-4a9b-9337-74987e881ff4"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4816), "admin001", null, null, 1, null, null },
                    { new Guid("0d13ab1c-a99b-4a9b-9337-74987e881ff4"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4817), "admin001", null, null, 1, null, null },
                    { new Guid("0d13ab1c-a99b-4a9b-9337-74987e881ff4"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4818), "admin001", null, null, 1, null, null },
                    { new Guid("0d13ab1c-a99b-4a9b-9337-74987e881ff4"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4818), "admin001", null, null, 1, null, null },
                    { new Guid("0d13ab1c-a99b-4a9b-9337-74987e881ff4"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4819), "admin001", null, null, 1, null, null },
                    { new Guid("0d13ab1c-a99b-4a9b-9337-74987e881ff4"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4820), "admin001", null, null, 1, null, null },
                    { new Guid("0d13ab1c-a99b-4a9b-9337-74987e881ff4"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4820), "admin001", null, null, 1, null, null },
                    { new Guid("0d13ab1c-a99b-4a9b-9337-74987e881ff4"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4821), "admin001", null, null, 1, null, null },
                    { new Guid("0f1fd977-a5cb-483b-baed-1be5ea2f83fd"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4315), "admin001", null, null, 1, null, null },
                    { new Guid("0f1fd977-a5cb-483b-baed-1be5ea2f83fd"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4316), "admin001", null, null, 1, null, null },
                    { new Guid("0f1fd977-a5cb-483b-baed-1be5ea2f83fd"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4316), "admin001", null, null, 1, null, null },
                    { new Guid("0f1fd977-a5cb-483b-baed-1be5ea2f83fd"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4317), "admin001", null, null, 1, null, null },
                    { new Guid("0f1fd977-a5cb-483b-baed-1be5ea2f83fd"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4318), "admin001", null, null, 1, null, null },
                    { new Guid("0f1fd977-a5cb-483b-baed-1be5ea2f83fd"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4318), "admin001", null, null, 1, null, null },
                    { new Guid("0f1fd977-a5cb-483b-baed-1be5ea2f83fd"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4319), "admin001", null, null, 1, null, null },
                    { new Guid("0f1fd977-a5cb-483b-baed-1be5ea2f83fd"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4320), "admin001", null, null, 1, null, null },
                    { new Guid("11026381-2cf1-4560-a48c-c5477dca1744"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4916), "admin001", null, null, 1, null, null },
                    { new Guid("11026381-2cf1-4560-a48c-c5477dca1744"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4917), "admin001", null, null, 1, null, null },
                    { new Guid("11026381-2cf1-4560-a48c-c5477dca1744"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4917), "admin001", null, null, 1, null, null },
                    { new Guid("11026381-2cf1-4560-a48c-c5477dca1744"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4918), "admin001", null, null, 1, null, null },
                    { new Guid("11026381-2cf1-4560-a48c-c5477dca1744"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4918), "admin001", null, null, 1, null, null },
                    { new Guid("11026381-2cf1-4560-a48c-c5477dca1744"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4919), "admin001", null, null, 1, null, null },
                    { new Guid("11026381-2cf1-4560-a48c-c5477dca1744"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4920), "admin001", null, null, 1, null, null },
                    { new Guid("11026381-2cf1-4560-a48c-c5477dca1744"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4920), "admin001", null, null, 1, null, null },
                    { new Guid("11c8d104-8cad-467c-90ab-9ee343af32aa"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4474), "admin001", null, null, 1, null, null },
                    { new Guid("11c8d104-8cad-467c-90ab-9ee343af32aa"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4475), "admin001", null, null, 1, null, null },
                    { new Guid("11c8d104-8cad-467c-90ab-9ee343af32aa"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4476), "admin001", null, null, 1, null, null },
                    { new Guid("11c8d104-8cad-467c-90ab-9ee343af32aa"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4476), "admin001", null, null, 1, null, null },
                    { new Guid("11c8d104-8cad-467c-90ab-9ee343af32aa"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4477), "admin001", null, null, 1, null, null },
                    { new Guid("11c8d104-8cad-467c-90ab-9ee343af32aa"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4477), "admin001", null, null, 1, null, null },
                    { new Guid("11c8d104-8cad-467c-90ab-9ee343af32aa"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4478), "admin001", null, null, 1, null, null },
                    { new Guid("11c8d104-8cad-467c-90ab-9ee343af32aa"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4479), "admin001", null, null, 1, null, null },
                    { new Guid("145e426c-4a4f-45ee-bc96-0c4d65c150c4"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5031), "admin001", null, null, 1, null, null },
                    { new Guid("145e426c-4a4f-45ee-bc96-0c4d65c150c4"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5032), "admin001", null, null, 1, null, null },
                    { new Guid("145e426c-4a4f-45ee-bc96-0c4d65c150c4"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5032), "admin001", null, null, 1, null, null },
                    { new Guid("145e426c-4a4f-45ee-bc96-0c4d65c150c4"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5033), "admin001", null, null, 1, null, null },
                    { new Guid("145e426c-4a4f-45ee-bc96-0c4d65c150c4"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5033), "admin001", null, null, 1, null, null },
                    { new Guid("145e426c-4a4f-45ee-bc96-0c4d65c150c4"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5034), "admin001", null, null, 1, null, null },
                    { new Guid("145e426c-4a4f-45ee-bc96-0c4d65c150c4"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5035), "admin001", null, null, 1, null, null },
                    { new Guid("145e426c-4a4f-45ee-bc96-0c4d65c150c4"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5035), "admin001", null, null, 1, null, null },
                    { new Guid("154542d1-1d70-4679-847a-6a0cfed78fa1"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4436), "admin001", null, null, 1, null, null },
                    { new Guid("154542d1-1d70-4679-847a-6a0cfed78fa1"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4436), "admin001", null, null, 1, null, null },
                    { new Guid("154542d1-1d70-4679-847a-6a0cfed78fa1"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4437), "admin001", null, null, 1, null, null },
                    { new Guid("154542d1-1d70-4679-847a-6a0cfed78fa1"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4438), "admin001", null, null, 1, null, null },
                    { new Guid("154542d1-1d70-4679-847a-6a0cfed78fa1"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4438), "admin001", null, null, 1, null, null },
                    { new Guid("154542d1-1d70-4679-847a-6a0cfed78fa1"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4439), "admin001", null, null, 1, null, null },
                    { new Guid("154542d1-1d70-4679-847a-6a0cfed78fa1"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4440), "admin001", null, null, 1, null, null },
                    { new Guid("154542d1-1d70-4679-847a-6a0cfed78fa1"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4440), "admin001", null, null, 1, null, null },
                    { new Guid("1ade82a7-8916-48fb-add9-46029b987661"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4425), "admin001", null, null, 1, null, null },
                    { new Guid("1ade82a7-8916-48fb-add9-46029b987661"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4426), "admin001", null, null, 1, null, null },
                    { new Guid("1ade82a7-8916-48fb-add9-46029b987661"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4427), "admin001", null, null, 1, null, null },
                    { new Guid("1ade82a7-8916-48fb-add9-46029b987661"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4427), "admin001", null, null, 1, null, null },
                    { new Guid("1ade82a7-8916-48fb-add9-46029b987661"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4428), "admin001", null, null, 1, null, null },
                    { new Guid("1ade82a7-8916-48fb-add9-46029b987661"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4429), "admin001", null, null, 1, null, null },
                    { new Guid("1ade82a7-8916-48fb-add9-46029b987661"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4429), "admin001", null, null, 1, null, null },
                    { new Guid("1ade82a7-8916-48fb-add9-46029b987661"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4430), "admin001", null, null, 1, null, null },
                    { new Guid("2260d31c-075b-48e9-aedb-8ac9d1cf32d0"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4829), "admin001", null, null, 1, null, null },
                    { new Guid("2260d31c-075b-48e9-aedb-8ac9d1cf32d0"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4829), "admin001", null, null, 1, null, null },
                    { new Guid("2260d31c-075b-48e9-aedb-8ac9d1cf32d0"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4830), "admin001", null, null, 1, null, null },
                    { new Guid("2260d31c-075b-48e9-aedb-8ac9d1cf32d0"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4830), "admin001", null, null, 1, null, null },
                    { new Guid("2260d31c-075b-48e9-aedb-8ac9d1cf32d0"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4831), "admin001", null, null, 1, null, null },
                    { new Guid("2260d31c-075b-48e9-aedb-8ac9d1cf32d0"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4832), "admin001", null, null, 1, null, null },
                    { new Guid("2260d31c-075b-48e9-aedb-8ac9d1cf32d0"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4832), "admin001", null, null, 1, null, null },
                    { new Guid("2260d31c-075b-48e9-aedb-8ac9d1cf32d0"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4833), "admin001", null, null, 1, null, null },
                    { new Guid("22edf58f-bf3f-4b04-a2f0-56304e8dae80"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4309), "admin001", null, null, 1, null, null },
                    { new Guid("22edf58f-bf3f-4b04-a2f0-56304e8dae80"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4310), "admin001", null, null, 1, null, null },
                    { new Guid("22edf58f-bf3f-4b04-a2f0-56304e8dae80"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4311), "admin001", null, null, 1, null, null },
                    { new Guid("22edf58f-bf3f-4b04-a2f0-56304e8dae80"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4312), "admin001", null, null, 1, null, null },
                    { new Guid("22edf58f-bf3f-4b04-a2f0-56304e8dae80"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4312), "admin001", null, null, 1, null, null },
                    { new Guid("22edf58f-bf3f-4b04-a2f0-56304e8dae80"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4313), "admin001", null, null, 1, null, null },
                    { new Guid("22edf58f-bf3f-4b04-a2f0-56304e8dae80"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4314), "admin001", null, null, 1, null, null },
                    { new Guid("22edf58f-bf3f-4b04-a2f0-56304e8dae80"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4314), "admin001", null, null, 1, null, null },
                    { new Guid("27641c12-fc9c-491c-8355-f292df006482"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4549), "admin001", null, null, 1, null, null },
                    { new Guid("27641c12-fc9c-491c-8355-f292df006482"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4549), "admin001", null, null, 1, null, null },
                    { new Guid("27641c12-fc9c-491c-8355-f292df006482"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4550), "admin001", null, null, 1, null, null },
                    { new Guid("27641c12-fc9c-491c-8355-f292df006482"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4551), "admin001", null, null, 1, null, null },
                    { new Guid("27641c12-fc9c-491c-8355-f292df006482"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4551), "admin001", null, null, 1, null, null },
                    { new Guid("27641c12-fc9c-491c-8355-f292df006482"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4552), "admin001", null, null, 1, null, null },
                    { new Guid("27641c12-fc9c-491c-8355-f292df006482"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4552), "admin001", null, null, 1, null, null },
                    { new Guid("27641c12-fc9c-491c-8355-f292df006482"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4553), "admin001", null, null, 1, null, null },
                    { new Guid("2820b31f-f228-4c48-9ff8-0c5f7e75f0e1"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4778), "admin001", null, null, 1, null, null },
                    { new Guid("2820b31f-f228-4c48-9ff8-0c5f7e75f0e1"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4779), "admin001", null, null, 1, null, null },
                    { new Guid("2820b31f-f228-4c48-9ff8-0c5f7e75f0e1"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4780), "admin001", null, null, 1, null, null },
                    { new Guid("2820b31f-f228-4c48-9ff8-0c5f7e75f0e1"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4780), "admin001", null, null, 1, null, null },
                    { new Guid("2820b31f-f228-4c48-9ff8-0c5f7e75f0e1"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4788), "admin001", null, null, 1, null, null },
                    { new Guid("2820b31f-f228-4c48-9ff8-0c5f7e75f0e1"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4789), "admin001", null, null, 1, null, null },
                    { new Guid("2820b31f-f228-4c48-9ff8-0c5f7e75f0e1"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4789), "admin001", null, null, 1, null, null },
                    { new Guid("2820b31f-f228-4c48-9ff8-0c5f7e75f0e1"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4790), "admin001", null, null, 1, null, null },
                    { new Guid("2f731439-0c60-410a-add5-a84373cf0cdd"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4975), "admin001", null, null, 1, null, null },
                    { new Guid("2f731439-0c60-410a-add5-a84373cf0cdd"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4976), "admin001", null, null, 1, null, null },
                    { new Guid("2f731439-0c60-410a-add5-a84373cf0cdd"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4977), "admin001", null, null, 1, null, null },
                    { new Guid("2f731439-0c60-410a-add5-a84373cf0cdd"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4977), "admin001", null, null, 1, null, null },
                    { new Guid("2f731439-0c60-410a-add5-a84373cf0cdd"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4978), "admin001", null, null, 1, null, null },
                    { new Guid("2f731439-0c60-410a-add5-a84373cf0cdd"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4979), "admin001", null, null, 1, null, null },
                    { new Guid("2f731439-0c60-410a-add5-a84373cf0cdd"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4979), "admin001", null, null, 1, null, null },
                    { new Guid("2f731439-0c60-410a-add5-a84373cf0cdd"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4980), "admin001", null, null, 1, null, null },
                    { new Guid("31cf6296-8eb1-409e-8126-803a00c9be72"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4617), "admin001", null, null, 1, null, null },
                    { new Guid("31cf6296-8eb1-409e-8126-803a00c9be72"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4618), "admin001", null, null, 1, null, null },
                    { new Guid("31cf6296-8eb1-409e-8126-803a00c9be72"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4619), "admin001", null, null, 1, null, null },
                    { new Guid("31cf6296-8eb1-409e-8126-803a00c9be72"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4619), "admin001", null, null, 1, null, null },
                    { new Guid("31cf6296-8eb1-409e-8126-803a00c9be72"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4620), "admin001", null, null, 1, null, null },
                    { new Guid("31cf6296-8eb1-409e-8126-803a00c9be72"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4620), "admin001", null, null, 1, null, null },
                    { new Guid("31cf6296-8eb1-409e-8126-803a00c9be72"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4621), "admin001", null, null, 1, null, null },
                    { new Guid("31cf6296-8eb1-409e-8126-803a00c9be72"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4622), "admin001", null, null, 1, null, null },
                    { new Guid("348bdcee-c6bc-4de3-bb5a-ef3bd1c4a767"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4846), "admin001", null, null, 1, null, null },
                    { new Guid("348bdcee-c6bc-4de3-bb5a-ef3bd1c4a767"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4846), "admin001", null, null, 1, null, null },
                    { new Guid("348bdcee-c6bc-4de3-bb5a-ef3bd1c4a767"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4847), "admin001", null, null, 1, null, null },
                    { new Guid("348bdcee-c6bc-4de3-bb5a-ef3bd1c4a767"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4847), "admin001", null, null, 1, null, null },
                    { new Guid("348bdcee-c6bc-4de3-bb5a-ef3bd1c4a767"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4848), "admin001", null, null, 1, null, null },
                    { new Guid("348bdcee-c6bc-4de3-bb5a-ef3bd1c4a767"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4849), "admin001", null, null, 1, null, null },
                    { new Guid("348bdcee-c6bc-4de3-bb5a-ef3bd1c4a767"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4849), "admin001", null, null, 1, null, null },
                    { new Guid("348bdcee-c6bc-4de3-bb5a-ef3bd1c4a767"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4850), "admin001", null, null, 1, null, null },
                    { new Guid("36903caa-4406-43e6-b456-874049ca9c27"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4679), "admin001", null, null, 1, null, null },
                    { new Guid("36903caa-4406-43e6-b456-874049ca9c27"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4680), "admin001", null, null, 1, null, null },
                    { new Guid("36903caa-4406-43e6-b456-874049ca9c27"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4680), "admin001", null, null, 1, null, null },
                    { new Guid("36903caa-4406-43e6-b456-874049ca9c27"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4681), "admin001", null, null, 1, null, null },
                    { new Guid("36903caa-4406-43e6-b456-874049ca9c27"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4681), "admin001", null, null, 1, null, null },
                    { new Guid("36903caa-4406-43e6-b456-874049ca9c27"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4682), "admin001", null, null, 1, null, null },
                    { new Guid("36903caa-4406-43e6-b456-874049ca9c27"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4683), "admin001", null, null, 1, null, null },
                    { new Guid("36903caa-4406-43e6-b456-874049ca9c27"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4683), "admin001", null, null, 1, null, null },
                    { new Guid("36fa002d-ddef-44c2-952d-f8a8dc92cb13"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4998), "admin001", null, null, 1, null, null },
                    { new Guid("36fa002d-ddef-44c2-952d-f8a8dc92cb13"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4998), "admin001", null, null, 1, null, null },
                    { new Guid("36fa002d-ddef-44c2-952d-f8a8dc92cb13"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4999), "admin001", null, null, 1, null, null },
                    { new Guid("36fa002d-ddef-44c2-952d-f8a8dc92cb13"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5000), "admin001", null, null, 1, null, null },
                    { new Guid("36fa002d-ddef-44c2-952d-f8a8dc92cb13"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5000), "admin001", null, null, 1, null, null },
                    { new Guid("36fa002d-ddef-44c2-952d-f8a8dc92cb13"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5001), "admin001", null, null, 1, null, null },
                    { new Guid("36fa002d-ddef-44c2-952d-f8a8dc92cb13"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5001), "admin001", null, null, 1, null, null },
                    { new Guid("36fa002d-ddef-44c2-952d-f8a8dc92cb13"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5002), "admin001", null, null, 1, null, null },
                    { new Guid("3e2531f0-3d8b-4be7-b3e0-57f075c80d5f"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4898), "admin001", null, null, 1, null, null },
                    { new Guid("3e2531f0-3d8b-4be7-b3e0-57f075c80d5f"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4899), "admin001", null, null, 1, null, null },
                    { new Guid("3e2531f0-3d8b-4be7-b3e0-57f075c80d5f"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4899), "admin001", null, null, 1, null, null },
                    { new Guid("3e2531f0-3d8b-4be7-b3e0-57f075c80d5f"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4900), "admin001", null, null, 1, null, null },
                    { new Guid("3e2531f0-3d8b-4be7-b3e0-57f075c80d5f"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4901), "admin001", null, null, 1, null, null },
                    { new Guid("3e2531f0-3d8b-4be7-b3e0-57f075c80d5f"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4901), "admin001", null, null, 1, null, null },
                    { new Guid("3e2531f0-3d8b-4be7-b3e0-57f075c80d5f"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4909), "admin001", null, null, 1, null, null },
                    { new Guid("3e2531f0-3d8b-4be7-b3e0-57f075c80d5f"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4910), "admin001", null, null, 1, null, null },
                    { new Guid("4acf0f53-e7fc-4120-be15-5ecb266139ed"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4697), "admin001", null, null, 1, null, null },
                    { new Guid("4acf0f53-e7fc-4120-be15-5ecb266139ed"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4698), "admin001", null, null, 1, null, null },
                    { new Guid("4acf0f53-e7fc-4120-be15-5ecb266139ed"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4698), "admin001", null, null, 1, null, null },
                    { new Guid("4acf0f53-e7fc-4120-be15-5ecb266139ed"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4699), "admin001", null, null, 1, null, null },
                    { new Guid("4acf0f53-e7fc-4120-be15-5ecb266139ed"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4700), "admin001", null, null, 1, null, null },
                    { new Guid("4acf0f53-e7fc-4120-be15-5ecb266139ed"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4700), "admin001", null, null, 1, null, null },
                    { new Guid("4acf0f53-e7fc-4120-be15-5ecb266139ed"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4701), "admin001", null, null, 1, null, null },
                    { new Guid("4acf0f53-e7fc-4120-be15-5ecb266139ed"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4702), "admin001", null, null, 1, null, null },
                    { new Guid("4e147947-1d48-4252-8c61-7628ac51418b"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(3634), "admin001", null, null, 1, null, null },
                    { new Guid("4e147947-1d48-4252-8c61-7628ac51418b"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4273), "admin001", null, null, 1, null, null },
                    { new Guid("4e147947-1d48-4252-8c61-7628ac51418b"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4275), "admin001", null, null, 1, null, null },
                    { new Guid("4e147947-1d48-4252-8c61-7628ac51418b"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4275), "admin001", null, null, 1, null, null },
                    { new Guid("4e147947-1d48-4252-8c61-7628ac51418b"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4276), "admin001", null, null, 1, null, null },
                    { new Guid("4e147947-1d48-4252-8c61-7628ac51418b"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4278), "admin001", null, null, 1, null, null },
                    { new Guid("4e147947-1d48-4252-8c61-7628ac51418b"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4279), "admin001", null, null, 1, null, null },
                    { new Guid("4e147947-1d48-4252-8c61-7628ac51418b"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4279), "admin001", null, null, 1, null, null },
                    { new Guid("4ed5ea2e-a3fe-4d35-a23f-b1f26fc34b3e"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4578), "admin001", null, null, 1, null, null },
                    { new Guid("4ed5ea2e-a3fe-4d35-a23f-b1f26fc34b3e"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4579), "admin001", null, null, 1, null, null },
                    { new Guid("4ed5ea2e-a3fe-4d35-a23f-b1f26fc34b3e"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4580), "admin001", null, null, 1, null, null },
                    { new Guid("4ed5ea2e-a3fe-4d35-a23f-b1f26fc34b3e"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4580), "admin001", null, null, 1, null, null },
                    { new Guid("4ed5ea2e-a3fe-4d35-a23f-b1f26fc34b3e"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4581), "admin001", null, null, 1, null, null },
                    { new Guid("4ed5ea2e-a3fe-4d35-a23f-b1f26fc34b3e"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4582), "admin001", null, null, 1, null, null },
                    { new Guid("4ed5ea2e-a3fe-4d35-a23f-b1f26fc34b3e"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4582), "admin001", null, null, 1, null, null },
                    { new Guid("4ed5ea2e-a3fe-4d35-a23f-b1f26fc34b3e"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4583), "admin001", null, null, 1, null, null },
                    { new Guid("4f1444a3-f152-47b1-bc3d-5ea20898483a"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5057), "admin001", null, null, 1, null, null },
                    { new Guid("4f1444a3-f152-47b1-bc3d-5ea20898483a"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5058), "admin001", null, null, 1, null, null },
                    { new Guid("4f1444a3-f152-47b1-bc3d-5ea20898483a"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5058), "admin001", null, null, 1, null, null },
                    { new Guid("4f1444a3-f152-47b1-bc3d-5ea20898483a"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5059), "admin001", null, null, 1, null, null },
                    { new Guid("4f1444a3-f152-47b1-bc3d-5ea20898483a"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5059), "admin001", null, null, 1, null, null },
                    { new Guid("4f1444a3-f152-47b1-bc3d-5ea20898483a"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5060), "admin001", null, null, 1, null, null },
                    { new Guid("4f1444a3-f152-47b1-bc3d-5ea20898483a"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5061), "admin001", null, null, 1, null, null },
                    { new Guid("4f1444a3-f152-47b1-bc3d-5ea20898483a"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5061), "admin001", null, null, 1, null, null },
                    { new Guid("50e4ce08-e62d-4faf-a85f-e0b20e7f7fa6"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4441), "admin001", null, null, 1, null, null },
                    { new Guid("50e4ce08-e62d-4faf-a85f-e0b20e7f7fa6"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4442), "admin001", null, null, 1, null, null },
                    { new Guid("50e4ce08-e62d-4faf-a85f-e0b20e7f7fa6"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4442), "admin001", null, null, 1, null, null },
                    { new Guid("50e4ce08-e62d-4faf-a85f-e0b20e7f7fa6"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4443), "admin001", null, null, 1, null, null },
                    { new Guid("50e4ce08-e62d-4faf-a85f-e0b20e7f7fa6"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4456), "admin001", null, null, 1, null, null },
                    { new Guid("50e4ce08-e62d-4faf-a85f-e0b20e7f7fa6"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4457), "admin001", null, null, 1, null, null },
                    { new Guid("50e4ce08-e62d-4faf-a85f-e0b20e7f7fa6"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4457), "admin001", null, null, 1, null, null },
                    { new Guid("50e4ce08-e62d-4faf-a85f-e0b20e7f7fa6"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4458), "admin001", null, null, 1, null, null },
                    { new Guid("555a4523-171b-49c4-8da0-34873d510cec"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4942), "admin001", null, null, 1, null, null },
                    { new Guid("555a4523-171b-49c4-8da0-34873d510cec"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4949), "admin001", null, null, 1, null, null },
                    { new Guid("555a4523-171b-49c4-8da0-34873d510cec"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4950), "admin001", null, null, 1, null, null },
                    { new Guid("555a4523-171b-49c4-8da0-34873d510cec"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4950), "admin001", null, null, 1, null, null },
                    { new Guid("555a4523-171b-49c4-8da0-34873d510cec"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4951), "admin001", null, null, 1, null, null },
                    { new Guid("555a4523-171b-49c4-8da0-34873d510cec"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4952), "admin001", null, null, 1, null, null },
                    { new Guid("555a4523-171b-49c4-8da0-34873d510cec"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4952), "admin001", null, null, 1, null, null },
                    { new Guid("555a4523-171b-49c4-8da0-34873d510cec"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4953), "admin001", null, null, 1, null, null },
                    { new Guid("58187303-8b99-4e57-b190-2edf770bc193"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4384), "admin001", null, null, 1, null, null },
                    { new Guid("58187303-8b99-4e57-b190-2edf770bc193"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4384), "admin001", null, null, 1, null, null },
                    { new Guid("58187303-8b99-4e57-b190-2edf770bc193"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4385), "admin001", null, null, 1, null, null },
                    { new Guid("58187303-8b99-4e57-b190-2edf770bc193"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4386), "admin001", null, null, 1, null, null },
                    { new Guid("58187303-8b99-4e57-b190-2edf770bc193"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4386), "admin001", null, null, 1, null, null },
                    { new Guid("58187303-8b99-4e57-b190-2edf770bc193"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4387), "admin001", null, null, 1, null, null },
                    { new Guid("58187303-8b99-4e57-b190-2edf770bc193"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4387), "admin001", null, null, 1, null, null },
                    { new Guid("58187303-8b99-4e57-b190-2edf770bc193"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4388), "admin001", null, null, 1, null, null },
                    { new Guid("5a305858-dbbb-4b49-a6b5-7f8c03205a9a"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5036), "admin001", null, null, 1, null, null },
                    { new Guid("5a305858-dbbb-4b49-a6b5-7f8c03205a9a"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5037), "admin001", null, null, 1, null, null },
                    { new Guid("5a305858-dbbb-4b49-a6b5-7f8c03205a9a"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5038), "admin001", null, null, 1, null, null },
                    { new Guid("5a305858-dbbb-4b49-a6b5-7f8c03205a9a"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5038), "admin001", null, null, 1, null, null },
                    { new Guid("5a305858-dbbb-4b49-a6b5-7f8c03205a9a"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5039), "admin001", null, null, 1, null, null },
                    { new Guid("5a305858-dbbb-4b49-a6b5-7f8c03205a9a"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5039), "admin001", null, null, 1, null, null },
                    { new Guid("5a305858-dbbb-4b49-a6b5-7f8c03205a9a"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5040), "admin001", null, null, 1, null, null },
                    { new Guid("5a305858-dbbb-4b49-a6b5-7f8c03205a9a"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5041), "admin001", null, null, 1, null, null },
                    { new Guid("5a873ca8-f57b-400a-8670-9de8b34fa0dc"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4993), "admin001", null, null, 1, null, null },
                    { new Guid("5a873ca8-f57b-400a-8670-9de8b34fa0dc"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4993), "admin001", null, null, 1, null, null },
                    { new Guid("5a873ca8-f57b-400a-8670-9de8b34fa0dc"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4994), "admin001", null, null, 1, null, null },
                    { new Guid("5a873ca8-f57b-400a-8670-9de8b34fa0dc"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4994), "admin001", null, null, 1, null, null },
                    { new Guid("5a873ca8-f57b-400a-8670-9de8b34fa0dc"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4995), "admin001", null, null, 1, null, null },
                    { new Guid("5a873ca8-f57b-400a-8670-9de8b34fa0dc"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4996), "admin001", null, null, 1, null, null },
                    { new Guid("5a873ca8-f57b-400a-8670-9de8b34fa0dc"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4996), "admin001", null, null, 1, null, null },
                    { new Guid("5a873ca8-f57b-400a-8670-9de8b34fa0dc"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4997), "admin001", null, null, 1, null, null },
                    { new Guid("5b4923a9-a6ca-49bc-b385-8cbb4aa8156b"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4280), "admin001", null, null, 1, null, null },
                    { new Guid("5b4923a9-a6ca-49bc-b385-8cbb4aa8156b"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4282), "admin001", null, null, 1, null, null },
                    { new Guid("5b4923a9-a6ca-49bc-b385-8cbb4aa8156b"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4283), "admin001", null, null, 1, null, null },
                    { new Guid("5b4923a9-a6ca-49bc-b385-8cbb4aa8156b"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4283), "admin001", null, null, 1, null, null },
                    { new Guid("5b4923a9-a6ca-49bc-b385-8cbb4aa8156b"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4284), "admin001", null, null, 1, null, null },
                    { new Guid("5b4923a9-a6ca-49bc-b385-8cbb4aa8156b"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4285), "admin001", null, null, 1, null, null },
                    { new Guid("5b4923a9-a6ca-49bc-b385-8cbb4aa8156b"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4285), "admin001", null, null, 1, null, null },
                    { new Guid("5b4923a9-a6ca-49bc-b385-8cbb4aa8156b"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4286), "admin001", null, null, 1, null, null },
                    { new Guid("5cefa8be-2da0-4acf-b91d-bf6e47d640b2"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4459), "admin001", null, null, 1, null, null },
                    { new Guid("5cefa8be-2da0-4acf-b91d-bf6e47d640b2"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4460), "admin001", null, null, 1, null, null },
                    { new Guid("5cefa8be-2da0-4acf-b91d-bf6e47d640b2"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4460), "admin001", null, null, 1, null, null },
                    { new Guid("5cefa8be-2da0-4acf-b91d-bf6e47d640b2"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4461), "admin001", null, null, 1, null, null },
                    { new Guid("5cefa8be-2da0-4acf-b91d-bf6e47d640b2"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4461), "admin001", null, null, 1, null, null },
                    { new Guid("5cefa8be-2da0-4acf-b91d-bf6e47d640b2"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4462), "admin001", null, null, 1, null, null },
                    { new Guid("5cefa8be-2da0-4acf-b91d-bf6e47d640b2"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4463), "admin001", null, null, 1, null, null },
                    { new Guid("5cefa8be-2da0-4acf-b91d-bf6e47d640b2"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4463), "admin001", null, null, 1, null, null },
                    { new Guid("5e068985-1b4e-472a-9e69-dd6188677e04"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4873), "admin001", null, null, 1, null, null },
                    { new Guid("5e068985-1b4e-472a-9e69-dd6188677e04"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4873), "admin001", null, null, 1, null, null },
                    { new Guid("5e068985-1b4e-472a-9e69-dd6188677e04"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4874), "admin001", null, null, 1, null, null },
                    { new Guid("5e068985-1b4e-472a-9e69-dd6188677e04"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4874), "admin001", null, null, 1, null, null },
                    { new Guid("5e068985-1b4e-472a-9e69-dd6188677e04"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4875), "admin001", null, null, 1, null, null },
                    { new Guid("5e068985-1b4e-472a-9e69-dd6188677e04"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4876), "admin001", null, null, 1, null, null },
                    { new Guid("5e068985-1b4e-472a-9e69-dd6188677e04"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4876), "admin001", null, null, 1, null, null },
                    { new Guid("5e068985-1b4e-472a-9e69-dd6188677e04"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4877), "admin001", null, null, 1, null, null },
                    { new Guid("5ef767e1-67cf-42bb-89b5-e55d422439d8"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4349), "admin001", null, null, 1, null, null },
                    { new Guid("5ef767e1-67cf-42bb-89b5-e55d422439d8"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4350), "admin001", null, null, 1, null, null },
                    { new Guid("5ef767e1-67cf-42bb-89b5-e55d422439d8"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4351), "admin001", null, null, 1, null, null },
                    { new Guid("5ef767e1-67cf-42bb-89b5-e55d422439d8"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4351), "admin001", null, null, 1, null, null },
                    { new Guid("5ef767e1-67cf-42bb-89b5-e55d422439d8"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4352), "admin001", null, null, 1, null, null },
                    { new Guid("5ef767e1-67cf-42bb-89b5-e55d422439d8"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4353), "admin001", null, null, 1, null, null },
                    { new Guid("5ef767e1-67cf-42bb-89b5-e55d422439d8"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4353), "admin001", null, null, 1, null, null },
                    { new Guid("5ef767e1-67cf-42bb-89b5-e55d422439d8"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4354), "admin001", null, null, 1, null, null },
                    { new Guid("683b5749-bd3a-4e20-8ed3-22dc15209195"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4628), "admin001", null, null, 1, null, null },
                    { new Guid("683b5749-bd3a-4e20-8ed3-22dc15209195"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4628), "admin001", null, null, 1, null, null },
                    { new Guid("683b5749-bd3a-4e20-8ed3-22dc15209195"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4629), "admin001", null, null, 1, null, null },
                    { new Guid("683b5749-bd3a-4e20-8ed3-22dc15209195"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4629), "admin001", null, null, 1, null, null },
                    { new Guid("683b5749-bd3a-4e20-8ed3-22dc15209195"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4630), "admin001", null, null, 1, null, null },
                    { new Guid("683b5749-bd3a-4e20-8ed3-22dc15209195"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4631), "admin001", null, null, 1, null, null },
                    { new Guid("683b5749-bd3a-4e20-8ed3-22dc15209195"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4631), "admin001", null, null, 1, null, null },
                    { new Guid("683b5749-bd3a-4e20-8ed3-22dc15209195"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4632), "admin001", null, null, 1, null, null },
                    { new Guid("68690226-74a0-419a-aa02-aba402517db4"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4708), "admin001", null, null, 1, null, null },
                    { new Guid("68690226-74a0-419a-aa02-aba402517db4"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4708), "admin001", null, null, 1, null, null },
                    { new Guid("68690226-74a0-419a-aa02-aba402517db4"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4709), "admin001", null, null, 1, null, null },
                    { new Guid("68690226-74a0-419a-aa02-aba402517db4"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4710), "admin001", null, null, 1, null, null },
                    { new Guid("68690226-74a0-419a-aa02-aba402517db4"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4710), "admin001", null, null, 1, null, null },
                    { new Guid("68690226-74a0-419a-aa02-aba402517db4"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4711), "admin001", null, null, 1, null, null },
                    { new Guid("68690226-74a0-419a-aa02-aba402517db4"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4711), "admin001", null, null, 1, null, null },
                    { new Guid("68690226-74a0-419a-aa02-aba402517db4"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4712), "admin001", null, null, 1, null, null },
                    { new Guid("68c6cca4-458f-4cbd-b373-0641d50e709e"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4560), "admin001", null, null, 1, null, null },
                    { new Guid("68c6cca4-458f-4cbd-b373-0641d50e709e"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4561), "admin001", null, null, 1, null, null },
                    { new Guid("68c6cca4-458f-4cbd-b373-0641d50e709e"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4561), "admin001", null, null, 1, null, null },
                    { new Guid("68c6cca4-458f-4cbd-b373-0641d50e709e"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4562), "admin001", null, null, 1, null, null },
                    { new Guid("68c6cca4-458f-4cbd-b373-0641d50e709e"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4570), "admin001", null, null, 1, null, null },
                    { new Guid("68c6cca4-458f-4cbd-b373-0641d50e709e"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4571), "admin001", null, null, 1, null, null },
                    { new Guid("68c6cca4-458f-4cbd-b373-0641d50e709e"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4572), "admin001", null, null, 1, null, null },
                    { new Guid("68c6cca4-458f-4cbd-b373-0641d50e709e"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4572), "admin001", null, null, 1, null, null },
                    { new Guid("6df8ad01-7386-4116-aaae-30a03e142962"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4643), "admin001", null, null, 1, null, null },
                    { new Guid("6df8ad01-7386-4116-aaae-30a03e142962"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4643), "admin001", null, null, 1, null, null },
                    { new Guid("6df8ad01-7386-4116-aaae-30a03e142962"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4644), "admin001", null, null, 1, null, null },
                    { new Guid("6df8ad01-7386-4116-aaae-30a03e142962"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4655), "admin001", null, null, 1, null, null },
                    { new Guid("6df8ad01-7386-4116-aaae-30a03e142962"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4656), "admin001", null, null, 1, null, null },
                    { new Guid("6df8ad01-7386-4116-aaae-30a03e142962"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4656), "admin001", null, null, 1, null, null },
                    { new Guid("6df8ad01-7386-4116-aaae-30a03e142962"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4657), "admin001", null, null, 1, null, null },
                    { new Guid("6df8ad01-7386-4116-aaae-30a03e142962"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4658), "admin001", null, null, 1, null, null },
                    { new Guid("706ed9de-dfc7-45c3-a9ee-638c66603143"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4893), "admin001", null, null, 1, null, null },
                    { new Guid("706ed9de-dfc7-45c3-a9ee-638c66603143"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4894), "admin001", null, null, 1, null, null },
                    { new Guid("706ed9de-dfc7-45c3-a9ee-638c66603143"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4894), "admin001", null, null, 1, null, null },
                    { new Guid("706ed9de-dfc7-45c3-a9ee-638c66603143"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4895), "admin001", null, null, 1, null, null },
                    { new Guid("706ed9de-dfc7-45c3-a9ee-638c66603143"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4895), "admin001", null, null, 1, null, null },
                    { new Guid("706ed9de-dfc7-45c3-a9ee-638c66603143"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4896), "admin001", null, null, 1, null, null },
                    { new Guid("706ed9de-dfc7-45c3-a9ee-638c66603143"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4897), "admin001", null, null, 1, null, null },
                    { new Guid("706ed9de-dfc7-45c3-a9ee-638c66603143"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4897), "admin001", null, null, 1, null, null },
                    { new Guid("7364f791-5f62-4858-a80f-01c96eaf55cc"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4303), "admin001", null, null, 1, null, null },
                    { new Guid("7364f791-5f62-4858-a80f-01c96eaf55cc"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4304), "admin001", null, null, 1, null, null },
                    { new Guid("7364f791-5f62-4858-a80f-01c96eaf55cc"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4305), "admin001", null, null, 1, null, null },
                    { new Guid("7364f791-5f62-4858-a80f-01c96eaf55cc"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4305), "admin001", null, null, 1, null, null },
                    { new Guid("7364f791-5f62-4858-a80f-01c96eaf55cc"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4306), "admin001", null, null, 1, null, null },
                    { new Guid("7364f791-5f62-4858-a80f-01c96eaf55cc"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4307), "admin001", null, null, 1, null, null },
                    { new Guid("7364f791-5f62-4858-a80f-01c96eaf55cc"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4307), "admin001", null, null, 1, null, null },
                    { new Guid("7364f791-5f62-4858-a80f-01c96eaf55cc"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4308), "admin001", null, null, 1, null, null },
                    { new Guid("7690f536-eed6-41ce-9344-f147e48128d1"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5047), "admin001", null, null, 1, null, null },
                    { new Guid("7690f536-eed6-41ce-9344-f147e48128d1"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5047), "admin001", null, null, 1, null, null },
                    { new Guid("7690f536-eed6-41ce-9344-f147e48128d1"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5048), "admin001", null, null, 1, null, null },
                    { new Guid("7690f536-eed6-41ce-9344-f147e48128d1"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5049), "admin001", null, null, 1, null, null },
                    { new Guid("7690f536-eed6-41ce-9344-f147e48128d1"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5049), "admin001", null, null, 1, null, null },
                    { new Guid("7690f536-eed6-41ce-9344-f147e48128d1"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5050), "admin001", null, null, 1, null, null },
                    { new Guid("7690f536-eed6-41ce-9344-f147e48128d1"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5050), "admin001", null, null, 1, null, null },
                    { new Guid("7690f536-eed6-41ce-9344-f147e48128d1"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5051), "admin001", null, null, 1, null, null },
                    { new Guid("77cc660a-2af6-4982-96a4-64a85f68698f"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4538), "admin001", null, null, 1, null, null },
                    { new Guid("77cc660a-2af6-4982-96a4-64a85f68698f"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4539), "admin001", null, null, 1, null, null },
                    { new Guid("77cc660a-2af6-4982-96a4-64a85f68698f"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4540), "admin001", null, null, 1, null, null },
                    { new Guid("77cc660a-2af6-4982-96a4-64a85f68698f"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4540), "admin001", null, null, 1, null, null },
                    { new Guid("77cc660a-2af6-4982-96a4-64a85f68698f"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4541), "admin001", null, null, 1, null, null },
                    { new Guid("77cc660a-2af6-4982-96a4-64a85f68698f"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4541), "admin001", null, null, 1, null, null },
                    { new Guid("77cc660a-2af6-4982-96a4-64a85f68698f"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4542), "admin001", null, null, 1, null, null },
                    { new Guid("77cc660a-2af6-4982-96a4-64a85f68698f"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4543), "admin001", null, null, 1, null, null },
                    { new Guid("79ce53d0-9756-40dd-a4f5-86efc16cd4ed"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4806), "admin001", null, null, 1, null, null },
                    { new Guid("79ce53d0-9756-40dd-a4f5-86efc16cd4ed"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4807), "admin001", null, null, 1, null, null },
                    { new Guid("79ce53d0-9756-40dd-a4f5-86efc16cd4ed"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4807), "admin001", null, null, 1, null, null },
                    { new Guid("79ce53d0-9756-40dd-a4f5-86efc16cd4ed"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4808), "admin001", null, null, 1, null, null },
                    { new Guid("79ce53d0-9756-40dd-a4f5-86efc16cd4ed"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4809), "admin001", null, null, 1, null, null },
                    { new Guid("79ce53d0-9756-40dd-a4f5-86efc16cd4ed"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4809), "admin001", null, null, 1, null, null },
                    { new Guid("79ce53d0-9756-40dd-a4f5-86efc16cd4ed"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4810), "admin001", null, null, 1, null, null },
                    { new Guid("79ce53d0-9756-40dd-a4f5-86efc16cd4ed"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4811), "admin001", null, null, 1, null, null },
                    { new Guid("7b14ad04-7499-4000-a053-3fd7d87ca132"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4554), "admin001", null, null, 1, null, null },
                    { new Guid("7b14ad04-7499-4000-a053-3fd7d87ca132"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4554), "admin001", null, null, 1, null, null },
                    { new Guid("7b14ad04-7499-4000-a053-3fd7d87ca132"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4555), "admin001", null, null, 1, null, null },
                    { new Guid("7b14ad04-7499-4000-a053-3fd7d87ca132"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4556), "admin001", null, null, 1, null, null },
                    { new Guid("7b14ad04-7499-4000-a053-3fd7d87ca132"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4556), "admin001", null, null, 1, null, null },
                    { new Guid("7b14ad04-7499-4000-a053-3fd7d87ca132"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4557), "admin001", null, null, 1, null, null },
                    { new Guid("7b14ad04-7499-4000-a053-3fd7d87ca132"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4558), "admin001", null, null, 1, null, null },
                    { new Guid("7b14ad04-7499-4000-a053-3fd7d87ca132"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4558), "admin001", null, null, 1, null, null },
                    { new Guid("7c84f59a-4712-4d12-b244-e4230812f874"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4506), "admin001", null, null, 1, null, null },
                    { new Guid("7c84f59a-4712-4d12-b244-e4230812f874"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4508), "admin001", null, null, 1, null, null },
                    { new Guid("7c84f59a-4712-4d12-b244-e4230812f874"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4509), "admin001", null, null, 1, null, null },
                    { new Guid("7c84f59a-4712-4d12-b244-e4230812f874"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4510), "admin001", null, null, 1, null, null },
                    { new Guid("7c84f59a-4712-4d12-b244-e4230812f874"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4510), "admin001", null, null, 1, null, null },
                    { new Guid("7c84f59a-4712-4d12-b244-e4230812f874"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4511), "admin001", null, null, 1, null, null },
                    { new Guid("7c84f59a-4712-4d12-b244-e4230812f874"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4512), "admin001", null, null, 1, null, null },
                    { new Guid("7c84f59a-4712-4d12-b244-e4230812f874"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4512), "admin001", null, null, 1, null, null },
                    { new Guid("7ca153c1-4079-4a92-be13-6fe64607f0de"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5003), "admin001", null, null, 1, null, null },
                    { new Guid("7ca153c1-4079-4a92-be13-6fe64607f0de"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5003), "admin001", null, null, 1, null, null },
                    { new Guid("7ca153c1-4079-4a92-be13-6fe64607f0de"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5004), "admin001", null, null, 1, null, null },
                    { new Guid("7ca153c1-4079-4a92-be13-6fe64607f0de"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5005), "admin001", null, null, 1, null, null },
                    { new Guid("7ca153c1-4079-4a92-be13-6fe64607f0de"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5005), "admin001", null, null, 1, null, null },
                    { new Guid("7ca153c1-4079-4a92-be13-6fe64607f0de"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5006), "admin001", null, null, 1, null, null },
                    { new Guid("7ca153c1-4079-4a92-be13-6fe64607f0de"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5007), "admin001", null, null, 1, null, null },
                    { new Guid("7ca153c1-4079-4a92-be13-6fe64607f0de"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5007), "admin001", null, null, 1, null, null },
                    { new Guid("7d033af7-bdd9-4855-8d09-82629f2a303b"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4954), "admin001", null, null, 1, null, null },
                    { new Guid("7d033af7-bdd9-4855-8d09-82629f2a303b"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4954), "admin001", null, null, 1, null, null },
                    { new Guid("7d033af7-bdd9-4855-8d09-82629f2a303b"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4955), "admin001", null, null, 1, null, null },
                    { new Guid("7d033af7-bdd9-4855-8d09-82629f2a303b"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4955), "admin001", null, null, 1, null, null },
                    { new Guid("7d033af7-bdd9-4855-8d09-82629f2a303b"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4956), "admin001", null, null, 1, null, null },
                    { new Guid("7d033af7-bdd9-4855-8d09-82629f2a303b"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4957), "admin001", null, null, 1, null, null },
                    { new Guid("7d033af7-bdd9-4855-8d09-82629f2a303b"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4957), "admin001", null, null, 1, null, null },
                    { new Guid("7d033af7-bdd9-4855-8d09-82629f2a303b"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4958), "admin001", null, null, 1, null, null },
                    { new Guid("80940ef4-8bef-48d0-9299-be07d0f95782"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4753), "admin001", null, null, 1, null, null },
                    { new Guid("80940ef4-8bef-48d0-9299-be07d0f95782"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4753), "admin001", null, null, 1, null, null },
                    { new Guid("80940ef4-8bef-48d0-9299-be07d0f95782"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4754), "admin001", null, null, 1, null, null },
                    { new Guid("80940ef4-8bef-48d0-9299-be07d0f95782"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4755), "admin001", null, null, 1, null, null },
                    { new Guid("80940ef4-8bef-48d0-9299-be07d0f95782"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4755), "admin001", null, null, 1, null, null },
                    { new Guid("80940ef4-8bef-48d0-9299-be07d0f95782"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4756), "admin001", null, null, 1, null, null },
                    { new Guid("80940ef4-8bef-48d0-9299-be07d0f95782"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4757), "admin001", null, null, 1, null, null },
                    { new Guid("80940ef4-8bef-48d0-9299-be07d0f95782"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4757), "admin001", null, null, 1, null, null },
                    { new Guid("83a2b881-1346-49c8-8680-b5d445fc0641"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4734), "admin001", null, null, 1, null, null },
                    { new Guid("83a2b881-1346-49c8-8680-b5d445fc0641"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4735), "admin001", null, null, 1, null, null },
                    { new Guid("83a2b881-1346-49c8-8680-b5d445fc0641"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4735), "admin001", null, null, 1, null, null },
                    { new Guid("83a2b881-1346-49c8-8680-b5d445fc0641"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4736), "admin001", null, null, 1, null, null },
                    { new Guid("83a2b881-1346-49c8-8680-b5d445fc0641"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4736), "admin001", null, null, 1, null, null },
                    { new Guid("83a2b881-1346-49c8-8680-b5d445fc0641"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4737), "admin001", null, null, 1, null, null },
                    { new Guid("83a2b881-1346-49c8-8680-b5d445fc0641"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4738), "admin001", null, null, 1, null, null },
                    { new Guid("83a2b881-1346-49c8-8680-b5d445fc0641"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4738), "admin001", null, null, 1, null, null },
                    { new Guid("840fc1d6-628d-4bb6-aeef-937ac85b8b01"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4395), "admin001", null, null, 1, null, null },
                    { new Guid("840fc1d6-628d-4bb6-aeef-937ac85b8b01"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4396), "admin001", null, null, 1, null, null },
                    { new Guid("840fc1d6-628d-4bb6-aeef-937ac85b8b01"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4397), "admin001", null, null, 1, null, null },
                    { new Guid("840fc1d6-628d-4bb6-aeef-937ac85b8b01"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4397), "admin001", null, null, 1, null, null },
                    { new Guid("840fc1d6-628d-4bb6-aeef-937ac85b8b01"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4398), "admin001", null, null, 1, null, null },
                    { new Guid("840fc1d6-628d-4bb6-aeef-937ac85b8b01"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4398), "admin001", null, null, 1, null, null },
                    { new Guid("840fc1d6-628d-4bb6-aeef-937ac85b8b01"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4399), "admin001", null, null, 1, null, null },
                    { new Guid("840fc1d6-628d-4bb6-aeef-937ac85b8b01"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4400), "admin001", null, null, 1, null, null },
                    { new Guid("87a93920-4b52-4b64-8d52-975a9d5035c5"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4664), "admin001", null, null, 1, null, null },
                    { new Guid("87a93920-4b52-4b64-8d52-975a9d5035c5"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4664), "admin001", null, null, 1, null, null },
                    { new Guid("87a93920-4b52-4b64-8d52-975a9d5035c5"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4665), "admin001", null, null, 1, null, null },
                    { new Guid("87a93920-4b52-4b64-8d52-975a9d5035c5"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4665), "admin001", null, null, 1, null, null },
                    { new Guid("87a93920-4b52-4b64-8d52-975a9d5035c5"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4666), "admin001", null, null, 1, null, null },
                    { new Guid("87a93920-4b52-4b64-8d52-975a9d5035c5"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4667), "admin001", null, null, 1, null, null },
                    { new Guid("87a93920-4b52-4b64-8d52-975a9d5035c5"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4667), "admin001", null, null, 1, null, null },
                    { new Guid("87a93920-4b52-4b64-8d52-975a9d5035c5"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4668), "admin001", null, null, 1, null, null },
                    { new Guid("882ae160-fb8b-4ff2-8552-8057237b1330"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4883), "admin001", null, null, 1, null, null },
                    { new Guid("882ae160-fb8b-4ff2-8552-8057237b1330"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4883), "admin001", null, null, 1, null, null },
                    { new Guid("882ae160-fb8b-4ff2-8552-8057237b1330"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4884), "admin001", null, null, 1, null, null },
                    { new Guid("882ae160-fb8b-4ff2-8552-8057237b1330"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4885), "admin001", null, null, 1, null, null },
                    { new Guid("882ae160-fb8b-4ff2-8552-8057237b1330"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4885), "admin001", null, null, 1, null, null },
                    { new Guid("882ae160-fb8b-4ff2-8552-8057237b1330"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4886), "admin001", null, null, 1, null, null },
                    { new Guid("882ae160-fb8b-4ff2-8552-8057237b1330"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4886), "admin001", null, null, 1, null, null },
                    { new Guid("882ae160-fb8b-4ff2-8552-8057237b1330"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4887), "admin001", null, null, 1, null, null },
                    { new Guid("8b8514eb-1c9e-4cc1-bc58-17b520fe9716"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4911), "admin001", null, null, 1, null, null },
                    { new Guid("8b8514eb-1c9e-4cc1-bc58-17b520fe9716"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4912), "admin001", null, null, 1, null, null },
                    { new Guid("8b8514eb-1c9e-4cc1-bc58-17b520fe9716"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4912), "admin001", null, null, 1, null, null },
                    { new Guid("8b8514eb-1c9e-4cc1-bc58-17b520fe9716"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4913), "admin001", null, null, 1, null, null },
                    { new Guid("8b8514eb-1c9e-4cc1-bc58-17b520fe9716"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4913), "admin001", null, null, 1, null, null },
                    { new Guid("8b8514eb-1c9e-4cc1-bc58-17b520fe9716"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4914), "admin001", null, null, 1, null, null },
                    { new Guid("8b8514eb-1c9e-4cc1-bc58-17b520fe9716"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4915), "admin001", null, null, 1, null, null },
                    { new Guid("8b8514eb-1c9e-4cc1-bc58-17b520fe9716"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4915), "admin001", null, null, 1, null, null },
                    { new Guid("915a7cff-22ca-4242-8038-9c0cd91af6f8"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4796), "admin001", null, null, 1, null, null },
                    { new Guid("915a7cff-22ca-4242-8038-9c0cd91af6f8"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4796), "admin001", null, null, 1, null, null },
                    { new Guid("915a7cff-22ca-4242-8038-9c0cd91af6f8"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4797), "admin001", null, null, 1, null, null },
                    { new Guid("915a7cff-22ca-4242-8038-9c0cd91af6f8"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4798), "admin001", null, null, 1, null, null },
                    { new Guid("915a7cff-22ca-4242-8038-9c0cd91af6f8"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4798), "admin001", null, null, 1, null, null },
                    { new Guid("915a7cff-22ca-4242-8038-9c0cd91af6f8"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4799), "admin001", null, null, 1, null, null },
                    { new Guid("915a7cff-22ca-4242-8038-9c0cd91af6f8"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4800), "admin001", null, null, 1, null, null },
                    { new Guid("915a7cff-22ca-4242-8038-9c0cd91af6f8"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4800), "admin001", null, null, 1, null, null },
                    { new Guid("9200a3f4-5955-4505-914d-b0eb66f9dc7e"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4861), "admin001", null, null, 1, null, null },
                    { new Guid("9200a3f4-5955-4505-914d-b0eb66f9dc7e"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4862), "admin001", null, null, 1, null, null },
                    { new Guid("9200a3f4-5955-4505-914d-b0eb66f9dc7e"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4862), "admin001", null, null, 1, null, null },
                    { new Guid("9200a3f4-5955-4505-914d-b0eb66f9dc7e"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4869), "admin001", null, null, 1, null, null },
                    { new Guid("9200a3f4-5955-4505-914d-b0eb66f9dc7e"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4870), "admin001", null, null, 1, null, null },
                    { new Guid("9200a3f4-5955-4505-914d-b0eb66f9dc7e"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4870), "admin001", null, null, 1, null, null },
                    { new Guid("9200a3f4-5955-4505-914d-b0eb66f9dc7e"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4871), "admin001", null, null, 1, null, null },
                    { new Guid("9200a3f4-5955-4505-914d-b0eb66f9dc7e"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4872), "admin001", null, null, 1, null, null },
                    { new Guid("9202f9ab-7844-4823-ac27-71b3b6782f88"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4773), "admin001", null, null, 1, null, null },
                    { new Guid("9202f9ab-7844-4823-ac27-71b3b6782f88"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4774), "admin001", null, null, 1, null, null },
                    { new Guid("9202f9ab-7844-4823-ac27-71b3b6782f88"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4775), "admin001", null, null, 1, null, null },
                    { new Guid("9202f9ab-7844-4823-ac27-71b3b6782f88"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4775), "admin001", null, null, 1, null, null },
                    { new Guid("9202f9ab-7844-4823-ac27-71b3b6782f88"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4776), "admin001", null, null, 1, null, null },
                    { new Guid("9202f9ab-7844-4823-ac27-71b3b6782f88"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4776), "admin001", null, null, 1, null, null },
                    { new Guid("9202f9ab-7844-4823-ac27-71b3b6782f88"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4777), "admin001", null, null, 1, null, null },
                    { new Guid("9202f9ab-7844-4823-ac27-71b3b6782f88"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4778), "admin001", null, null, 1, null, null },
                    { new Guid("92d61cbc-fbf3-41ac-85fc-11f0c1a28186"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5062), "admin001", null, null, 1, null, null },
                    { new Guid("92d61cbc-fbf3-41ac-85fc-11f0c1a28186"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5063), "admin001", null, null, 1, null, null },
                    { new Guid("92d61cbc-fbf3-41ac-85fc-11f0c1a28186"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5063), "admin001", null, null, 1, null, null },
                    { new Guid("92d61cbc-fbf3-41ac-85fc-11f0c1a28186"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5072), "admin001", null, null, 1, null, null },
                    { new Guid("92d61cbc-fbf3-41ac-85fc-11f0c1a28186"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5073), "admin001", null, null, 1, null, null },
                    { new Guid("92d61cbc-fbf3-41ac-85fc-11f0c1a28186"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5074), "admin001", null, null, 1, null, null },
                    { new Guid("92d61cbc-fbf3-41ac-85fc-11f0c1a28186"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5074), "admin001", null, null, 1, null, null },
                    { new Guid("92d61cbc-fbf3-41ac-85fc-11f0c1a28186"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5075), "admin001", null, null, 1, null, null },
                    { new Guid("9347cab2-df18-4caa-a94c-19191678e881"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4878), "admin001", null, null, 1, null, null },
                    { new Guid("9347cab2-df18-4caa-a94c-19191678e881"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4878), "admin001", null, null, 1, null, null },
                    { new Guid("9347cab2-df18-4caa-a94c-19191678e881"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4879), "admin001", null, null, 1, null, null },
                    { new Guid("9347cab2-df18-4caa-a94c-19191678e881"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4879), "admin001", null, null, 1, null, null },
                    { new Guid("9347cab2-df18-4caa-a94c-19191678e881"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4880), "admin001", null, null, 1, null, null },
                    { new Guid("9347cab2-df18-4caa-a94c-19191678e881"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4881), "admin001", null, null, 1, null, null },
                    { new Guid("9347cab2-df18-4caa-a94c-19191678e881"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4881), "admin001", null, null, 1, null, null },
                    { new Guid("9347cab2-df18-4caa-a94c-19191678e881"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4882), "admin001", null, null, 1, null, null },
                    { new Guid("958f872d-0221-4e6d-bc27-93bdf2f18adf"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4360), "admin001", null, null, 1, null, null },
                    { new Guid("958f872d-0221-4e6d-bc27-93bdf2f18adf"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4360), "admin001", null, null, 1, null, null },
                    { new Guid("958f872d-0221-4e6d-bc27-93bdf2f18adf"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4361), "admin001", null, null, 1, null, null },
                    { new Guid("958f872d-0221-4e6d-bc27-93bdf2f18adf"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4362), "admin001", null, null, 1, null, null },
                    { new Guid("958f872d-0221-4e6d-bc27-93bdf2f18adf"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4362), "admin001", null, null, 1, null, null },
                    { new Guid("958f872d-0221-4e6d-bc27-93bdf2f18adf"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4363), "admin001", null, null, 1, null, null },
                    { new Guid("958f872d-0221-4e6d-bc27-93bdf2f18adf"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4363), "admin001", null, null, 1, null, null },
                    { new Guid("958f872d-0221-4e6d-bc27-93bdf2f18adf"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4364), "admin001", null, null, 1, null, null },
                    { new Guid("99d8eb9c-2343-4baf-b417-fd0ebf1dd45c"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4612), "admin001", null, null, 1, null, null },
                    { new Guid("99d8eb9c-2343-4baf-b417-fd0ebf1dd45c"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4613), "admin001", null, null, 1, null, null },
                    { new Guid("99d8eb9c-2343-4baf-b417-fd0ebf1dd45c"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4613), "admin001", null, null, 1, null, null },
                    { new Guid("99d8eb9c-2343-4baf-b417-fd0ebf1dd45c"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4614), "admin001", null, null, 1, null, null },
                    { new Guid("99d8eb9c-2343-4baf-b417-fd0ebf1dd45c"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4615), "admin001", null, null, 1, null, null },
                    { new Guid("99d8eb9c-2343-4baf-b417-fd0ebf1dd45c"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4615), "admin001", null, null, 1, null, null },
                    { new Guid("99d8eb9c-2343-4baf-b417-fd0ebf1dd45c"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4616), "admin001", null, null, 1, null, null },
                    { new Guid("99d8eb9c-2343-4baf-b417-fd0ebf1dd45c"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4617), "admin001", null, null, 1, null, null },
                    { new Guid("9b57d396-d566-4074-93a4-709c7f88665c"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4513), "admin001", null, null, 1, null, null },
                    { new Guid("9b57d396-d566-4074-93a4-709c7f88665c"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4514), "admin001", null, null, 1, null, null },
                    { new Guid("9b57d396-d566-4074-93a4-709c7f88665c"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4514), "admin001", null, null, 1, null, null },
                    { new Guid("9b57d396-d566-4074-93a4-709c7f88665c"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4515), "admin001", null, null, 1, null, null },
                    { new Guid("9b57d396-d566-4074-93a4-709c7f88665c"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4516), "admin001", null, null, 1, null, null },
                    { new Guid("9b57d396-d566-4074-93a4-709c7f88665c"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4516), "admin001", null, null, 1, null, null },
                    { new Guid("9b57d396-d566-4074-93a4-709c7f88665c"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4517), "admin001", null, null, 1, null, null },
                    { new Guid("9b57d396-d566-4074-93a4-709c7f88665c"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4518), "admin001", null, null, 1, null, null },
                    { new Guid("9c765e6e-c77c-4e4f-b2bc-e9397b3d4373"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4518), "admin001", null, null, 1, null, null },
                    { new Guid("9c765e6e-c77c-4e4f-b2bc-e9397b3d4373"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4528), "admin001", null, null, 1, null, null },
                    { new Guid("9c765e6e-c77c-4e4f-b2bc-e9397b3d4373"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4529), "admin001", null, null, 1, null, null },
                    { new Guid("9c765e6e-c77c-4e4f-b2bc-e9397b3d4373"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4530), "admin001", null, null, 1, null, null },
                    { new Guid("9c765e6e-c77c-4e4f-b2bc-e9397b3d4373"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4531), "admin001", null, null, 1, null, null },
                    { new Guid("9c765e6e-c77c-4e4f-b2bc-e9397b3d4373"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4531), "admin001", null, null, 1, null, null },
                    { new Guid("9c765e6e-c77c-4e4f-b2bc-e9397b3d4373"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4532), "admin001", null, null, 1, null, null },
                    { new Guid("9c765e6e-c77c-4e4f-b2bc-e9397b3d4373"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4532), "admin001", null, null, 1, null, null },
                    { new Guid("a3c22019-836b-41b6-ba5b-fab1ac0ad1b0"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4431), "admin001", null, null, 1, null, null },
                    { new Guid("a3c22019-836b-41b6-ba5b-fab1ac0ad1b0"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4431), "admin001", null, null, 1, null, null },
                    { new Guid("a3c22019-836b-41b6-ba5b-fab1ac0ad1b0"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4432), "admin001", null, null, 1, null, null },
                    { new Guid("a3c22019-836b-41b6-ba5b-fab1ac0ad1b0"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4433), "admin001", null, null, 1, null, null },
                    { new Guid("a3c22019-836b-41b6-ba5b-fab1ac0ad1b0"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4433), "admin001", null, null, 1, null, null },
                    { new Guid("a3c22019-836b-41b6-ba5b-fab1ac0ad1b0"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4434), "admin001", null, null, 1, null, null },
                    { new Guid("a3c22019-836b-41b6-ba5b-fab1ac0ad1b0"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4434), "admin001", null, null, 1, null, null },
                    { new Guid("a3c22019-836b-41b6-ba5b-fab1ac0ad1b0"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4435), "admin001", null, null, 1, null, null },
                    { new Guid("a67092c6-9810-4f29-b622-5696d48bb229"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4801), "admin001", null, null, 1, null, null },
                    { new Guid("a67092c6-9810-4f29-b622-5696d48bb229"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4802), "admin001", null, null, 1, null, null },
                    { new Guid("a67092c6-9810-4f29-b622-5696d48bb229"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4802), "admin001", null, null, 1, null, null },
                    { new Guid("a67092c6-9810-4f29-b622-5696d48bb229"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4803), "admin001", null, null, 1, null, null },
                    { new Guid("a67092c6-9810-4f29-b622-5696d48bb229"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4803), "admin001", null, null, 1, null, null },
                    { new Guid("a67092c6-9810-4f29-b622-5696d48bb229"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4804), "admin001", null, null, 1, null, null },
                    { new Guid("a67092c6-9810-4f29-b622-5696d48bb229"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4805), "admin001", null, null, 1, null, null },
                    { new Guid("a67092c6-9810-4f29-b622-5696d48bb229"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4805), "admin001", null, null, 1, null, null },
                    { new Guid("a6ce80e3-0009-49db-95ce-f141fac3cfaf"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4638), "admin001", null, null, 1, null, null },
                    { new Guid("a6ce80e3-0009-49db-95ce-f141fac3cfaf"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4638), "admin001", null, null, 1, null, null },
                    { new Guid("a6ce80e3-0009-49db-95ce-f141fac3cfaf"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4639), "admin001", null, null, 1, null, null },
                    { new Guid("a6ce80e3-0009-49db-95ce-f141fac3cfaf"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4640), "admin001", null, null, 1, null, null },
                    { new Guid("a6ce80e3-0009-49db-95ce-f141fac3cfaf"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4640), "admin001", null, null, 1, null, null },
                    { new Guid("a6ce80e3-0009-49db-95ce-f141fac3cfaf"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4641), "admin001", null, null, 1, null, null },
                    { new Guid("a6ce80e3-0009-49db-95ce-f141fac3cfaf"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4641), "admin001", null, null, 1, null, null },
                    { new Guid("a6ce80e3-0009-49db-95ce-f141fac3cfaf"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4642), "admin001", null, null, 1, null, null },
                    { new Guid("a8406847-bf38-4b0f-a3fd-be17679ef7a8"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4888), "admin001", null, null, 1, null, null },
                    { new Guid("a8406847-bf38-4b0f-a3fd-be17679ef7a8"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4888), "admin001", null, null, 1, null, null },
                    { new Guid("a8406847-bf38-4b0f-a3fd-be17679ef7a8"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4889), "admin001", null, null, 1, null, null },
                    { new Guid("a8406847-bf38-4b0f-a3fd-be17679ef7a8"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4890), "admin001", null, null, 1, null, null },
                    { new Guid("a8406847-bf38-4b0f-a3fd-be17679ef7a8"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4890), "admin001", null, null, 1, null, null },
                    { new Guid("a8406847-bf38-4b0f-a3fd-be17679ef7a8"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4891), "admin001", null, null, 1, null, null },
                    { new Guid("a8406847-bf38-4b0f-a3fd-be17679ef7a8"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4892), "admin001", null, null, 1, null, null },
                    { new Guid("a8406847-bf38-4b0f-a3fd-be17679ef7a8"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4892), "admin001", null, null, 1, null, null },
                    { new Guid("a9f2d799-6ddb-4e52-bea4-bcd7c9344c98"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4763), "admin001", null, null, 1, null, null },
                    { new Guid("a9f2d799-6ddb-4e52-bea4-bcd7c9344c98"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4764), "admin001", null, null, 1, null, null },
                    { new Guid("a9f2d799-6ddb-4e52-bea4-bcd7c9344c98"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4764), "admin001", null, null, 1, null, null },
                    { new Guid("a9f2d799-6ddb-4e52-bea4-bcd7c9344c98"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4765), "admin001", null, null, 1, null, null },
                    { new Guid("a9f2d799-6ddb-4e52-bea4-bcd7c9344c98"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4766), "admin001", null, null, 1, null, null },
                    { new Guid("a9f2d799-6ddb-4e52-bea4-bcd7c9344c98"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4766), "admin001", null, null, 1, null, null },
                    { new Guid("a9f2d799-6ddb-4e52-bea4-bcd7c9344c98"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4767), "admin001", null, null, 1, null, null },
                    { new Guid("a9f2d799-6ddb-4e52-bea4-bcd7c9344c98"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4767), "admin001", null, null, 1, null, null },
                    { new Guid("af882899-0393-4094-abd3-d01f1dbd591a"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5076), "admin001", null, null, 1, null, null },
                    { new Guid("af882899-0393-4094-abd3-d01f1dbd591a"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5076), "admin001", null, null, 1, null, null },
                    { new Guid("af882899-0393-4094-abd3-d01f1dbd591a"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5077), "admin001", null, null, 1, null, null },
                    { new Guid("af882899-0393-4094-abd3-d01f1dbd591a"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5077), "admin001", null, null, 1, null, null },
                    { new Guid("af882899-0393-4094-abd3-d01f1dbd591a"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5078), "admin001", null, null, 1, null, null },
                    { new Guid("af882899-0393-4094-abd3-d01f1dbd591a"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5079), "admin001", null, null, 1, null, null },
                    { new Guid("af882899-0393-4094-abd3-d01f1dbd591a"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5079), "admin001", null, null, 1, null, null },
                    { new Guid("af882899-0393-4094-abd3-d01f1dbd591a"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5080), "admin001", null, null, 1, null, null },
                    { new Guid("b5ff1be4-457a-4531-8fc0-731585033d65"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4926), "admin001", null, null, 1, null, null },
                    { new Guid("b5ff1be4-457a-4531-8fc0-731585033d65"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4927), "admin001", null, null, 1, null, null },
                    { new Guid("b5ff1be4-457a-4531-8fc0-731585033d65"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4927), "admin001", null, null, 1, null, null },
                    { new Guid("b5ff1be4-457a-4531-8fc0-731585033d65"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4928), "admin001", null, null, 1, null, null },
                    { new Guid("b5ff1be4-457a-4531-8fc0-731585033d65"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4929), "admin001", null, null, 1, null, null },
                    { new Guid("b5ff1be4-457a-4531-8fc0-731585033d65"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4929), "admin001", null, null, 1, null, null },
                    { new Guid("b5ff1be4-457a-4531-8fc0-731585033d65"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4930), "admin001", null, null, 1, null, null },
                    { new Guid("b5ff1be4-457a-4531-8fc0-731585033d65"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4931), "admin001", null, null, 1, null, null },
                    { new Guid("b6269d88-ff91-410f-977b-09987052789b"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4365), "admin001", null, null, 1, null, null },
                    { new Guid("b6269d88-ff91-410f-977b-09987052789b"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4365), "admin001", null, null, 1, null, null },
                    { new Guid("b6269d88-ff91-410f-977b-09987052789b"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4374), "admin001", null, null, 1, null, null },
                    { new Guid("b6269d88-ff91-410f-977b-09987052789b"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4375), "admin001", null, null, 1, null, null },
                    { new Guid("b6269d88-ff91-410f-977b-09987052789b"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4376), "admin001", null, null, 1, null, null },
                    { new Guid("b6269d88-ff91-410f-977b-09987052789b"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4376), "admin001", null, null, 1, null, null },
                    { new Guid("b6269d88-ff91-410f-977b-09987052789b"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4377), "admin001", null, null, 1, null, null },
                    { new Guid("b6269d88-ff91-410f-977b-09987052789b"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4378), "admin001", null, null, 1, null, null },
                    { new Guid("b694ee05-7bc0-42c8-935c-14ed4bfbd394"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4389), "admin001", null, null, 1, null, null },
                    { new Guid("b694ee05-7bc0-42c8-935c-14ed4bfbd394"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4391), "admin001", null, null, 1, null, null },
                    { new Guid("b694ee05-7bc0-42c8-935c-14ed4bfbd394"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4391), "admin001", null, null, 1, null, null },
                    { new Guid("b694ee05-7bc0-42c8-935c-14ed4bfbd394"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4392), "admin001", null, null, 1, null, null },
                    { new Guid("b694ee05-7bc0-42c8-935c-14ed4bfbd394"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4393), "admin001", null, null, 1, null, null },
                    { new Guid("b694ee05-7bc0-42c8-935c-14ed4bfbd394"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4393), "admin001", null, null, 1, null, null },
                    { new Guid("b694ee05-7bc0-42c8-935c-14ed4bfbd394"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4394), "admin001", null, null, 1, null, null },
                    { new Guid("b694ee05-7bc0-42c8-935c-14ed4bfbd394"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4394), "admin001", null, null, 1, null, null },
                    { new Guid("b78d1d22-8806-455a-9a45-fac21714167e"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4544), "admin001", null, null, 1, null, null },
                    { new Guid("b78d1d22-8806-455a-9a45-fac21714167e"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4544), "admin001", null, null, 1, null, null },
                    { new Guid("b78d1d22-8806-455a-9a45-fac21714167e"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4545), "admin001", null, null, 1, null, null },
                    { new Guid("b78d1d22-8806-455a-9a45-fac21714167e"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4545), "admin001", null, null, 1, null, null },
                    { new Guid("b78d1d22-8806-455a-9a45-fac21714167e"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4546), "admin001", null, null, 1, null, null },
                    { new Guid("b78d1d22-8806-455a-9a45-fac21714167e"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4547), "admin001", null, null, 1, null, null },
                    { new Guid("b78d1d22-8806-455a-9a45-fac21714167e"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4547), "admin001", null, null, 1, null, null },
                    { new Guid("b78d1d22-8806-455a-9a45-fac21714167e"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4548), "admin001", null, null, 1, null, null },
                    { new Guid("b9bb7c92-5a04-46d9-a881-0ed8a9c6ecb8"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4659), "admin001", null, null, 1, null, null },
                    { new Guid("b9bb7c92-5a04-46d9-a881-0ed8a9c6ecb8"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4659), "admin001", null, null, 1, null, null },
                    { new Guid("b9bb7c92-5a04-46d9-a881-0ed8a9c6ecb8"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4660), "admin001", null, null, 1, null, null },
                    { new Guid("b9bb7c92-5a04-46d9-a881-0ed8a9c6ecb8"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4660), "admin001", null, null, 1, null, null },
                    { new Guid("b9bb7c92-5a04-46d9-a881-0ed8a9c6ecb8"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4661), "admin001", null, null, 1, null, null },
                    { new Guid("b9bb7c92-5a04-46d9-a881-0ed8a9c6ecb8"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4662), "admin001", null, null, 1, null, null },
                    { new Guid("b9bb7c92-5a04-46d9-a881-0ed8a9c6ecb8"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4662), "admin001", null, null, 1, null, null },
                    { new Guid("b9bb7c92-5a04-46d9-a881-0ed8a9c6ecb8"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4663), "admin001", null, null, 1, null, null },
                    { new Guid("bd482162-7de6-45b8-86a2-0e8dc397d1c9"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4840), "admin001", null, null, 1, null, null },
                    { new Guid("bd482162-7de6-45b8-86a2-0e8dc397d1c9"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4841), "admin001", null, null, 1, null, null },
                    { new Guid("bd482162-7de6-45b8-86a2-0e8dc397d1c9"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4842), "admin001", null, null, 1, null, null },
                    { new Guid("bd482162-7de6-45b8-86a2-0e8dc397d1c9"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4842), "admin001", null, null, 1, null, null },
                    { new Guid("bd482162-7de6-45b8-86a2-0e8dc397d1c9"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4843), "admin001", null, null, 1, null, null },
                    { new Guid("bd482162-7de6-45b8-86a2-0e8dc397d1c9"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4844), "admin001", null, null, 1, null, null },
                    { new Guid("bd482162-7de6-45b8-86a2-0e8dc397d1c9"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4844), "admin001", null, null, 1, null, null },
                    { new Guid("bd482162-7de6-45b8-86a2-0e8dc397d1c9"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4845), "admin001", null, null, 1, null, null },
                    { new Guid("c1c91dc7-6e70-4eb6-ba06-548cfe15aaed"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4970), "admin001", null, null, 1, null, null },
                    { new Guid("c1c91dc7-6e70-4eb6-ba06-548cfe15aaed"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4971), "admin001", null, null, 1, null, null },
                    { new Guid("c1c91dc7-6e70-4eb6-ba06-548cfe15aaed"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4972), "admin001", null, null, 1, null, null },
                    { new Guid("c1c91dc7-6e70-4eb6-ba06-548cfe15aaed"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4972), "admin001", null, null, 1, null, null },
                    { new Guid("c1c91dc7-6e70-4eb6-ba06-548cfe15aaed"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4973), "admin001", null, null, 1, null, null },
                    { new Guid("c1c91dc7-6e70-4eb6-ba06-548cfe15aaed"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4973), "admin001", null, null, 1, null, null },
                    { new Guid("c1c91dc7-6e70-4eb6-ba06-548cfe15aaed"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4974), "admin001", null, null, 1, null, null },
                    { new Guid("c1c91dc7-6e70-4eb6-ba06-548cfe15aaed"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4975), "admin001", null, null, 1, null, null },
                    { new Guid("c4c36d25-79da-46ce-9e69-3774545e7c3e"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4622), "admin001", null, null, 1, null, null },
                    { new Guid("c4c36d25-79da-46ce-9e69-3774545e7c3e"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4623), "admin001", null, null, 1, null, null },
                    { new Guid("c4c36d25-79da-46ce-9e69-3774545e7c3e"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4624), "admin001", null, null, 1, null, null },
                    { new Guid("c4c36d25-79da-46ce-9e69-3774545e7c3e"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4624), "admin001", null, null, 1, null, null },
                    { new Guid("c4c36d25-79da-46ce-9e69-3774545e7c3e"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4625), "admin001", null, null, 1, null, null },
                    { new Guid("c4c36d25-79da-46ce-9e69-3774545e7c3e"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4626), "admin001", null, null, 1, null, null },
                    { new Guid("c4c36d25-79da-46ce-9e69-3774545e7c3e"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4626), "admin001", null, null, 1, null, null },
                    { new Guid("c4c36d25-79da-46ce-9e69-3774545e7c3e"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4627), "admin001", null, null, 1, null, null },
                    { new Guid("c70910c0-c236-4d3e-8df5-cc6d4647642f"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5042), "admin001", null, null, 1, null, null },
                    { new Guid("c70910c0-c236-4d3e-8df5-cc6d4647642f"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5042), "admin001", null, null, 1, null, null },
                    { new Guid("c70910c0-c236-4d3e-8df5-cc6d4647642f"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5043), "admin001", null, null, 1, null, null },
                    { new Guid("c70910c0-c236-4d3e-8df5-cc6d4647642f"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5043), "admin001", null, null, 1, null, null },
                    { new Guid("c70910c0-c236-4d3e-8df5-cc6d4647642f"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5044), "admin001", null, null, 1, null, null },
                    { new Guid("c70910c0-c236-4d3e-8df5-cc6d4647642f"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5045), "admin001", null, null, 1, null, null },
                    { new Guid("c70910c0-c236-4d3e-8df5-cc6d4647642f"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5045), "admin001", null, null, 1, null, null },
                    { new Guid("c70910c0-c236-4d3e-8df5-cc6d4647642f"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5046), "admin001", null, null, 1, null, null },
                    { new Guid("c7a26b9e-da22-4a2b-9b51-7d52d99baa2a"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4674), "admin001", null, null, 1, null, null },
                    { new Guid("c7a26b9e-da22-4a2b-9b51-7d52d99baa2a"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4675), "admin001", null, null, 1, null, null },
                    { new Guid("c7a26b9e-da22-4a2b-9b51-7d52d99baa2a"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4675), "admin001", null, null, 1, null, null },
                    { new Guid("c7a26b9e-da22-4a2b-9b51-7d52d99baa2a"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4676), "admin001", null, null, 1, null, null },
                    { new Guid("c7a26b9e-da22-4a2b-9b51-7d52d99baa2a"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4676), "admin001", null, null, 1, null, null },
                    { new Guid("c7a26b9e-da22-4a2b-9b51-7d52d99baa2a"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4677), "admin001", null, null, 1, null, null },
                    { new Guid("c7a26b9e-da22-4a2b-9b51-7d52d99baa2a"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4678), "admin001", null, null, 1, null, null },
                    { new Guid("c7a26b9e-da22-4a2b-9b51-7d52d99baa2a"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4678), "admin001", null, null, 1, null, null },
                    { new Guid("c7d9abc3-b445-4da6-b5ab-e45fcdc49738"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5013), "admin001", null, null, 1, null, null },
                    { new Guid("c7d9abc3-b445-4da6-b5ab-e45fcdc49738"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5014), "admin001", null, null, 1, null, null },
                    { new Guid("c7d9abc3-b445-4da6-b5ab-e45fcdc49738"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5014), "admin001", null, null, 1, null, null },
                    { new Guid("c7d9abc3-b445-4da6-b5ab-e45fcdc49738"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5015), "admin001", null, null, 1, null, null },
                    { new Guid("c7d9abc3-b445-4da6-b5ab-e45fcdc49738"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5016), "admin001", null, null, 1, null, null },
                    { new Guid("c7d9abc3-b445-4da6-b5ab-e45fcdc49738"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5016), "admin001", null, null, 1, null, null },
                    { new Guid("c7d9abc3-b445-4da6-b5ab-e45fcdc49738"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5017), "admin001", null, null, 1, null, null },
                    { new Guid("c7d9abc3-b445-4da6-b5ab-e45fcdc49738"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5018), "admin001", null, null, 1, null, null },
                    { new Guid("c985f6f5-83d0-4e5a-9a53-401de650823a"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5018), "admin001", null, null, 1, null, null },
                    { new Guid("c985f6f5-83d0-4e5a-9a53-401de650823a"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5019), "admin001", null, null, 1, null, null },
                    { new Guid("c985f6f5-83d0-4e5a-9a53-401de650823a"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5020), "admin001", null, null, 1, null, null },
                    { new Guid("c985f6f5-83d0-4e5a-9a53-401de650823a"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5020), "admin001", null, null, 1, null, null },
                    { new Guid("c985f6f5-83d0-4e5a-9a53-401de650823a"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5021), "admin001", null, null, 1, null, null },
                    { new Guid("c985f6f5-83d0-4e5a-9a53-401de650823a"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5021), "admin001", null, null, 1, null, null },
                    { new Guid("c985f6f5-83d0-4e5a-9a53-401de650823a"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5022), "admin001", null, null, 1, null, null },
                    { new Guid("c985f6f5-83d0-4e5a-9a53-401de650823a"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5023), "admin001", null, null, 1, null, null },
                    { new Guid("c9d66d2d-acf3-4f68-a7ad-7d578c089f8b"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4320), "admin001", null, null, 1, null, null },
                    { new Guid("c9d66d2d-acf3-4f68-a7ad-7d578c089f8b"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4321), "admin001", null, null, 1, null, null },
                    { new Guid("c9d66d2d-acf3-4f68-a7ad-7d578c089f8b"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4322), "admin001", null, null, 1, null, null },
                    { new Guid("c9d66d2d-acf3-4f68-a7ad-7d578c089f8b"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4322), "admin001", null, null, 1, null, null },
                    { new Guid("c9d66d2d-acf3-4f68-a7ad-7d578c089f8b"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4323), "admin001", null, null, 1, null, null },
                    { new Guid("c9d66d2d-acf3-4f68-a7ad-7d578c089f8b"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4324), "admin001", null, null, 1, null, null },
                    { new Guid("c9d66d2d-acf3-4f68-a7ad-7d578c089f8b"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4324), "admin001", null, null, 1, null, null },
                    { new Guid("c9d66d2d-acf3-4f68-a7ad-7d578c089f8b"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4325), "admin001", null, null, 1, null, null },
                    { new Guid("cc82920a-ae3b-4a58-b16f-f5af1f73a9e7"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4584), "admin001", null, null, 1, null, null },
                    { new Guid("cc82920a-ae3b-4a58-b16f-f5af1f73a9e7"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4584), "admin001", null, null, 1, null, null },
                    { new Guid("cc82920a-ae3b-4a58-b16f-f5af1f73a9e7"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4585), "admin001", null, null, 1, null, null },
                    { new Guid("cc82920a-ae3b-4a58-b16f-f5af1f73a9e7"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4585), "admin001", null, null, 1, null, null },
                    { new Guid("cc82920a-ae3b-4a58-b16f-f5af1f73a9e7"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4586), "admin001", null, null, 1, null, null },
                    { new Guid("cc82920a-ae3b-4a58-b16f-f5af1f73a9e7"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4587), "admin001", null, null, 1, null, null },
                    { new Guid("cc82920a-ae3b-4a58-b16f-f5af1f73a9e7"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4587), "admin001", null, null, 1, null, null },
                    { new Guid("cc82920a-ae3b-4a58-b16f-f5af1f73a9e7"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4588), "admin001", null, null, 1, null, null },
                    { new Guid("ccc910aa-32d3-441c-a29f-05855bbcf6b6"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4834), "admin001", null, null, 1, null, null },
                    { new Guid("ccc910aa-32d3-441c-a29f-05855bbcf6b6"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4836), "admin001", null, null, 1, null, null },
                    { new Guid("ccc910aa-32d3-441c-a29f-05855bbcf6b6"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4836), "admin001", null, null, 1, null, null },
                    { new Guid("ccc910aa-32d3-441c-a29f-05855bbcf6b6"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4837), "admin001", null, null, 1, null, null },
                    { new Guid("ccc910aa-32d3-441c-a29f-05855bbcf6b6"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4838), "admin001", null, null, 1, null, null },
                    { new Guid("ccc910aa-32d3-441c-a29f-05855bbcf6b6"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4838), "admin001", null, null, 1, null, null },
                    { new Guid("ccc910aa-32d3-441c-a29f-05855bbcf6b6"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4839), "admin001", null, null, 1, null, null },
                    { new Guid("ccc910aa-32d3-441c-a29f-05855bbcf6b6"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4839), "admin001", null, null, 1, null, null },
                    { new Guid("d011b65e-f51b-424f-a579-c72fee84c12b"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4400), "admin001", null, null, 1, null, null },
                    { new Guid("d011b65e-f51b-424f-a579-c72fee84c12b"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4409), "admin001", null, null, 1, null, null },
                    { new Guid("d011b65e-f51b-424f-a579-c72fee84c12b"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4410), "admin001", null, null, 1, null, null },
                    { new Guid("d011b65e-f51b-424f-a579-c72fee84c12b"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4410), "admin001", null, null, 1, null, null },
                    { new Guid("d011b65e-f51b-424f-a579-c72fee84c12b"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4411), "admin001", null, null, 1, null, null },
                    { new Guid("d011b65e-f51b-424f-a579-c72fee84c12b"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4412), "admin001", null, null, 1, null, null },
                    { new Guid("d011b65e-f51b-424f-a579-c72fee84c12b"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4412), "admin001", null, null, 1, null, null },
                    { new Guid("d011b65e-f51b-424f-a579-c72fee84c12b"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4413), "admin001", null, null, 1, null, null },
                    { new Guid("d0b511fc-4a44-48a1-8a6d-db0c9f96aeeb"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5052), "admin001", null, null, 1, null, null },
                    { new Guid("d0b511fc-4a44-48a1-8a6d-db0c9f96aeeb"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5052), "admin001", null, null, 1, null, null },
                    { new Guid("d0b511fc-4a44-48a1-8a6d-db0c9f96aeeb"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5053), "admin001", null, null, 1, null, null },
                    { new Guid("d0b511fc-4a44-48a1-8a6d-db0c9f96aeeb"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5054), "admin001", null, null, 1, null, null },
                    { new Guid("d0b511fc-4a44-48a1-8a6d-db0c9f96aeeb"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5054), "admin001", null, null, 1, null, null },
                    { new Guid("d0b511fc-4a44-48a1-8a6d-db0c9f96aeeb"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5055), "admin001", null, null, 1, null, null },
                    { new Guid("d0b511fc-4a44-48a1-8a6d-db0c9f96aeeb"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5056), "admin001", null, null, 1, null, null },
                    { new Guid("d0b511fc-4a44-48a1-8a6d-db0c9f96aeeb"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5056), "admin001", null, null, 1, null, null },
                    { new Guid("d143bfa9-575c-4d80-8b39-e43972325e09"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4758), "admin001", null, null, 1, null, null },
                    { new Guid("d143bfa9-575c-4d80-8b39-e43972325e09"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4759), "admin001", null, null, 1, null, null },
                    { new Guid("d143bfa9-575c-4d80-8b39-e43972325e09"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4759), "admin001", null, null, 1, null, null },
                    { new Guid("d143bfa9-575c-4d80-8b39-e43972325e09"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4760), "admin001", null, null, 1, null, null },
                    { new Guid("d143bfa9-575c-4d80-8b39-e43972325e09"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4760), "admin001", null, null, 1, null, null },
                    { new Guid("d143bfa9-575c-4d80-8b39-e43972325e09"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4761), "admin001", null, null, 1, null, null },
                    { new Guid("d143bfa9-575c-4d80-8b39-e43972325e09"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4762), "admin001", null, null, 1, null, null },
                    { new Guid("d143bfa9-575c-4d80-8b39-e43972325e09"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4762), "admin001", null, null, 1, null, null },
                    { new Guid("d188b497-0aca-48c5-9d09-004575c66af4"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4856), "admin001", null, null, 1, null, null },
                    { new Guid("d188b497-0aca-48c5-9d09-004575c66af4"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4857), "admin001", null, null, 1, null, null },
                    { new Guid("d188b497-0aca-48c5-9d09-004575c66af4"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4857), "admin001", null, null, 1, null, null },
                    { new Guid("d188b497-0aca-48c5-9d09-004575c66af4"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4858), "admin001", null, null, 1, null, null },
                    { new Guid("d188b497-0aca-48c5-9d09-004575c66af4"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4858), "admin001", null, null, 1, null, null },
                    { new Guid("d188b497-0aca-48c5-9d09-004575c66af4"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4859), "admin001", null, null, 1, null, null },
                    { new Guid("d188b497-0aca-48c5-9d09-004575c66af4"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4860), "admin001", null, null, 1, null, null },
                    { new Guid("d188b497-0aca-48c5-9d09-004575c66af4"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4860), "admin001", null, null, 1, null, null },
                    { new Guid("d2e25264-a384-4383-8d15-2d97323a0ee3"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4464), "admin001", null, null, 1, null, null },
                    { new Guid("d2e25264-a384-4383-8d15-2d97323a0ee3"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4465), "admin001", null, null, 1, null, null },
                    { new Guid("d2e25264-a384-4383-8d15-2d97323a0ee3"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4465), "admin001", null, null, 1, null, null },
                    { new Guid("d2e25264-a384-4383-8d15-2d97323a0ee3"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4466), "admin001", null, null, 1, null, null },
                    { new Guid("d2e25264-a384-4383-8d15-2d97323a0ee3"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4467), "admin001", null, null, 1, null, null },
                    { new Guid("d2e25264-a384-4383-8d15-2d97323a0ee3"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4467), "admin001", null, null, 1, null, null },
                    { new Guid("d2e25264-a384-4383-8d15-2d97323a0ee3"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4468), "admin001", null, null, 1, null, null },
                    { new Guid("d2e25264-a384-4383-8d15-2d97323a0ee3"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4468), "admin001", null, null, 1, null, null },
                    { new Guid("d75d29d4-6fd8-4be5-8554-07153f4de0e8"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4573), "admin001", null, null, 1, null, null },
                    { new Guid("d75d29d4-6fd8-4be5-8554-07153f4de0e8"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4574), "admin001", null, null, 1, null, null },
                    { new Guid("d75d29d4-6fd8-4be5-8554-07153f4de0e8"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4575), "admin001", null, null, 1, null, null },
                    { new Guid("d75d29d4-6fd8-4be5-8554-07153f4de0e8"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4575), "admin001", null, null, 1, null, null },
                    { new Guid("d75d29d4-6fd8-4be5-8554-07153f4de0e8"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4576), "admin001", null, null, 1, null, null },
                    { new Guid("d75d29d4-6fd8-4be5-8554-07153f4de0e8"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4576), "admin001", null, null, 1, null, null },
                    { new Guid("d75d29d4-6fd8-4be5-8554-07153f4de0e8"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4577), "admin001", null, null, 1, null, null },
                    { new Guid("d75d29d4-6fd8-4be5-8554-07153f4de0e8"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4578), "admin001", null, null, 1, null, null },
                    { new Guid("d88f257b-f579-46f8-8a8a-181addb8a338"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4702), "admin001", null, null, 1, null, null },
                    { new Guid("d88f257b-f579-46f8-8a8a-181addb8a338"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4703), "admin001", null, null, 1, null, null },
                    { new Guid("d88f257b-f579-46f8-8a8a-181addb8a338"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4704), "admin001", null, null, 1, null, null },
                    { new Guid("d88f257b-f579-46f8-8a8a-181addb8a338"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4704), "admin001", null, null, 1, null, null },
                    { new Guid("d88f257b-f579-46f8-8a8a-181addb8a338"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4705), "admin001", null, null, 1, null, null },
                    { new Guid("d88f257b-f579-46f8-8a8a-181addb8a338"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4706), "admin001", null, null, 1, null, null },
                    { new Guid("d88f257b-f579-46f8-8a8a-181addb8a338"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4706), "admin001", null, null, 1, null, null },
                    { new Guid("d88f257b-f579-46f8-8a8a-181addb8a338"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4707), "admin001", null, null, 1, null, null },
                    { new Guid("d902a254-bdc4-42b5-8127-72b196b3e585"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4485), "admin001", null, null, 1, null, null },
                    { new Guid("d902a254-bdc4-42b5-8127-72b196b3e585"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4485), "admin001", null, null, 1, null, null },
                    { new Guid("d902a254-bdc4-42b5-8127-72b196b3e585"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4486), "admin001", null, null, 1, null, null },
                    { new Guid("d902a254-bdc4-42b5-8127-72b196b3e585"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4486), "admin001", null, null, 1, null, null },
                    { new Guid("d902a254-bdc4-42b5-8127-72b196b3e585"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4487), "admin001", null, null, 1, null, null },
                    { new Guid("d902a254-bdc4-42b5-8127-72b196b3e585"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4488), "admin001", null, null, 1, null, null },
                    { new Guid("d902a254-bdc4-42b5-8127-72b196b3e585"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4488), "admin001", null, null, 1, null, null },
                    { new Guid("d902a254-bdc4-42b5-8127-72b196b3e585"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4500), "admin001", null, null, 1, null, null },
                    { new Guid("da411a92-ed81-4853-937f-f2484df044ed"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4599), "admin001", null, null, 1, null, null },
                    { new Guid("da411a92-ed81-4853-937f-f2484df044ed"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4600), "admin001", null, null, 1, null, null },
                    { new Guid("da411a92-ed81-4853-937f-f2484df044ed"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4600), "admin001", null, null, 1, null, null },
                    { new Guid("da411a92-ed81-4853-937f-f2484df044ed"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4601), "admin001", null, null, 1, null, null },
                    { new Guid("da411a92-ed81-4853-937f-f2484df044ed"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4601), "admin001", null, null, 1, null, null },
                    { new Guid("da411a92-ed81-4853-937f-f2484df044ed"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4602), "admin001", null, null, 1, null, null },
                    { new Guid("da411a92-ed81-4853-937f-f2484df044ed"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4603), "admin001", null, null, 1, null, null },
                    { new Guid("da411a92-ed81-4853-937f-f2484df044ed"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4611), "admin001", null, null, 1, null, null },
                    { new Guid("daa57055-5788-41d6-a9d6-c765b55f974b"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4936), "admin001", null, null, 1, null, null },
                    { new Guid("daa57055-5788-41d6-a9d6-c765b55f974b"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4937), "admin001", null, null, 1, null, null },
                    { new Guid("daa57055-5788-41d6-a9d6-c765b55f974b"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4938), "admin001", null, null, 1, null, null },
                    { new Guid("daa57055-5788-41d6-a9d6-c765b55f974b"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4938), "admin001", null, null, 1, null, null },
                    { new Guid("daa57055-5788-41d6-a9d6-c765b55f974b"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4939), "admin001", null, null, 1, null, null },
                    { new Guid("daa57055-5788-41d6-a9d6-c765b55f974b"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4940), "admin001", null, null, 1, null, null },
                    { new Guid("daa57055-5788-41d6-a9d6-c765b55f974b"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4940), "admin001", null, null, 1, null, null },
                    { new Guid("daa57055-5788-41d6-a9d6-c765b55f974b"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4941), "admin001", null, null, 1, null, null },
                    { new Guid("dafeacb2-ae96-47d8-9c08-ed79d6b31b05"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4965), "admin001", null, null, 1, null, null },
                    { new Guid("dafeacb2-ae96-47d8-9c08-ed79d6b31b05"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4966), "admin001", null, null, 1, null, null },
                    { new Guid("dafeacb2-ae96-47d8-9c08-ed79d6b31b05"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4966), "admin001", null, null, 1, null, null },
                    { new Guid("dafeacb2-ae96-47d8-9c08-ed79d6b31b05"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4967), "admin001", null, null, 1, null, null },
                    { new Guid("dafeacb2-ae96-47d8-9c08-ed79d6b31b05"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4968), "admin001", null, null, 1, null, null },
                    { new Guid("dafeacb2-ae96-47d8-9c08-ed79d6b31b05"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4968), "admin001", null, null, 1, null, null },
                    { new Guid("dafeacb2-ae96-47d8-9c08-ed79d6b31b05"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4969), "admin001", null, null, 1, null, null },
                    { new Guid("dafeacb2-ae96-47d8-9c08-ed79d6b31b05"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4970), "admin001", null, null, 1, null, null },
                    { new Guid("dafec166-b3c2-4241-b264-fba119ede85c"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4981), "admin001", null, null, 1, null, null },
                    { new Guid("dafec166-b3c2-4241-b264-fba119ede85c"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4981), "admin001", null, null, 1, null, null },
                    { new Guid("dafec166-b3c2-4241-b264-fba119ede85c"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4982), "admin001", null, null, 1, null, null },
                    { new Guid("dafec166-b3c2-4241-b264-fba119ede85c"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4982), "admin001", null, null, 1, null, null },
                    { new Guid("dafec166-b3c2-4241-b264-fba119ede85c"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4983), "admin001", null, null, 1, null, null },
                    { new Guid("dafec166-b3c2-4241-b264-fba119ede85c"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4990), "admin001", null, null, 1, null, null },
                    { new Guid("dafec166-b3c2-4241-b264-fba119ede85c"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4991), "admin001", null, null, 1, null, null },
                    { new Guid("dafec166-b3c2-4241-b264-fba119ede85c"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4992), "admin001", null, null, 1, null, null },
                    { new Guid("db81778d-7e8a-4da3-9ce6-78fd3d261ff1"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4669), "admin001", null, null, 1, null, null },
                    { new Guid("db81778d-7e8a-4da3-9ce6-78fd3d261ff1"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4669), "admin001", null, null, 1, null, null },
                    { new Guid("db81778d-7e8a-4da3-9ce6-78fd3d261ff1"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4670), "admin001", null, null, 1, null, null },
                    { new Guid("db81778d-7e8a-4da3-9ce6-78fd3d261ff1"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4671), "admin001", null, null, 1, null, null },
                    { new Guid("db81778d-7e8a-4da3-9ce6-78fd3d261ff1"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4671), "admin001", null, null, 1, null, null },
                    { new Guid("db81778d-7e8a-4da3-9ce6-78fd3d261ff1"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4672), "admin001", null, null, 1, null, null },
                    { new Guid("db81778d-7e8a-4da3-9ce6-78fd3d261ff1"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4672), "admin001", null, null, 1, null, null },
                    { new Guid("db81778d-7e8a-4da3-9ce6-78fd3d261ff1"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4673), "admin001", null, null, 1, null, null },
                    { new Guid("dc1246db-98cc-41cd-915a-93adc3bf49fd"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4469), "admin001", null, null, 1, null, null },
                    { new Guid("dc1246db-98cc-41cd-915a-93adc3bf49fd"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4470), "admin001", null, null, 1, null, null },
                    { new Guid("dc1246db-98cc-41cd-915a-93adc3bf49fd"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4470), "admin001", null, null, 1, null, null },
                    { new Guid("dc1246db-98cc-41cd-915a-93adc3bf49fd"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4471), "admin001", null, null, 1, null, null },
                    { new Guid("dc1246db-98cc-41cd-915a-93adc3bf49fd"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4472), "admin001", null, null, 1, null, null },
                    { new Guid("dc1246db-98cc-41cd-915a-93adc3bf49fd"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4472), "admin001", null, null, 1, null, null },
                    { new Guid("dc1246db-98cc-41cd-915a-93adc3bf49fd"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4473), "admin001", null, null, 1, null, null },
                    { new Guid("dc1246db-98cc-41cd-915a-93adc3bf49fd"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4474), "admin001", null, null, 1, null, null },
                    { new Guid("de84f2a8-9446-4cf5-b61c-195cefb1c655"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4921), "admin001", null, null, 1, null, null },
                    { new Guid("de84f2a8-9446-4cf5-b61c-195cefb1c655"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4922), "admin001", null, null, 1, null, null },
                    { new Guid("de84f2a8-9446-4cf5-b61c-195cefb1c655"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4922), "admin001", null, null, 1, null, null },
                    { new Guid("de84f2a8-9446-4cf5-b61c-195cefb1c655"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4923), "admin001", null, null, 1, null, null },
                    { new Guid("de84f2a8-9446-4cf5-b61c-195cefb1c655"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4924), "admin001", null, null, 1, null, null },
                    { new Guid("de84f2a8-9446-4cf5-b61c-195cefb1c655"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4924), "admin001", null, null, 1, null, null },
                    { new Guid("de84f2a8-9446-4cf5-b61c-195cefb1c655"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4925), "admin001", null, null, 1, null, null },
                    { new Guid("de84f2a8-9446-4cf5-b61c-195cefb1c655"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4925), "admin001", null, null, 1, null, null },
                    { new Guid("e1397403-dfe9-44ed-9777-c653de54fb73"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4355), "admin001", null, null, 1, null, null },
                    { new Guid("e1397403-dfe9-44ed-9777-c653de54fb73"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4355), "admin001", null, null, 1, null, null },
                    { new Guid("e1397403-dfe9-44ed-9777-c653de54fb73"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4356), "admin001", null, null, 1, null, null },
                    { new Guid("e1397403-dfe9-44ed-9777-c653de54fb73"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4356), "admin001", null, null, 1, null, null },
                    { new Guid("e1397403-dfe9-44ed-9777-c653de54fb73"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4357), "admin001", null, null, 1, null, null },
                    { new Guid("e1397403-dfe9-44ed-9777-c653de54fb73"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4358), "admin001", null, null, 1, null, null },
                    { new Guid("e1397403-dfe9-44ed-9777-c653de54fb73"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4358), "admin001", null, null, 1, null, null },
                    { new Guid("e1397403-dfe9-44ed-9777-c653de54fb73"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4359), "admin001", null, null, 1, null, null },
                    { new Guid("e2d8be78-06fa-4fcc-9337-a7161805ec41"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4501), "admin001", null, null, 1, null, null },
                    { new Guid("e2d8be78-06fa-4fcc-9337-a7161805ec41"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4502), "admin001", null, null, 1, null, null },
                    { new Guid("e2d8be78-06fa-4fcc-9337-a7161805ec41"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4502), "admin001", null, null, 1, null, null },
                    { new Guid("e2d8be78-06fa-4fcc-9337-a7161805ec41"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4503), "admin001", null, null, 1, null, null },
                    { new Guid("e2d8be78-06fa-4fcc-9337-a7161805ec41"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4504), "admin001", null, null, 1, null, null },
                    { new Guid("e2d8be78-06fa-4fcc-9337-a7161805ec41"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4504), "admin001", null, null, 1, null, null },
                    { new Guid("e2d8be78-06fa-4fcc-9337-a7161805ec41"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4505), "admin001", null, null, 1, null, null },
                    { new Guid("e2d8be78-06fa-4fcc-9337-a7161805ec41"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4505), "admin001", null, null, 1, null, null },
                    { new Guid("e3f24681-c4be-48e0-aee5-b182d685eb82"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4326), "admin001", null, null, 1, null, null },
                    { new Guid("e3f24681-c4be-48e0-aee5-b182d685eb82"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4326), "admin001", null, null, 1, null, null },
                    { new Guid("e3f24681-c4be-48e0-aee5-b182d685eb82"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4327), "admin001", null, null, 1, null, null },
                    { new Guid("e3f24681-c4be-48e0-aee5-b182d685eb82"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4328), "admin001", null, null, 1, null, null },
                    { new Guid("e3f24681-c4be-48e0-aee5-b182d685eb82"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4328), "admin001", null, null, 1, null, null },
                    { new Guid("e3f24681-c4be-48e0-aee5-b182d685eb82"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4329), "admin001", null, null, 1, null, null },
                    { new Guid("e3f24681-c4be-48e0-aee5-b182d685eb82"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4336), "admin001", null, null, 1, null, null },
                    { new Guid("e3f24681-c4be-48e0-aee5-b182d685eb82"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4337), "admin001", null, null, 1, null, null },
                    { new Guid("e3f7bc6d-6d2f-4b0c-b6a2-0587f14e6a9f"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4811), "admin001", null, null, 1, null, null },
                    { new Guid("e3f7bc6d-6d2f-4b0c-b6a2-0587f14e6a9f"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4812), "admin001", null, null, 1, null, null },
                    { new Guid("e3f7bc6d-6d2f-4b0c-b6a2-0587f14e6a9f"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4813), "admin001", null, null, 1, null, null },
                    { new Guid("e3f7bc6d-6d2f-4b0c-b6a2-0587f14e6a9f"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4813), "admin001", null, null, 1, null, null },
                    { new Guid("e3f7bc6d-6d2f-4b0c-b6a2-0587f14e6a9f"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4814), "admin001", null, null, 1, null, null },
                    { new Guid("e3f7bc6d-6d2f-4b0c-b6a2-0587f14e6a9f"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4814), "admin001", null, null, 1, null, null },
                    { new Guid("e3f7bc6d-6d2f-4b0c-b6a2-0587f14e6a9f"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4815), "admin001", null, null, 1, null, null },
                    { new Guid("e3f7bc6d-6d2f-4b0c-b6a2-0587f14e6a9f"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4816), "admin001", null, null, 1, null, null },
                    { new Guid("e53bcecc-8e4f-4d89-8421-d007352456c7"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4479), "admin001", null, null, 1, null, null },
                    { new Guid("e53bcecc-8e4f-4d89-8421-d007352456c7"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4480), "admin001", null, null, 1, null, null },
                    { new Guid("e53bcecc-8e4f-4d89-8421-d007352456c7"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4481), "admin001", null, null, 1, null, null },
                    { new Guid("e53bcecc-8e4f-4d89-8421-d007352456c7"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4481), "admin001", null, null, 1, null, null },
                    { new Guid("e53bcecc-8e4f-4d89-8421-d007352456c7"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4482), "admin001", null, null, 1, null, null },
                    { new Guid("e53bcecc-8e4f-4d89-8421-d007352456c7"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4483), "admin001", null, null, 1, null, null },
                    { new Guid("e53bcecc-8e4f-4d89-8421-d007352456c7"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4483), "admin001", null, null, 1, null, null },
                    { new Guid("e53bcecc-8e4f-4d89-8421-d007352456c7"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4484), "admin001", null, null, 1, null, null },
                    { new Guid("e7c1e5b9-7038-49bc-84ba-4ad0892be078"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5081), "admin001", null, null, 1, null, null },
                    { new Guid("e7c1e5b9-7038-49bc-84ba-4ad0892be078"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5081), "admin001", null, null, 1, null, null },
                    { new Guid("e7c1e5b9-7038-49bc-84ba-4ad0892be078"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5082), "admin001", null, null, 1, null, null },
                    { new Guid("e7c1e5b9-7038-49bc-84ba-4ad0892be078"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5083), "admin001", null, null, 1, null, null },
                    { new Guid("e7c1e5b9-7038-49bc-84ba-4ad0892be078"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5083), "admin001", null, null, 1, null, null },
                    { new Guid("e7c1e5b9-7038-49bc-84ba-4ad0892be078"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5084), "admin001", null, null, 1, null, null },
                    { new Guid("e7c1e5b9-7038-49bc-84ba-4ad0892be078"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5084), "admin001", null, null, 1, null, null },
                    { new Guid("e7c1e5b9-7038-49bc-84ba-4ad0892be078"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(5085), "admin001", null, null, 1, null, null },
                    { new Guid("e8a2a9e8-6c2b-490d-91ec-f796a50615f8"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4414), "admin001", null, null, 1, null, null },
                    { new Guid("e8a2a9e8-6c2b-490d-91ec-f796a50615f8"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4414), "admin001", null, null, 1, null, null },
                    { new Guid("e8a2a9e8-6c2b-490d-91ec-f796a50615f8"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4415), "admin001", null, null, 1, null, null },
                    { new Guid("e8a2a9e8-6c2b-490d-91ec-f796a50615f8"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4416), "admin001", null, null, 1, null, null },
                    { new Guid("e8a2a9e8-6c2b-490d-91ec-f796a50615f8"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4416), "admin001", null, null, 1, null, null },
                    { new Guid("e8a2a9e8-6c2b-490d-91ec-f796a50615f8"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4417), "admin001", null, null, 1, null, null },
                    { new Guid("e8a2a9e8-6c2b-490d-91ec-f796a50615f8"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4417), "admin001", null, null, 1, null, null },
                    { new Guid("e8a2a9e8-6c2b-490d-91ec-f796a50615f8"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4418), "admin001", null, null, 1, null, null },
                    { new Guid("e916b054-c7c8-4fcb-b482-f0f81196f947"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4718), "admin001", null, null, 1, null, null },
                    { new Guid("e916b054-c7c8-4fcb-b482-f0f81196f947"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4729), "admin001", null, null, 1, null, null },
                    { new Guid("e916b054-c7c8-4fcb-b482-f0f81196f947"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4730), "admin001", null, null, 1, null, null },
                    { new Guid("e916b054-c7c8-4fcb-b482-f0f81196f947"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4731), "admin001", null, null, 1, null, null },
                    { new Guid("e916b054-c7c8-4fcb-b482-f0f81196f947"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4731), "admin001", null, null, 1, null, null },
                    { new Guid("e916b054-c7c8-4fcb-b482-f0f81196f947"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4732), "admin001", null, null, 1, null, null },
                    { new Guid("e916b054-c7c8-4fcb-b482-f0f81196f947"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4732), "admin001", null, null, 1, null, null },
                    { new Guid("e916b054-c7c8-4fcb-b482-f0f81196f947"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4733), "admin001", null, null, 1, null, null },
                    { new Guid("e97936ed-5bce-4f87-9601-eb125b6af1a5"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4344), "admin001", null, null, 1, null, null },
                    { new Guid("e97936ed-5bce-4f87-9601-eb125b6af1a5"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4345), "admin001", null, null, 1, null, null },
                    { new Guid("e97936ed-5bce-4f87-9601-eb125b6af1a5"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4346), "admin001", null, null, 1, null, null },
                    { new Guid("e97936ed-5bce-4f87-9601-eb125b6af1a5"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4346), "admin001", null, null, 1, null, null },
                    { new Guid("e97936ed-5bce-4f87-9601-eb125b6af1a5"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4347), "admin001", null, null, 1, null, null },
                    { new Guid("e97936ed-5bce-4f87-9601-eb125b6af1a5"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4347), "admin001", null, null, 1, null, null },
                    { new Guid("e97936ed-5bce-4f87-9601-eb125b6af1a5"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4348), "admin001", null, null, 1, null, null },
                    { new Guid("e97936ed-5bce-4f87-9601-eb125b6af1a5"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4349), "admin001", null, null, 1, null, null },
                    { new Guid("eda90344-680d-442b-903d-2d210f59e110"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4594), "admin001", null, null, 1, null, null },
                    { new Guid("eda90344-680d-442b-903d-2d210f59e110"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4594), "admin001", null, null, 1, null, null },
                    { new Guid("eda90344-680d-442b-903d-2d210f59e110"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4595), "admin001", null, null, 1, null, null },
                    { new Guid("eda90344-680d-442b-903d-2d210f59e110"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4596), "admin001", null, null, 1, null, null },
                    { new Guid("eda90344-680d-442b-903d-2d210f59e110"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4596), "admin001", null, null, 1, null, null },
                    { new Guid("eda90344-680d-442b-903d-2d210f59e110"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4597), "admin001", null, null, 1, null, null },
                    { new Guid("eda90344-680d-442b-903d-2d210f59e110"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4598), "admin001", null, null, 1, null, null },
                    { new Guid("eda90344-680d-442b-903d-2d210f59e110"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4598), "admin001", null, null, 1, null, null },
                    { new Guid("edb2f1f6-b93d-49bb-8fb1-04d9796238cb"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4338), "admin001", null, null, 1, null, null },
                    { new Guid("edb2f1f6-b93d-49bb-8fb1-04d9796238cb"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4340), "admin001", null, null, 1, null, null },
                    { new Guid("edb2f1f6-b93d-49bb-8fb1-04d9796238cb"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4340), "admin001", null, null, 1, null, null },
                    { new Guid("edb2f1f6-b93d-49bb-8fb1-04d9796238cb"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4341), "admin001", null, null, 1, null, null },
                    { new Guid("edb2f1f6-b93d-49bb-8fb1-04d9796238cb"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4342), "admin001", null, null, 1, null, null },
                    { new Guid("edb2f1f6-b93d-49bb-8fb1-04d9796238cb"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4342), "admin001", null, null, 1, null, null },
                    { new Guid("edb2f1f6-b93d-49bb-8fb1-04d9796238cb"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4343), "admin001", null, null, 1, null, null },
                    { new Guid("edb2f1f6-b93d-49bb-8fb1-04d9796238cb"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4344), "admin001", null, null, 1, null, null },
                    { new Guid("ee3261b3-18e4-48ca-80cd-5f6e8fb49195"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4768), "admin001", null, null, 1, null, null },
                    { new Guid("ee3261b3-18e4-48ca-80cd-5f6e8fb49195"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4769), "admin001", null, null, 1, null, null },
                    { new Guid("ee3261b3-18e4-48ca-80cd-5f6e8fb49195"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4769), "admin001", null, null, 1, null, null },
                    { new Guid("ee3261b3-18e4-48ca-80cd-5f6e8fb49195"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4770), "admin001", null, null, 1, null, null },
                    { new Guid("ee3261b3-18e4-48ca-80cd-5f6e8fb49195"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4771), "admin001", null, null, 1, null, null },
                    { new Guid("ee3261b3-18e4-48ca-80cd-5f6e8fb49195"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4771), "admin001", null, null, 1, null, null },
                    { new Guid("ee3261b3-18e4-48ca-80cd-5f6e8fb49195"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4772), "admin001", null, null, 1, null, null },
                    { new Guid("ee3261b3-18e4-48ca-80cd-5f6e8fb49195"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4773), "admin001", null, null, 1, null, null },
                    { new Guid("f129d533-7c12-4779-a40b-9614ddb98eaa"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4297), "admin001", null, null, 1, null, null },
                    { new Guid("f129d533-7c12-4779-a40b-9614ddb98eaa"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4298), "admin001", null, null, 1, null, null },
                    { new Guid("f129d533-7c12-4779-a40b-9614ddb98eaa"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4299), "admin001", null, null, 1, null, null },
                    { new Guid("f129d533-7c12-4779-a40b-9614ddb98eaa"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4300), "admin001", null, null, 1, null, null },
                    { new Guid("f129d533-7c12-4779-a40b-9614ddb98eaa"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4301), "admin001", null, null, 1, null, null },
                    { new Guid("f129d533-7c12-4779-a40b-9614ddb98eaa"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4301), "admin001", null, null, 1, null, null },
                    { new Guid("f129d533-7c12-4779-a40b-9614ddb98eaa"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4302), "admin001", null, null, 1, null, null },
                    { new Guid("f129d533-7c12-4779-a40b-9614ddb98eaa"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4303), "admin001", null, null, 1, null, null },
                    { new Guid("f5d2e9b4-3b91-4761-b695-02a2d39275b5"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4959), "admin001", null, null, 1, null, null },
                    { new Guid("f5d2e9b4-3b91-4761-b695-02a2d39275b5"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4960), "admin001", null, null, 1, null, null },
                    { new Guid("f5d2e9b4-3b91-4761-b695-02a2d39275b5"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4961), "admin001", null, null, 1, null, null },
                    { new Guid("f5d2e9b4-3b91-4761-b695-02a2d39275b5"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4962), "admin001", null, null, 1, null, null },
                    { new Guid("f5d2e9b4-3b91-4761-b695-02a2d39275b5"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4962), "admin001", null, null, 1, null, null },
                    { new Guid("f5d2e9b4-3b91-4761-b695-02a2d39275b5"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4963), "admin001", null, null, 1, null, null },
                    { new Guid("f5d2e9b4-3b91-4761-b695-02a2d39275b5"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4964), "admin001", null, null, 1, null, null },
                    { new Guid("f5d2e9b4-3b91-4761-b695-02a2d39275b5"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4964), "admin001", null, null, 1, null, null },
                    { new Guid("f68aed0b-a60e-4c64-882a-16e429d5927b"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4378), "admin001", null, null, 1, null, null },
                    { new Guid("f68aed0b-a60e-4c64-882a-16e429d5927b"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4379), "admin001", null, null, 1, null, null },
                    { new Guid("f68aed0b-a60e-4c64-882a-16e429d5927b"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4380), "admin001", null, null, 1, null, null },
                    { new Guid("f68aed0b-a60e-4c64-882a-16e429d5927b"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4380), "admin001", null, null, 1, null, null },
                    { new Guid("f68aed0b-a60e-4c64-882a-16e429d5927b"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4381), "admin001", null, null, 1, null, null },
                    { new Guid("f68aed0b-a60e-4c64-882a-16e429d5927b"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4382), "admin001", null, null, 1, null, null },
                    { new Guid("f68aed0b-a60e-4c64-882a-16e429d5927b"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4382), "admin001", null, null, 1, null, null },
                    { new Guid("f68aed0b-a60e-4c64-882a-16e429d5927b"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4383), "admin001", null, null, 1, null, null },
                    { new Guid("f81437d4-be69-4218-b195-d6eb07835a80"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4533), "admin001", null, null, 1, null, null },
                    { new Guid("f81437d4-be69-4218-b195-d6eb07835a80"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4534), "admin001", null, null, 1, null, null },
                    { new Guid("f81437d4-be69-4218-b195-d6eb07835a80"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4534), "admin001", null, null, 1, null, null },
                    { new Guid("f81437d4-be69-4218-b195-d6eb07835a80"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4535), "admin001", null, null, 1, null, null },
                    { new Guid("f81437d4-be69-4218-b195-d6eb07835a80"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4536), "admin001", null, null, 1, null, null },
                    { new Guid("f81437d4-be69-4218-b195-d6eb07835a80"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4536), "admin001", null, null, 1, null, null },
                    { new Guid("f81437d4-be69-4218-b195-d6eb07835a80"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4537), "admin001", null, null, 1, null, null },
                    { new Guid("f81437d4-be69-4218-b195-d6eb07835a80"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4538), "admin001", null, null, 1, null, null },
                    { new Guid("fa9db598-0c8c-4027-a24d-84b66cb0c584"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4589), "admin001", null, null, 1, null, null },
                    { new Guid("fa9db598-0c8c-4027-a24d-84b66cb0c584"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4589), "admin001", null, null, 1, null, null },
                    { new Guid("fa9db598-0c8c-4027-a24d-84b66cb0c584"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4590), "admin001", null, null, 1, null, null },
                    { new Guid("fa9db598-0c8c-4027-a24d-84b66cb0c584"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4590), "admin001", null, null, 1, null, null },
                    { new Guid("fa9db598-0c8c-4027-a24d-84b66cb0c584"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4591), "admin001", null, null, 1, null, null },
                    { new Guid("fa9db598-0c8c-4027-a24d-84b66cb0c584"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4592), "admin001", null, null, 1, null, null },
                    { new Guid("fa9db598-0c8c-4027-a24d-84b66cb0c584"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4592), "admin001", null, null, 1, null, null },
                    { new Guid("fa9db598-0c8c-4027-a24d-84b66cb0c584"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4593), "admin001", null, null, 1, null, null },
                    { new Guid("fb256ab8-108b-405a-bf0b-79a87c95567b"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4420), "admin001", null, null, 1, null, null },
                    { new Guid("fb256ab8-108b-405a-bf0b-79a87c95567b"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4421), "admin001", null, null, 1, null, null },
                    { new Guid("fb256ab8-108b-405a-bf0b-79a87c95567b"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4421), "admin001", null, null, 1, null, null },
                    { new Guid("fb256ab8-108b-405a-bf0b-79a87c95567b"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4422), "admin001", null, null, 1, null, null },
                    { new Guid("fb256ab8-108b-405a-bf0b-79a87c95567b"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4423), "admin001", null, null, 1, null, null },
                    { new Guid("fb256ab8-108b-405a-bf0b-79a87c95567b"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4423), "admin001", null, null, 1, null, null },
                    { new Guid("fb256ab8-108b-405a-bf0b-79a87c95567b"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4424), "admin001", null, null, 1, null, null },
                    { new Guid("fb256ab8-108b-405a-bf0b-79a87c95567b"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4425), "admin001", null, null, 1, null, null },
                    { new Guid("fbac33ea-3bad-4271-9112-893420674318"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4851), "admin001", null, null, 1, null, null },
                    { new Guid("fbac33ea-3bad-4271-9112-893420674318"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4851), "admin001", null, null, 1, null, null },
                    { new Guid("fbac33ea-3bad-4271-9112-893420674318"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4852), "admin001", null, null, 1, null, null },
                    { new Guid("fbac33ea-3bad-4271-9112-893420674318"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4853), "admin001", null, null, 1, null, null },
                    { new Guid("fbac33ea-3bad-4271-9112-893420674318"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4853), "admin001", null, null, 1, null, null },
                    { new Guid("fbac33ea-3bad-4271-9112-893420674318"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4854), "admin001", null, null, 1, null, null },
                    { new Guid("fbac33ea-3bad-4271-9112-893420674318"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4854), "admin001", null, null, 1, null, null },
                    { new Guid("fbac33ea-3bad-4271-9112-893420674318"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4855), "admin001", null, null, 1, null, null },
                    { new Guid("fd1ae84e-81bf-436c-9659-17e28251e71a"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4633), "admin001", null, null, 1, null, null },
                    { new Guid("fd1ae84e-81bf-436c-9659-17e28251e71a"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4633), "admin001", null, null, 1, null, null },
                    { new Guid("fd1ae84e-81bf-436c-9659-17e28251e71a"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4634), "admin001", null, null, 1, null, null },
                    { new Guid("fd1ae84e-81bf-436c-9659-17e28251e71a"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4634), "admin001", null, null, 1, null, null },
                    { new Guid("fd1ae84e-81bf-436c-9659-17e28251e71a"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4635), "admin001", null, null, 1, null, null },
                    { new Guid("fd1ae84e-81bf-436c-9659-17e28251e71a"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4636), "admin001", null, null, 1, null, null },
                    { new Guid("fd1ae84e-81bf-436c-9659-17e28251e71a"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4636), "admin001", null, null, 1, null, null },
                    { new Guid("fd1ae84e-81bf-436c-9659-17e28251e71a"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4637), "admin001", null, null, 1, null, null },
                    { new Guid("fd910afd-b100-4fd9-822a-36b0d1c9f731"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4739), "admin001", null, null, 1, null, null },
                    { new Guid("fd910afd-b100-4fd9-822a-36b0d1c9f731"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4748), "admin001", null, null, 1, null, null },
                    { new Guid("fd910afd-b100-4fd9-822a-36b0d1c9f731"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4749), "admin001", null, null, 1, null, null },
                    { new Guid("fd910afd-b100-4fd9-822a-36b0d1c9f731"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4749), "admin001", null, null, 1, null, null },
                    { new Guid("fd910afd-b100-4fd9-822a-36b0d1c9f731"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4750), "admin001", null, null, 1, null, null },
                    { new Guid("fd910afd-b100-4fd9-822a-36b0d1c9f731"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4751), "admin001", null, null, 1, null, null },
                    { new Guid("fd910afd-b100-4fd9-822a-36b0d1c9f731"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4751), "admin001", null, null, 1, null, null },
                    { new Guid("fd910afd-b100-4fd9-822a-36b0d1c9f731"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4752), "admin001", null, null, 1, null, null },
                    { new Guid("fe0f508b-7377-44ed-be42-a70a95468050"), "learner001", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4791), "admin001", null, null, 1, null, null },
                    { new Guid("fe0f508b-7377-44ed-be42-a70a95468050"), "learner002", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4791), "admin001", null, null, 1, null, null },
                    { new Guid("fe0f508b-7377-44ed-be42-a70a95468050"), "learner003", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4792), "admin001", null, null, 1, null, null },
                    { new Guid("fe0f508b-7377-44ed-be42-a70a95468050"), "learner004", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4793), "admin001", null, null, 1, null, null },
                    { new Guid("fe0f508b-7377-44ed-be42-a70a95468050"), "learner005", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4793), "admin001", null, null, 1, null, null },
                    { new Guid("fe0f508b-7377-44ed-be42-a70a95468050"), "learner006", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4794), "admin001", null, null, 1, null, null },
                    { new Guid("fe0f508b-7377-44ed-be42-a70a95468050"), "learner007", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4794), "admin001", null, null, 1, null, null },
                    { new Guid("fe0f508b-7377-44ed-be42-a70a95468050"), "learner008", 0, new DateTime(2025, 2, 3, 9, 32, 56, 744, DateTimeKind.Utc).AddTicks(4795), "admin001", null, null, 1, null, null }
                });

            migrationBuilder.InsertData(
                table: "EntranceTestResult",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "CriteriaId", "CriteriaName", "DeletedAt", "DeletedById", "EntranceTestStudentId", "RecordStatus", "Score", "UpdateById", "UpdatedAt" },
                values: new object[] { new Guid("db568e73-f274-4ca1-ab63-1fdcc8b3b271"), new DateTime(2025, 2, 3, 9, 32, 56, 743, DateTimeKind.Utc).AddTicks(7515), "admin001", new Guid("8455165b-eae2-4132-803a-7c192390d44e"), null, null, null, new Guid("363853bc-8134-4ff0-b017-7b920d916555"), 1, null, null, null });

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
