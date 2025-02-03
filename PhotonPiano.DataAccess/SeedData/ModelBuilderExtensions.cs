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

        const string learner001 = "learner001";
        const string learner002 = "learner002";
        const string learner003 = "learner003";
        const string learner004 = "learner004";
        const string learner005 = "learner005";
        const string learner006 = "learner006";
        const string learner007 = "learner007";
        const string learner008 = "learner008";
        const string learner009 = "learner009";
        const string learner010 = "learner010";

        const string teacher001 = "lymytranTest@gmail.com";
        const string teacher002 = "quachthemyTest@gmail.com";
        const string teacher003 = "buiducnamTest@gmail.com";
        const string teacherPhatLord = "teacherphatlord@gmail.com";


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
                AccountFirebaseId = "nQhzMDSe8aW5RLerTaHa6yvh8c23",
                Email = "minh@gmail.com",
                Role = Role.Student,
                UserName = "minh@gmail.com",
                Level = Level.Beginner,
                StudentStatus = StudentStatus.Unregistered
            },
            new Account
            {
                AccountFirebaseId = "gnRssA2sZHWnXB23oUuUxwz95Ln1",
                Email = "staff123@gmail.com",
                Role = Role.Staff,
                UserName = "staff 123"
            },
            new Account
            {
                AccountFirebaseId = teacher001,
                Email = "teacher002@gmail.com",
                Role = Role.Instructor,
                UserName = teacher001
            },
            new Account
            {
                AccountFirebaseId = teacher003,
                Email = "teacher003@gmail.com",
                Role = Role.Instructor,
                UserName = teacher003
            },
            new Account
            {
                AccountFirebaseId = learner001,
                Email = "learner001@gmail.com",
                Role = Role.Student,
                UserName = learner001,
                Level = Level.Beginner,
                StudentStatus = StudentStatus.Unregistered
            },
            new Account
            {
                AccountFirebaseId = learner002,
                Email = "learner002@gmail.com",
                Role = Role.Student,
                UserName = learner002,
                Level = Level.Beginner,
                StudentStatus = StudentStatus.Unregistered
            },
            new Account
            {
                AccountFirebaseId = learner003,
                Email = "learner003@gmail.com",
                Role = Role.Student,
                UserName = learner003,
                Level = Level.Beginner,
                StudentStatus = StudentStatus.Unregistered
            },
            new Account
            {
                AccountFirebaseId = learner004,
                Email = "learner004@gmail.com",
                Role = Role.Student,
                UserName = learner004,
                Level = Level.Beginner,
                StudentStatus = StudentStatus.Unregistered
            },
            new Account
            {
                AccountFirebaseId = learner005,
                Email = "learner005@gmail.com",
                Role = Role.Student,
                UserName = learner005,
                Level = Level.Beginner,
                StudentStatus = StudentStatus.Unregistered
            },
            new Account
            {
                AccountFirebaseId = learner006,
                Email = "learner006@gmail.com",
                Role = Role.Student,
                UserName = learner006,
                Level = Level.Beginner,
                StudentStatus = StudentStatus.Unregistered
            },
            new Account
            {
                AccountFirebaseId = learner007,
                Email = "learner007@gmail.com",
                Role = Role.Student,
                UserName = learner007,
                Level = Level.Beginner,
                StudentStatus = StudentStatus.Unregistered
            },
            new Account
            {
                AccountFirebaseId = learner008,
                Email = "learner008@gmail.com",
                Role = Role.Student,
                UserName = learner008,
                Level = Level.Beginner,
                StudentStatus = StudentStatus.Unregistered
            },
            new Account
            {
                AccountFirebaseId = learner009,
                Email = "learner009@gmail.com",
                Role = Role.Student,
                UserName = learner009,
                Level = Level.Beginner,
                StudentStatus = StudentStatus.Unregistered
            },
            new Account
            {
                AccountFirebaseId = learner010,
                Email = "learner010@gmail.com",
                Role = Role.Student,
                UserName = learner010,
                Level = Level.Beginner,
                StudentStatus = StudentStatus.Unregistered
            },
            new Account
            {
                AccountFirebaseId = "1axRN4fG0ybZyDOYvO8wnkm5lHJ3",
                Email = teacherPhatLord,
                Role = Role.Instructor,
                UserName = teacherPhatLord,
                Level = Level.Intermediate
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

        var roomGuids = new List<Guid>();
        for (var i = 0; i < 20; i++) roomGuids.Add(Guid.NewGuid());

        var rooms = new List<Room>();
        for (var i = 0; i < 20; i++)
            rooms.Add(new Room
            {
                Id = roomGuids[i],
                Name = $"Room {i + 1}",
                CreatedById = admin001
            });

        modelBuilder.Entity<Room>().HasData(rooms);

        #endregion

        #region EntranceTest Model

        var entranceTestGuid1 = Guid.NewGuid();
        var entranceTestGuid2 = Guid.NewGuid();
        var entranceTestGuid3 = Guid.NewGuid();

        modelBuilder.Entity<EntranceTest>().HasData(
            new EntranceTest
            {
                Id = entranceTestGuid1,
                RoomId = roomGuids[0],
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
                RoomId = roomGuids[1],
                Shift = Shift.Shift3_10h45_12h,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3)),
                RoomName = "Room 2",
                CreatedById = admin001,
                Name = "EntranceTest 2"
            },
            new EntranceTest
            {
                Id = entranceTestGuid3,
                RoomId = roomGuids[2],
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
                StudentFirebaseId = learner001,
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
                StudentFirebaseId = learner001,
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
                StudentFirebaseId = learner001,
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
        var classTestGuid3 = Guid.NewGuid();
        var classTestGuid4 = Guid.NewGuid();
        var classTestGuid5 = Guid.NewGuid();
        var classPhatLord = Guid.NewGuid();
        var classTestGuid6 = Guid.NewGuid();
        var classTestGuid7 = Guid.NewGuid();
        var classTestGuid8 = Guid.NewGuid();
        var classTestGuid9 = Guid.NewGuid();
        var classTestGuid10 = Guid.NewGuid();

        modelBuilder.Entity<Class>().HasData(
            new Class
            {
                Id = classTestGuid1,
                CreatedById = admin001,
                InstructorId = teacher001,
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class 1",
                Level = Level.Intermediate
            },
            new Class
            {
                Id = classTestGuid2,
                CreatedById = admin001,
                InstructorId = teacher002,
                Status = ClassStatus.NotStarted,
                IsPublic = true,
                Name = "Class 2",
                Level = Level.Advanced
            },
            new Class
            {
                Id = classTestGuid3,
                CreatedById = admin001,
                InstructorId = teacher003,
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class 3",
                Level = Level.Beginner
            },
            new Class
            {
                Id = classTestGuid4,
                CreatedById = admin001,
                InstructorId = teacher002,
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class 4",
                Level = Level.Intermediate
            },
            new Class
            {
                Id = classTestGuid5,
                CreatedById = admin001,
                InstructorId = teacher003,
                Status = ClassStatus.NotStarted,
                IsPublic = true,
                Name = "Class 5",
                Level = Level.Advanced
            },
            new Class
            {
                Id = classPhatLord,
                CreatedById = admin001,
                InstructorId = "1axRN4fG0ybZyDOYvO8wnkm5lHJ3",
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class Teacher Phat",
                Level = Level.Advanced
            },
            new Class
            {
                Id = classTestGuid6,
                CreatedById = admin001,
                InstructorId = teacher001,
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class 6",
                Level = Level.Intermediate
            },
            new Class
            {
                Id = classTestGuid7,
                CreatedById = admin001,
                InstructorId = teacher002,
                Status = ClassStatus.NotStarted,
                IsPublic = true,
                Name = "Class 7",
                Level = Level.Advanced
            },
            new Class
            {
                Id = classTestGuid8,
                CreatedById = admin001,
                InstructorId = teacher003,
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class 8",
                Level = Level.Beginner
            },
            new Class
            {
                Id = classTestGuid9,
                CreatedById = admin001,
                InstructorId = teacher002,
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class 9",
                Level = Level.Intermediate
            },
            new Class
            {
                Id = classTestGuid10,
                CreatedById = admin001,
                InstructorId = teacher003,
                Status = ClassStatus.NotStarted,
                IsPublic = true,
                Name = "Class 10",
                Level = Level.Advanced
            }
        );

        #endregion

        #region Slot Model

        var slotGuids = new List<Guid>();
        for (var i = 0; i < 120; i++) slotGuids.Add(Guid.NewGuid());

        var shifts = new[]
        {
            Shift.Shift1_7h_8h30, Shift.Shift2_8h45_10h15, Shift.Shift3_10h45_12h, Shift.Shift4_12h30_14h00,
            Shift.Shift5_14h15_15h45
        };

        var slots = new List<Slot>();
        for (var i = 0; i < 20; i++)
            slots.Add(new Slot
            {
                Id = slotGuids[i],
                ClassId = classTestGuid1,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });

        for (var i = 20; i < 40; i++)
            slots.Add(new Slot
            {
                Id = slotGuids[i],
                ClassId = classTestGuid2,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i - 20)),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });

        for (var i = 40; i < 60; i++)
            slots.Add(new Slot
            {
                Id = slotGuids[i],
                ClassId = classTestGuid3,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i - 40)),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });

        for (var i = 60; i < 80; i++)
            slots.Add(new Slot
            {
                Id = slotGuids[i],
                ClassId = classTestGuid4,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i - 60)),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });

        for (var i = 80; i < 100; i++)
            slots.Add(new Slot
            {
                Id = slotGuids[i],
                ClassId = classTestGuid5,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i - 80)),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });

        // Slots for Class Phat Lord
        for (var i = 100; i < 120; i++)
            slots.Add(new Slot
            {
                Id = slotGuids[i],
                ClassId = classPhatLord,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i - 100)),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });

        modelBuilder.Entity<Slot>().HasData(slots);

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

        #region SlotStudent Model

        var slotStudentGuids = new List<Guid>();
        for (var i = 0; i < 120; i++) slotStudentGuids.Add(Guid.NewGuid());

        var slotStudents = new List<SlotStudent>();
        var studentFirebaseIds = new[]
            { learner001, learner002, learner003, learner004, learner005, learner006, learner007, learner008 };

        for (var i = 0; i < 20; i++)
        for (var j = 0; j < studentFirebaseIds.Length; j++)
            slotStudents.Add(new SlotStudent
            {
                SlotId = slotGuids[i],
                StudentFirebaseId = studentFirebaseIds[j],
                CreatedById = admin001
            });

        for (var i = 20; i < 40; i++)
        for (var j = 0; j < studentFirebaseIds.Length; j++)
            slotStudents.Add(new SlotStudent
            {
                SlotId = slotGuids[i],
                StudentFirebaseId = studentFirebaseIds[j],
                CreatedById = admin001
            });

        for (var i = 40; i < 60; i++)
        for (var j = 0; j < studentFirebaseIds.Length; j++)
            slotStudents.Add(new SlotStudent
            {
                SlotId = slotGuids[i],
                StudentFirebaseId = studentFirebaseIds[j],
                CreatedById = admin001
            });

        for (var i = 60; i < 80; i++)
        for (var j = 0; j < studentFirebaseIds.Length; j++)
            slotStudents.Add(new SlotStudent
            {
                SlotId = slotGuids[i],
                StudentFirebaseId = studentFirebaseIds[j],
                CreatedById = admin001
            });

        for (var i = 80; i < 100; i++)
        for (var j = 0; j < studentFirebaseIds.Length; j++)
            slotStudents.Add(new SlotStudent
            {
                SlotId = slotGuids[i],
                StudentFirebaseId = studentFirebaseIds[j],
                CreatedById = admin001
            });

        // Adding SlotStudent for Class Phat Lord
        for (var i = 100; i < 120; i++)
        for (var j = 0; j < studentFirebaseIds.Length; j++)
            slotStudents.Add(new SlotStudent
            {
                SlotId = slotGuids[i],
                StudentFirebaseId = studentFirebaseIds[j],
                CreatedById = admin001
            });

        modelBuilder.Entity<SlotStudent>().HasData(slotStudents);

        #endregion

        #region StudentClass Model

        var studentClassGuids = new List<Guid>();
        for (var i = 0; i < 10; i++) studentClassGuids.Add(Guid.NewGuid());

        var studentClasses = new List<StudentClass>
        {
            new()
            {
                Id = studentClassGuids[0],
                ClassId = classTestGuid1,
                StudentFirebaseId = learner001,
                CreatedById = admin001,
                IsPassed = false,
                GPA = null,
                InstructorComment = null
            },
            new()
            {
                Id = studentClassGuids[1],
                ClassId = classTestGuid2,
                StudentFirebaseId = learner002,
                CreatedById = admin001,
                IsPassed = false,
                GPA = null,
                InstructorComment = null
            },
            new()
            {
                Id = studentClassGuids[2],
                ClassId = classTestGuid3,
                StudentFirebaseId = learner003,
                CreatedById = admin001,
                IsPassed = false,
                GPA = null,
                InstructorComment = null
            },
            new()
            {
                Id = studentClassGuids[3],
                ClassId = classTestGuid4,
                StudentFirebaseId = learner004,
                CreatedById = admin001,
                IsPassed = false,
                GPA = null,
                InstructorComment = null
            },
            new()
            {
                Id = studentClassGuids[4],
                ClassId = classTestGuid5,
                StudentFirebaseId = learner005,
                CreatedById = admin001,
                IsPassed = false,
                GPA = null,
                InstructorComment = null
            },
            new()
            {
                Id = studentClassGuids[5],
                ClassId = classPhatLord,
                StudentFirebaseId = learner006,
                CreatedById = admin001,
                IsPassed = false,
                GPA = null,
                InstructorComment = null
            },
            new()
            {
                Id = studentClassGuids[6],
                ClassId = classTestGuid6,
                StudentFirebaseId = learner007,
                CreatedById = admin001,
                IsPassed = false,
                GPA = null,
                InstructorComment = null
            },
            new()
            {
                Id = studentClassGuids[7],
                ClassId = classTestGuid7,
                StudentFirebaseId = learner008,
                CreatedById = admin001,
                IsPassed = false,
                GPA = null,
                InstructorComment = null
            },
            new()
            {
                Id = studentClassGuids[8],
                ClassId = classTestGuid8,
                StudentFirebaseId = learner009,
                CreatedById = admin001,
                IsPassed = false,
                GPA = null,
                InstructorComment = null
            },
            new()
            {
                Id = studentClassGuids[9],
                ClassId = classTestGuid9,
                StudentFirebaseId = learner010,
                CreatedById = admin001,
                IsPassed = false,
                GPA = null,
                InstructorComment = null
            }
        };

        modelBuilder.Entity<StudentClass>().HasData(studentClasses);

        #endregion

        #region DayOff Model

        var dayOffs = new List<DayOff>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tết Dương Lịch",
                StartTime = new DateTime(DateTime.UtcNow.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(DateTime.UtcNow.Year, 1, 1, 23, 59, 59, DateTimeKind.Utc),
                CreatedById = admin001
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tết Nguyên Đán",
                StartTime = new DateTime(DateTime.UtcNow.Year, 2, 10, 0, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(DateTime.UtcNow.Year, 2, 16, 23, 59, 59, DateTimeKind.Utc),
                CreatedById = admin001
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Giỗ Tổ Hùng Vương",
                StartTime = new DateTime(DateTime.UtcNow.Year, 4, 21, 0, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(DateTime.UtcNow.Year, 4, 21, 23, 59, 59, DateTimeKind.Utc),
                CreatedById = admin001
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Ngày Giải Phóng Miền Nam",
                StartTime = new DateTime(DateTime.UtcNow.Year, 4, 30, 0, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(DateTime.UtcNow.Year, 4, 30, 23, 59, 59, DateTimeKind.Utc),
                CreatedById = admin001
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Ngày Quốc Tế Lao Động",
                StartTime = new DateTime(DateTime.UtcNow.Year, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(DateTime.UtcNow.Year, 5, 1, 23, 59, 59, DateTimeKind.Utc),
                CreatedById = admin001
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Ngày Quốc Khánh",
                StartTime = new DateTime(DateTime.UtcNow.Year, 9, 2, 0, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(DateTime.UtcNow.Year, 9, 2, 23, 59, 59, DateTimeKind.Utc),
                CreatedById = admin001
            }
        };

        modelBuilder.Entity<DayOff>().HasData(dayOffs);

        #endregion
    }
}