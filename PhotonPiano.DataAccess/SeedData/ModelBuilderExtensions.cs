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

        var studentFirebaseIds = new[]
        {
            "learner001", "learner002", "learner003", "learner004", "learner005", "learner006", "learner007",
            "learner008",
            "learner009", "learner010", "learner011", "learner012", "learner013", "learner014", "learner015",
            "learner016",
            "learner017", "learner018", "learner019", "learner020", "learner021", "learner022", "learner023",
            "learner024",
            "learner025", "learner026", "learner027", "learner028", "learner029", "learner030", "learner031",
            "learner032",
            "learner033", "learner034", "learner035", "learner036", "learner037", "learner038", "learner039",
            "learner040",
            "learner041", "learner042", "learner043", "learner044", "learner045", "learner046", "learner047",
            "learner048",
            "learner049", "learner050"
        };

        const string teacher001 = "lymytrantest@gmail.com";
        const string teacher002 = "quachthemytest@gmail.com";
        const string teacher003 = "buiducnamtest@gmail.com";
        const string teacherPhatLord = "teacherphatlord@gmail.com";
        const string teacherBaga = "teacherbaga@gmail.com";

        var accounts = new List<Account>
        {
            new()
            {
                AccountFirebaseId = admin001,
                Email = "admin001@gmail.com",
                Role = Role.Administrator,
                UserName = admin001
            }
        };

        for (var i = 0; i < studentFirebaseIds.Length; i++)
            accounts.Add(new Account
            {
                AccountFirebaseId = studentFirebaseIds[i],
                Email = $"{studentFirebaseIds[i]}@gmail.com",
                Role = Role.Student,
                UserName = studentFirebaseIds[i],
                Level = Level.Beginner,
                StudentStatus = StudentStatus.Unregistered
            });

        modelBuilder.Entity<Account>().HasData(accounts.ToArray());

        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                AccountFirebaseId = teacher001,
                Email = teacher001,
                Role = Role.Instructor,
                UserName = teacher001
            },
            new Account
            {
                AccountFirebaseId = teacher002,
                Email = teacher002,
                Role = Role.Instructor,
                UserName = teacher002
            },
            new Account
            {
                AccountFirebaseId = teacher003,
                Email = teacher003,
                Role = Role.Instructor,
                UserName = teacher003
            },
            new Account
            {
                AccountFirebaseId = "1axRN4fG0ybZyDOYvO8wnkm5lHJ3",
                Email = teacherPhatLord,
                Role = Role.Instructor,
                UserName = teacherPhatLord,
                Level = Level.Intermediate
            },
            new Account
            {
                AccountFirebaseId = "5YyhXdDHEiXZx38K5kj2qOrgM0l2",
                Email = teacherBaga,
                Role = Role.Instructor,
                UserName = teacherBaga,
                Level = Level.Intermediate
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
                StudentFirebaseId = studentFirebaseIds[0], // learner001
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
                StudentFirebaseId = studentFirebaseIds[1], // learner002
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
                StudentFirebaseId = studentFirebaseIds[2], // learner003
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

        modelBuilder.Entity<Class>().HasData(
            new Class
            {
                Id = classTestGuid1,
                CreatedById = admin001,
                InstructorId = teacher001,
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class Level 1",
                Level = Level.Beginner
            },
            new Class
            {
                Id = classTestGuid2,
                CreatedById = admin001,
                InstructorId = teacher002,
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class Level 2",
                Level = Level.Novice
            },
            new Class
            {
                Id = classTestGuid3,
                CreatedById = admin001,
                InstructorId = teacher003,
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class Level 3",
                Level = Level.Intermediate
            },
            new Class
            {
                Id = classTestGuid4,
                CreatedById = admin001,
                InstructorId = "1axRN4fG0ybZyDOYvO8wnkm5lHJ3",
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class Level 4",
                Level = Level.Advanced
            },
            new Class
            {
                Id = classTestGuid5,
                CreatedById = admin001,
                InstructorId = "5YyhXdDHEiXZx38K5kj2qOrgM0l2",
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class Level 5",
                Level = Level.Virtuoso
            }
        );

        #endregion

        #region Slot Model

        var slotGuids = new List<Guid>();
        for (var i = 0; i < 200; i++) slotGuids.Add(Guid.NewGuid());

        var shifts = new[]
        {
            Shift.Shift1_7h_8h30, Shift.Shift2_8h45_10h15, Shift.Shift3_10h45_12h, Shift.Shift4_12h30_14h00,
            Shift.Shift5_14h15_15h45
        };

        var slots = new List<Slot>();

// Level 1: 30 sessions, 2 sessions per week (Monday and Wednesday)
        for (var i = 0; i < 15; i++)
        {
            var date = DateTime.UtcNow.AddDays(i * 7);
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2],
                ClassId = classTestGuid1,
                Date = DateOnly.FromDateTime(date),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2 + 1],
                ClassId = classTestGuid1,
                Date = DateOnly.FromDateTime(date.AddDays(2)),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });
        }

// Level 2: 30 sessions, 2 sessions per week (Tuesday and Thursday)
        for (var i = 15; i < 30; i++)
        {
            var date = DateTime.UtcNow.AddDays((i - 15) * 7 + 1);
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2],
                ClassId = classTestGuid2,
                Date = DateOnly.FromDateTime(date),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2 + 1],
                ClassId = classTestGuid2,
                Date = DateOnly.FromDateTime(date.AddDays(2)),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });
        }

// Level 3: 30 sessions, 2 sessions per week (Monday and Wednesday)
        for (var i = 30; i < 45; i++)
        {
            var date = DateTime.UtcNow.AddDays((i - 30) * 7);
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2],
                ClassId = classTestGuid3,
                Date = DateOnly.FromDateTime(date),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2 + 1],
                ClassId = classTestGuid3,
                Date = DateOnly.FromDateTime(date.AddDays(2)),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });
        }

// Level 4: 40 sessions, 2 sessions per week (Tuesday and Thursday)
        for (var i = 45; i < 65; i++)
        {
            var date = DateTime.UtcNow.AddDays((i - 45) * 7 + 1);
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2],
                ClassId = classTestGuid4,
                Date = DateOnly.FromDateTime(date),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2 + 1],
                ClassId = classTestGuid4,
                Date = DateOnly.FromDateTime(date.AddDays(2)),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });
        }

// Level 5: 50 sessions, 2 sessions per week (Monday and Wednesday)
        for (var i = 65; i < 90; i++)
        {
            var date = DateTime.UtcNow.AddDays((i - 65) * 7);
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2],
                ClassId = classTestGuid5,
                Date = DateOnly.FromDateTime(date),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2 + 1],
                ClassId = classTestGuid5,
                Date = DateOnly.FromDateTime(date.AddDays(2)),
                RoomId = roomGuids[i % 20],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.Ongoing
            });
        }

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
        for (var i = 0; i < 200; i++) slotStudentGuids.Add(Guid.NewGuid());

        var slotStudents = new List<SlotStudent>();

        // Assign students to slots ensuring each class has 8-10 students
        for (var i = 0; i < 180; i++)
        for (var j = 0; j < 10; j++)
            slotStudents.Add(new SlotStudent
            {
                SlotId = slotGuids[i],
                StudentFirebaseId = studentFirebaseIds[(i * 10 + j) % studentFirebaseIds.Length],
                CreatedById = admin001
            });

        modelBuilder.Entity<SlotStudent>().HasData(slotStudents);

        #endregion

        #region StudentClass Model

        var studentClassGuids = new List<Guid>();
        for (var i = 0; i < 50; i++) studentClassGuids.Add(Guid.NewGuid());

        var studentClasses = new List<StudentClass>();

        for (var i = 0; i < 5; i++)
        for (var j = 0; j < 10; j++)
            studentClasses.Add(new StudentClass
            {
                Id = studentClassGuids[i * 10 + j],
                ClassId = new[] { classTestGuid1, classTestGuid2, classTestGuid3, classTestGuid4, classTestGuid5 }[i],
                StudentFirebaseId = studentFirebaseIds[i * 10 + j],
                CreatedById = admin001,
                IsPassed = false,
                GPA = null,
                InstructorComment = null
            });

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