using Microsoft.EntityFrameworkCore;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.SeedData;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        #region Account Model

        const string admin001 = "admin001";
        const string teacher002 = "teacher002";
        const string learner003 = "learner003";

        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                AccountFirebaseId = admin001,
                Email = "admin001@gmail.com",
                Role = Role.Administrator,
                UserName = admin001
            },
            new Account
            {
                AccountFirebaseId = teacher002,
                Email = "teacher002@gmail.com",
                Role = Role.Instructor,
                UserName = teacher002
            },
            new Account
            {
                AccountFirebaseId = learner003,
                Email = "learner003@gmail.com",
                Role = Role.Student,
                UserName = learner003
            },
            new Account
            {
                AccountFirebaseId = "gnRssA2sZHWnXB23oUuUxwz95Ln1",
                Email = "staff123@gmail.com",
                Role = Role.Staff,
                UserName = "staff 123",
            }
        );

        #endregion

        #region Criterial Model

        var criteriaAccuracy = Guid.NewGuid();
        modelBuilder.Entity<Criteria>().HasData(
            // For EntranceTest
            new Criteria
            {
                Id = Guid.NewGuid(),
                Name = "Nhịp điệu",
                Weight = 10,
                For = CriteriaFor.EntranceTest,
                CreatedById = admin001
            },
            new Criteria
            {
                Id = criteriaAccuracy,
                Name = "Độ chính xác",
                Weight = 10,
                For = CriteriaFor.EntranceTest,
                CreatedById = admin001
            },
            new Criteria
            {
                Id = Guid.NewGuid(),
                Name = "Âm Sắc",
                Weight = 10,
                For = CriteriaFor.EntranceTest,
                CreatedById = admin001
            },
            new Criteria
            {
                Id = Guid.NewGuid(),
                Name = "Phong thái",
                Weight = 10,
                For = CriteriaFor.EntranceTest,
                CreatedById = admin001
            },

            // For Class
            new Criteria
            {
                Id = Guid.NewGuid(),
                Name = "Kiểm tra nhỏ 1",
                Weight = 5,
                For = CriteriaFor.Class,
                CreatedById = admin001
            },
            new Criteria
            {
                Id = Guid.NewGuid(),
                Name = "Kiểm tra nhỏ 2",
                Weight = 5,
                For = CriteriaFor.Class,
                CreatedById = admin001
            },
            new Criteria
            {
                Id = Guid.NewGuid(),
                Name = "Bài thi 1",
                Weight = 10,
                For = CriteriaFor.Class,
                CreatedById = "admin001"
            },
            new Criteria
            {
                Id = Guid.NewGuid(),
                Name = "Bài thi 2",
                Weight = 10,
                For = CriteriaFor.Class,
                CreatedById = "admin001"
            },
            new Criteria
            {
                Id = Guid.NewGuid(),
                Name = "Điểm chuyên cần",
                Weight = 5,
                For = CriteriaFor.Class,
                CreatedById = admin001
            },
            new Criteria
            {
                Id = Guid.NewGuid(),
                Name = "Thi cuối kỳ (Nhịp điệu)",
                Weight = 15,
                For = CriteriaFor.Class,
                CreatedById = admin001
            },
            new Criteria
            {
                Id = Guid.NewGuid(),
                Name = "Thi cuối kỳ (Độ chính xác)",
                Weight = 15,
                For = CriteriaFor.Class,
                CreatedById = admin001
            },
            new Criteria
            {
                Id = Guid.NewGuid(),
                Name = "Thi cuối kỳ (Âm sắc)",
                Weight = 15,
                For = CriteriaFor.Class,
                CreatedById = admin001
            },
            new Criteria
            {
                Id = Guid.NewGuid(),
                Name = "Thi cuối kỳ (Phong thái)",
                Weight = 15,
                For = CriteriaFor.Class,
                CreatedById = admin001
            }
        );

        #endregion

        #region Room Model

        var roomGuid1 = Guid.NewGuid();
        var roomGuid2 = Guid.NewGuid();
        var roomGuid3 = Guid.NewGuid();

        modelBuilder.Entity<Room>().HasData(
            new Room
            {
                Id = roomGuid1,
                Name = "Room 1",
                CreatedById = admin001
            },
            new Room
            {
                Id = roomGuid2,
                Name = "Room 2",
                CreatedById = admin001
            },
            new Room
            {
                Id = roomGuid3,
                Name = "Room 3",
                CreatedById = admin001
            }
        );

        #endregion

        #region EntranceTest Model

        var entranceTestGuid1 = Guid.NewGuid();
        var entranceTestGuid2 = Guid.NewGuid();
        var entranceTestGuid3 = Guid.NewGuid();

        modelBuilder.Entity<EntranceTest>().HasData(
            new EntranceTest
            {
                Id = entranceTestGuid1,
                RoomId = roomGuid1,
                Shift = Shift.Shift1_7h_8h30,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                InstructorId = teacher002,
                RoomName = "Room 1",
                CreatedById = admin001,
                Name = "EntranceTest 1"
            },
            new EntranceTest
            {
                Id = entranceTestGuid2,
                RoomId = roomGuid2,
                Shift = Shift.Shift3_10h45_12h,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3)),
                RoomName = "Room 2",
                CreatedById = admin001,
                Name = "EntranceTest 2"
            },
            new EntranceTest
            {
                Id = entranceTestGuid3,
                RoomId = roomGuid3,
                Shift = Shift.Shift5_14h15_15h45,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(5)),
                InstructorId = teacher002,
                CreatedById = admin001,
                Name = "EntranceTest 3"
            }
        );

        #endregion

        #region EntranceTestStudent Model

        var entranceTestStudentGuid1 = Guid.NewGuid();
        var entranceTestStudentGuid2 = Guid.NewGuid();
        var entranceTestStudentGuid3 = Guid.NewGuid();
        modelBuilder.Entity<EntranceTestStudent>().HasData(
            new EntranceTestStudent
            {
                Id = entranceTestStudentGuid1,
                BandScore = Random.Shared.NextInt64(3, 10),
                EntranceTestId = entranceTestGuid1,
                StudentFirebaseId = learner003,
                Rank = 1,
                Year = 2024,
                IsScoreAnnounced = true,
                CreatedById = admin001
            },
            new EntranceTestStudent
            {
                Id = entranceTestStudentGuid2,
                BandScore = Random.Shared.NextInt64(3, 10),
                EntranceTestId = entranceTestGuid1,
                StudentFirebaseId = learner003,
                Rank = 2,
                Year = 2024,
                IsScoreAnnounced = true,
                CreatedById = admin001
            },
            new EntranceTestStudent
            {
                Id = entranceTestStudentGuid3,
                BandScore = Random.Shared.NextInt64(3, 10),
                EntranceTestId = entranceTestGuid1,
                StudentFirebaseId = learner003,
                Rank = 3,
                Year = 2024,
                IsScoreAnnounced = true,
                CreatedById = admin001
            }
        );

        #endregion

        #region EntranceTestResult Model

        var entranceTestResultGuid = Guid.NewGuid();
        modelBuilder.Entity<EntranceTestResult>().HasData(
            new EntranceTestResult
            {
                Id = entranceTestResultGuid,
                CriteriaId = criteriaAccuracy,
                EntranceTestStudentId = entranceTestStudentGuid1,
                CreatedById = admin001
            }
        );

        #endregion

        #region Class Model
        var classTestGuid1 = Guid.NewGuid();
        var classTestGuid2 = Guid.NewGuid();
        modelBuilder.Entity<Class>().HasData(
            new Class()
            {
                Id = classTestGuid1,
                CreatedById = admin001,
                InstructorId = teacher002,
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class 1",
                Level = Level.Intermediate
            },
            new Class()
            {
                Id = classTestGuid2,
                CreatedById = admin001,
                InstructorId = teacher002,
                Status = ClassStatus.NotStarted,
                IsPublic = true,
                Name = "Class 2",
                Level = Level.Advanced
            }

        );


        #endregion

        #region Slot Model
        var slotTestGuid1 = Guid.NewGuid();
        var slotTestGuid2 = Guid.NewGuid();
        var slotTestGuid3 = Guid.NewGuid();
        var slotTestGuid4 = Guid.NewGuid();

        modelBuilder.Entity<Slot>().HasData(
            new Slot
            {
                Id = slotTestGuid1,
                ClassId = classTestGuid1,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                RoomId = roomGuid1,
                Shift = Shift.Shift1_7h_8h30,
                Status = SlotStatus.Ongoing,
            },

            new Slot
            {
                Id = slotTestGuid2,
                ClassId = classTestGuid1,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                RoomId = roomGuid1,
                Shift = Shift.Shift1_7h_8h30,
                Status = SlotStatus.Ongoing,
            },
            new Slot
            {
                Id = slotTestGuid3,
                ClassId = classTestGuid1,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4)),
                RoomId = roomGuid1,
                Shift = Shift.Shift1_7h_8h30,
                Status = SlotStatus.Ongoing,
            },
            new Slot
            {
                Id = slotTestGuid4,
                ClassId = classTestGuid1,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
                RoomId = roomGuid1,
                Shift = Shift.Shift1_7h_8h30,
                Status = SlotStatus.Ongoing,
            }
        );


        #endregion

        #region SystemConfig Model
        modelBuilder.Entity<SystemConfig>().HasData(
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Sĩ số lớp tối thiểu",
                ConfigValue = "8",  
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Sĩ số lớp tối đa",
                ConfigValue = "12",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Mức phí theo buổi LEVEL 1",
                ConfigValue = "200000",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Mức phí theo buổi LEVEL 2",
                ConfigValue = "250000",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Mức phí theo buổi LEVEL 3",
                ConfigValue = "300000",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Mức phí theo buổi LEVEL 4",
                ConfigValue = "350000",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Mức phí theo buổi LEVEL 5",
                ConfigValue = "400000",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Số buổi học 1 tuần LEVEL 1",
                ConfigValue = "2",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Số buổi học 1 tuần LEVEL 2",
                ConfigValue = "2",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Số buổi học 1 tuần LEVEL 3",
                ConfigValue = "2",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Số buổi học 1 tuần LEVEL 4",
                ConfigValue = "2",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Số buổi học 1 tuần LEVEL 5",
                ConfigValue = "2",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Tổng số buổi học LEVEL 1",
                ConfigValue = "30",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Tổng số buổi học LEVEL 2",
                ConfigValue = "30",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Tổng số buổi học LEVEL 3",
                ConfigValue = "30",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Tổng số buổi học LEVEL 4",
                ConfigValue = "40",
                Role = Role.Administrator
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Tổng số buổi học LEVEL 5",
                ConfigValue = "50",
                Role = Role.Administrator
            }

        );
        #endregion
    }
}