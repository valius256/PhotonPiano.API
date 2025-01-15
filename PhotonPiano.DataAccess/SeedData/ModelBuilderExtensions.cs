using Microsoft.EntityFrameworkCore;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.SeedData;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        #region Account Model

        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                AccountFirebaseId = "admin001",
                Email = "admin001@gmail.com",
                Role = Role.Administrator
            },
            new Account
            {
                AccountFirebaseId = "teacher002",
                Email = "teacher002@gmail.com",
                Role = Role.Instructor
            },
            new Account 
            {
                AccountFirebaseId = "learner003",
                Email = "learner003@gmail.com",
                Role = Role.Student
            }
        );

        #endregion

        #region Criterial Model

        var criteriaAccuracy = Guid.NewGuid();
        modelBuilder.Entity<Criteria>().HasData(
            // For EntranceTest
            new Criteria
            {
                Id = Guid.NewGuid(), Name = "Nhịp điệu", Weight = 10, For = CriteriaFor.EntranceTest,
                CreatedById = "admin001"
            },
            new Criteria
            {
                Id = criteriaAccuracy, Name = "Độ chính xác", Weight = 10, For = CriteriaFor.EntranceTest,
                CreatedById = "admin001"
            },
            new Criteria
            {
                Id = Guid.NewGuid(), Name = "Âm Sắc", Weight = 10, For = CriteriaFor.EntranceTest,
                CreatedById = "admin001"
            },
            new Criteria
            {
                Id = Guid.NewGuid(), Name = "Phong thái", Weight = 10, For = CriteriaFor.EntranceTest,
                CreatedById = "admin001"
            },

            // For Class
            new Criteria
            {
                Id = Guid.NewGuid(), Name = "Kiểm tra nhỏ 1", Weight = 5, For = CriteriaFor.Class,
                CreatedById = "admin001"
            },
            new Criteria
            {
                Id = Guid.NewGuid(), Name = "Kiểm tra nhỏ 2", Weight = 5, For = CriteriaFor.Class,
                CreatedById = "admin001"
            },
            new Criteria
            {
                Id = Guid.NewGuid(), Name = "Bài thi 1", Weight = 10, For = CriteriaFor.Class, CreatedById = "admin001"
            },
            new Criteria
            {
                Id = Guid.NewGuid(), Name = "Bài thi 2", Weight = 10, For = CriteriaFor.Class, CreatedById = "admin001"
            },
            new Criteria
            {
                Id = Guid.NewGuid(), Name = "Điểm chuyên cần", Weight = 5, For = CriteriaFor.Class,
                CreatedById = "admin001"
            },
            new Criteria
            {
                Id = Guid.NewGuid(), Name = "Thi cuối kỳ (Nhịp điệu)", Weight = 15, For = CriteriaFor.Class,
                CreatedById = "admin001"
            },
            new Criteria
            {
                Id = Guid.NewGuid(), Name = "Thi cuối kỳ (Độ chính xác)", Weight = 15, For = CriteriaFor.Class,
                CreatedById = "admin001"
            },
            new Criteria
            {
                Id = Guid.NewGuid(), Name = "Thi cuối kỳ (Âm sắc)", Weight = 15, For = CriteriaFor.Class,
                CreatedById = "admin001"
            },
            new Criteria
            {
                Id = Guid.NewGuid(), Name = "Thi cuối kỳ (Phong thái)", Weight = 15, For = CriteriaFor.Class,
                CreatedById = "admin001"
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
                CreatedById = "admin001"
            },
            new Room
            {
                Id = roomGuid2,
                Name = "Room 2",
                CreatedById = "admin001"
            },
            new Room
            {
                Id = roomGuid3,
                Name = "Room 3",
                CreatedById = "admin001"
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
                InstructorId = "teacher002",
                RoomName = "Room 1",
                CreatedById = "admin001"
            },
            new EntranceTest
            {
                Id = entranceTestGuid2,
                RoomId = roomGuid2,
                Shift = Shift.Shift3_10h45_12h,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3)),
                RoomName = "Room 2",
                CreatedById = "admin001"
            },
            new EntranceTest
            {
                Id = entranceTestGuid3,
                RoomId = roomGuid3,
                Shift = Shift.Shift5_14h15_15h45,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(5)),
                InstructorId = "teacher002",
                CreatedById = "admin001"
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
                StudentFirebaseId = "learner003",
                Rank = 1,
                Year = 2024,
                IsScoreAnnounced = true,
                CreatedById = "admin001"
            },
            new EntranceTestStudent
            {
                Id = entranceTestStudentGuid2,
                BandScore = Random.Shared.NextInt64(3, 10),
                EntranceTestId = entranceTestGuid1,
                StudentFirebaseId = "learner003",
                Rank = 2,
                Year = 2024,
                IsScoreAnnounced = true,
                CreatedById = "admin001"
            },
            new EntranceTestStudent
            {
                Id = entranceTestStudentGuid3,
                BandScore = Random.Shared.NextInt64(3, 10),
                EntranceTestId = entranceTestGuid1,
                StudentFirebaseId = "learner003",
                Rank = 3,
                Year = 2024,
                IsScoreAnnounced = true,
                CreatedById = "admin001"
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
                CreatedById = "admin001"
            }
        );

        #endregion
    }
}