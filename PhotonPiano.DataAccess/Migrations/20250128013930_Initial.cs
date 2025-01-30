using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

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
                    StudentStatus = table.Column<int>(type: "integer", nullable: true),
                    DesiredLevel = table.Column<string>(type: "text", nullable: true),
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
                    ReceiverFirebaseIds = table.Column<List<string>>(type: "text[]", nullable: false),
                    IsViewed = table.Column<bool>(type: "boolean", nullable: false),
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
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsFinished = table.Column<bool>(type: "boolean", nullable: false),
                    IsScorePublished = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(30)", nullable: false),
                    UpdateById = table.Column<string>(type: "character varying(30)", nullable: true),
                    DeletedById = table.Column<string>(type: "character varying(30)", nullable: true),
                    CertificateUrl = table.Column<string>(type: "text", nullable: true),
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
                    IsOpen = table.Column<bool>(type: "boolean", nullable: false),
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
                    ClassId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    Score = table.Column<decimal>(type: "numeric", nullable: false),
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
                    AttemptStatus = table.Column<int>(type: "integer", nullable: false),
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
