using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.SeedData;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        static async Task<string> GetFirebaseIdAsync(string email, string password)
        {
            using var client = new HttpClient();

            var url =
                "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyAgjNobHW7j13vXDbjd68ZmcGamsf26Z8c";

            var jsonRequest = JsonConvert.SerializeObject(new
            {
                email,
                password,
                returnSecureToken = true
            });

            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception("Error occurred while signing in to Firebase account: " + errorResponse);
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            return responseObject.localId;
        }
        var r = new Random();

        #region Level Model
        Guid level1 = Guid.NewGuid();
        Guid level2 = Guid.NewGuid();
        Guid level3 = Guid.NewGuid();
        Guid level4 = Guid.NewGuid();
        Guid level5 = Guid.NewGuid();
        Guid[] levels = [level1, level2, level3, level4, level5];
        modelBuilder.Entity<Level>().HasData(
            // For EntranceTest
            new Level
            {
                Id = level1,
                Name = "Sơ cấp (Beginner)",
                IsGenreDivided = false,
                Description = "Học viên mới bắt đầu làm quen với piano, nhận biết các phím đàn, nốt nhạc và tư thế ngồi đúng cách",
                SkillsEarned = [
                    "Hiểu biết cơ bản về bàn phím piano và vị trí các nốt nhạc.",
                    "Đọc nốt nhạc trên khóa Sol và khóa Fa.",
                    "Rèn luyện ngón tay với các bài tập đơn giản.",
                    "Chơi các giai điệu cơ bản bằng cả hai tay.",
                    "Nhận biết nhịp điệu đơn giản (2/4, 3/4, 4/4).",
                    "Áp dụng kỹ thuật legato (chơi liền tiếng) và staccato (chơi ngắt tiếng) cơ bản."
                ],
                PricePerSlot = 200000,
                TotalSlots = 30,
                SlotPerWeek = 2,
                NextLevelId = level2

            },
            new Level
            {
                Id = level2,
                Name = "Tiền trung cấp (Elementary/Pre-Intermediate)",
                IsGenreDivided = false,
                Description = "Học viên có thể chơi những bài nhạc đơn giản và bắt đầu làm quen với cách sử dụng pedal.",
                SkillsEarned = [
                    "Chơi những bản nhạc có hai tay độc lập với các tiết tấu khác nhau.",
                    "Hiểu và áp dụng dấu hóa (♯, ♭) vào bài nhạc.",
                    "Chuyển đổi hợp âm cơ bản và đệm đàn đơn giản.",
                    "Sử dụng pedal sustain một cách cơ bản.",
                    "Phát triển khả năng cảm âm và điều chỉnh lực đánh phím."
                ],
                PricePerSlot = 250000,
                TotalSlots = 30,
                SlotPerWeek = 2,
                NextLevelId = level3

            },
            new Level
            {
                Id = level3,
                Name = "Trung cấp (Intermediate)",
                IsGenreDivided = false,
                Description = "Học viên đã có nền tảng vững chắc và bắt đầu chơi các bản nhạc phức tạp hơn với nhiều sắc thái biểu cảm.",
                SkillsEarned = [
                    "Chơi các bản nhạc có tiết tấu nhanh hơn và nhiều kỹ thuật hơn.",
                    "Sử dụng pedal một cách linh hoạt để tạo hiệu ứng âm thanh tốt hơn.",
                    "Thành thạo các quãng (intervals) và hợp âm 7.",
                    "Chơi các gam (scale) và arpeggio ở nhiều tốc độ khác nhau.",
                    "Đọc bản nhạc nhanh hơn và luyện tập khả năng thị tấu (sight-reading).",
                    "Phát triển phong cách biểu diễn cá nhân."
                ],
                PricePerSlot = 300000,
                TotalSlots = 40,
                SlotPerWeek = 2,
                NextLevelId = level4

            },
            new Level
            {
                Id = level4,
                Name = "Cao cấp (Advanced)",
                IsGenreDivided = false,
                Description = "Học viên có thể chơi những tác phẩm phức tạp và yêu cầu kỹ thuật cao, đồng thời thể hiện cảm xúc qua từng giai điệu.",
                SkillsEarned = [
                    "Chơi các tác phẩm cổ điển của các nhà soạn nhạc như Mozart, Beethoven, Chopin...",
                    "Kỹ thuật legato, staccato, trill, tremolo ở mức độ cao.",
                    "Kiểm soát lực đánh phím để tạo độ sâu và sắc thái âm nhạc phong phú.",
                    "Thị tấu ở tốc độ cao hơn và có độ chính xác cao.",
                    "Sử dụng pedal một cách chuyên nghiệp để nâng cao hiệu ứng âm thanh.",
                    "Khả năng chơi piano với nhiều thể loại khác nhau (cổ điển, jazz, pop, đệm hát...)."
                ],
                PricePerSlot = 400000,
                TotalSlots = 40,
                SlotPerWeek = 2,
                NextLevelId = level5

            },
            new Level
            {
                Id = level5,
                Name = "Chuyên nghiệp (Professional/Master Level)",
                IsGenreDivided = false,
                Description = "Học viên đạt đến trình độ chuyên nghiệp, có thể biểu diễn trên sân khấu và thể hiện cá tính âm nhạc của mình.\r\n\r\n",
                SkillsEarned = [
                    "Chơi những tác phẩm khó và yêu cầu kỹ thuật phức tạp như Rachmaninoff, Liszt, Debussy...",
                    "Biểu diễn tự tin trên sân khấu với phong cách cá nhân.",
                    "Ứng biến (improvisation) và sáng tạo trong cách chơi.",
                    "Phối hợp với các nhạc cụ khác trong một dàn nhạc hoặc ban nhạc.",
                    "Sáng tác và phối nhạc theo phong cách riêng.",
                    "Kỹ năng giảng dạy piano cho người khác (nếu theo hướng sư phạm)."
                ],
                PricePerSlot = 500000,
                TotalSlots = 50,
                SlotPerWeek = 2,
            }
        );
        #endregion

        #region Account Model

        const string admin001 = "admin001";

        var studentEmails = new[]
        {
            "learner001@gmail.com", "learner002@gmail.com", "learner003@gmail.com", "learner004@gmail.com",
            "learner005@gmail.com",
            "learner006@gmail.com", "learner007@gmail.com", "learner008@gmail.com", "learner009@gmail.com",
            "learner010@gmail.com",
            "learner011@gmail.com", "learner012@gmail.com", "learner013@gmail.com", "learner014@gmail.com",
            "learner015@gmail.com",
            "learner016@gmail.com", "learner017@gmail.com", "learner018@gmail.com", "learner019@gmail.com",
            "learner020@gmail.com",
            "learner021@gmail.com", "learner022@gmail.com", "learner023@gmail.com", "learner024@gmail.com",
            "learner025@gmail.com",
            "learner026@gmail.com", "learner027@gmail.com", "learner028@gmail.com", "learner029@gmail.com",
            "learner030@gmail.com",
            "learner031@gmail.com", "learner032@gmail.com", "learner033@gmail.com", "learner034@gmail.com",
            "learner035@gmail.com",
            "learner036@gmail.com", "learner037@gmail.com", "learner038@gmail.com", "learner039@gmail.com",
            "learner040@gmail.com",
            "learner041@gmail.com", "learner042@gmail.com", "learner043@gmail.com", "learner044@gmail.com",
            "learner045@gmail.com",
            "learner046@gmail.com", "learner047@gmail.com", "learner048@gmail.com", "learner049@gmail.com",
            "learner050@gmail.com"
        };


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

        for (var i = 0; i < studentEmails.Length; i++)
        {
            var firebaseId = GetFirebaseIdAsync(studentEmails[i], "123456").Result;
            accounts.Add(new Account
            {
                AccountFirebaseId = firebaseId,
                Email = studentEmails[i],
                Role = Role.Student,
                UserName = studentEmails[i].Split('@')[0],
                LevelId = levels[r.Next(levels.Length)],
                StudentStatus = StudentStatus.Unregistered,
                FullName = NameGenerator.GenerateFullName()
            });
        }

        modelBuilder.Entity<Account>().HasData(accounts.ToArray());

        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                AccountFirebaseId = "I8Kl7zLMjbgwZrkN6h9fjHSMHoP2",
                Email = "lymytrantest@gmail.com",
                Role = Role.Instructor,
                UserName = "Cô Trân",
                FullName = "Lý Mỹ Trân"
            },
            new Account
            {
                AccountFirebaseId = "Q0gfswO9mZTGMReq3pyQ7l5qen03",
                Email = "quachthemytest@gmail.com",
                Role = Role.Instructor,
                UserName = "Thầy Mỹ",
                FullName = "Quách Thế Mỹ"
            },
            new Account
            {
                AccountFirebaseId = "tLbBkH40xreMLylKIopJz9Z09Le2",
                Email = "buiducnamtest@gmail.com",
                Role = Role.Instructor,
                UserName = "Thầy Nam",
                FullName = "Bùi Đức Nam"
            },
            new Account
            {
                AccountFirebaseId = "1axRN4fG0ybZyDOYvO8wnkm5lHJ3",
                Email = "teacherphatlord@gmail.com",
                Role = Role.Instructor,
                UserName = "Thầy Phát",
                LevelId = levels[r.Next(levels.Length)]
            },
            new Account
            {
                AccountFirebaseId = "5YyhXdDHEiXZx38K5kj2qOrgM0l2",
                Email = "teacherbaga@gmail.com",
                Role = Role.Instructor,
                UserName = "Thầy ba",
                LevelId = levels[r.Next(levels.Length)]
            },
            new Account
            {
                AccountFirebaseId = "nQhzMDSe8aW5RLerTaHa6yvh8c23",
                Email = "minh@gmail.com",
                Role = Role.Student,
                UserName = "minh@gmail.com",
                LevelId = levels[r.Next(levels.Length)],
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
                InstructorId = "I8Kl7zLMjbgwZrkN6h9fjHSMHoP2",
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
                InstructorId = "Q0gfswO9mZTGMReq3pyQ7l5qen03",
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
                StudentFirebaseId = GetFirebaseIdAsync("learner001@gmail.com", "123456").Result, // learner001
                LevelId = level1,
                Year = 2024,
                IsScoreAnnounced = true,
                CreatedById = admin001
            },
            new EntranceTestStudent
            {
                Id = entranceTestStudentGuid2,
                BandScore = Random.Shared.NextInt64(3, 10),
                EntranceTestId = entranceTestGuid1,
                StudentFirebaseId = GetFirebaseIdAsync("learner002@gmail.com", "123456").Result, // learner002
                LevelId = level2,
                Year = 2024,
                IsScoreAnnounced = true,
                CreatedById = admin001
            },
            new EntranceTestStudent
            {
                Id = entranceTestStudentGuid3,
                BandScore = Random.Shared.NextInt64(3, 10),
                EntranceTestId = entranceTestGuid1,
                StudentFirebaseId = GetFirebaseIdAsync("learner003@gmail.com", "123456").Result, // learner003
                LevelId = level4,
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
                InstructorId = "I8Kl7zLMjbgwZrkN6h9fjHSMHoP2",
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class Level 1",
                LevelId = level1
            },
            new Class
            {
                Id = classTestGuid2,
                CreatedById = admin001,
                InstructorId = "Q0gfswO9mZTGMReq3pyQ7l5qen03",
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class Level 2",
                LevelId = level2
            },
            new Class
            {
                Id = classTestGuid3,
                CreatedById = admin001,
                InstructorId = "tLbBkH40xreMLylKIopJz9Z09Le2",
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class Level 3",
                LevelId = level3
            },
            new Class
            {
                Id = classTestGuid4,
                CreatedById = admin001,
                InstructorId = "1axRN4fG0ybZyDOYvO8wnkm5lHJ3",
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class Level 4",
                LevelId = level4
            },
            new Class
            {
                Id = classTestGuid5,
                CreatedById = admin001,
                InstructorId = "5YyhXdDHEiXZx38K5kj2qOrgM0l2",
                Status = ClassStatus.Ongoing,
                IsPublic = true,
                Name = "Class Level 5",
                LevelId = level5
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

        // Assign a fixed room for each class
        var classRooms = new Dictionary<Guid, Guid>
        {
            { classTestGuid1, roomGuids[0] },
            { classTestGuid2, roomGuids[1] },
            { classTestGuid3, roomGuids[2] },
            { classTestGuid4, roomGuids[3] },
            { classTestGuid5, roomGuids[4] }
        };

        // Level 1: 30 sessions, 2 sessions per week (Monday and Wednesday)
        for (var i = 0; i < 15; i++)
        {
            var date = DateTime.UtcNow.AddDays(i * 7);
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2],
                ClassId = classTestGuid1,
                Date = DateOnly.FromDateTime(date),
                RoomId = classRooms[classTestGuid1],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.NotStarted
            });
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2 + 1],
                ClassId = classTestGuid1,
                Date = DateOnly.FromDateTime(date.AddDays(2)),
                RoomId = classRooms[classTestGuid1],
                Shift = shifts[(i + 1) % shifts.Length],
                Status = SlotStatus.NotStarted
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
                RoomId = classRooms[classTestGuid2],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.NotStarted
            });
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2 + 1],
                ClassId = classTestGuid2,
                Date = DateOnly.FromDateTime(date.AddDays(2)),
                RoomId = classRooms[classTestGuid2],
                Shift = shifts[(i + 1) % shifts.Length],
                Status = SlotStatus.NotStarted
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
                RoomId = classRooms[classTestGuid3],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.NotStarted
            });
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2 + 1],
                ClassId = classTestGuid3,
                Date = DateOnly.FromDateTime(date.AddDays(2)),
                RoomId = classRooms[classTestGuid3],
                Shift = shifts[(i + 1) % shifts.Length],
                Status = SlotStatus.NotStarted
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
                RoomId = classRooms[classTestGuid4],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.NotStarted
            });
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2 + 1],
                ClassId = classTestGuid4,
                Date = DateOnly.FromDateTime(date.AddDays(2)),
                RoomId = classRooms[classTestGuid4],
                Shift = shifts[(i + 1) % shifts.Length],
                Status = SlotStatus.NotStarted
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
                RoomId = classRooms[classTestGuid5],
                Shift = shifts[i % shifts.Length],
                Status = SlotStatus.NotStarted
            });
            slots.Add(new Slot
            {
                Id = slotGuids[i * 2 + 1],
                ClassId = classTestGuid5,
                Date = DateOnly.FromDateTime(date.AddDays(2)),
                RoomId = classRooms[classTestGuid5],
                Shift = shifts[(i + 1) % shifts.Length],
                Status = SlotStatus.NotStarted
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
            },
            new SystemConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = "Thuế suất năm 2025",
                ConfigValue = "0.05",
                Role = Role.Administrator
            }
        );

        #endregion

        #region SlotStudent Model

        var slotStudentGuids = new List<Guid>();
        for (var i = 0; i < 200; i++) slotStudentGuids.Add(Guid.NewGuid());

        var slotStudents = new List<SlotStudent>();

        // Fetch firebaseIds for all students
        var studentFirebaseIds = new List<string>();
        foreach (var email in studentEmails)
        {
            var firebaseId = GetFirebaseIdAsync(email, "123456").Result;
            studentFirebaseIds.Add(firebaseId);
        }

        // Assign students to classes
        var studentsPerClass = 10;
        var classIds = new[] { classTestGuid1, classTestGuid2, classTestGuid3, classTestGuid4, classTestGuid5 };
        var studentClassAssignments = new Dictionary<Guid, List<string>>();

        for (var i = 0; i < classIds.Length; i++)
        {
            var classId = classIds[i];
            studentClassAssignments[classId] = new List<string>();
            for (var j = 0; j < studentsPerClass; j++)
            {
                var studentIndex = i * studentsPerClass + j;
                if (studentIndex < studentFirebaseIds.Count)
                    studentClassAssignments[classId].Add(studentFirebaseIds[studentIndex]);
            }
        }

        // Assign students to slots based on their class assignment
        foreach (var slot in slots)
        {
            var studentsForClass = studentClassAssignments[slot.ClassId];
            foreach (var studentId in studentsForClass)
                slotStudents.Add(new SlotStudent
                {
                    SlotId = slot.Id,
                    StudentFirebaseId = studentId,
                    CreatedById = admin001
                });
        }

        modelBuilder.Entity<SlotStudent>().HasData(slotStudents);

        #endregion

        #region StudentClass Model

        var studentClassGuids = new List<Guid>();
        for (var i = 0; i < 50; i++) studentClassGuids.Add(Guid.NewGuid());

        var studentClasses = new List<StudentClass>();

        for (var i = 0; i < classIds.Length; i++)
        {
            var classId = classIds[i];
            var studentsInClass = studentClassAssignments[classId];
            for (var j = 0; j < studentsInClass.Count; j++)
                studentClasses.Add(new StudentClass
                {
                    Id = studentClassGuids[i * studentsPerClass + j],
                    ClassId = classId,
                    StudentFirebaseId = studentsInClass[j],
                    CreatedById = admin001,
                    IsPassed = false,
                    GPA = null,
                    InstructorComment = null
                });
        }

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