using Microsoft.EntityFrameworkCore;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.SeedData
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            #region Account Model
            modelBuilder.Entity<Account>().HasData(
                new Account()
                {
                    AccountFirebaseId = "admin001",
                    Email = "admin001@gmail.com",
                    Role = Role.Administrator
                },
                new Account()
                {
                    AccountFirebaseId = "teacher002",
                    Email = "teacher002@gmail.com",
                    Role  = Role.Instructor
                },
                new Account()
                {
                    AccountFirebaseId = "learner003",
                    Email = "learner003@gmail.com",
                    Role = Role.Student
                }
            );
            #endregion

            #region Criterial Model

            var criterialTestId = Guid.NewGuid();
                modelBuilder.Entity<Criteria>().HasData(
                    new Criteria()
                    {
                        Id = criterialTestId,
                        Description = "criterialTest Description",
                        CreatedById = "admin001",
                    }
                );

            #endregion

            #region Room Model
                var roomGuid1 = Guid.NewGuid();
                var roomGuid2 = Guid.NewGuid();
                var roomGuid3 = Guid.NewGuid();

                modelBuilder.Entity<Room>().HasData(
                    new Room()
                    {
                        Id = roomGuid1,
                        Name = "Room 1",
                        CreatedById = "admin001",
                    },
                    new Room()
                    {
                        Id = roomGuid2,
                        Name = "Room 2",
                        CreatedById = "admin001",
                    },
                    new Room()
                    {
                        Id = roomGuid3,
                        Name = "Room 3",
                        CreatedById = "admin001",
                    }

                );

            #endregion
            
            #region EntranceTest Model
            var entranceTestGuid1 = Guid.NewGuid();
            var entranceTestGuid2 = Guid.NewGuid();
            var entranceTestGuid3 = Guid.NewGuid();
            
            modelBuilder.Entity<EntranceTest>().HasData(
                new EntranceTest()
                {
                    Id = entranceTestGuid1,
                    RoomId = roomGuid1,
                    AnnouncedTime = DateTime.UtcNow.AddHours(1),
                    Shift = Shift.Shift1_7h_8h30,
                    StartTime = DateTime.UtcNow.AddHours(4),
                    TeacherFirebaseId = "teacher002",
                    RoomName = "Room 1",
                    CreatedById = "admin001",

                },
                new EntranceTest()
                {
                    Id = entranceTestGuid2,
                    RoomId = roomGuid2,
                    AnnouncedTime = DateTime.UtcNow.AddHours(1),
                    Shift = Shift.Shift3_10h45_12h,
                    StartTime = DateTime.UtcNow.AddHours(4),
                    RoomName = "Room 2",
                    CreatedById = "admin001",

                },
                new EntranceTest()
                {
                    Id = entranceTestGuid3,
                    RoomId = roomGuid3,
                    AnnouncedTime = DateTime.UtcNow.AddHours(1),
                    Shift = Shift.Shift5_14h15_15h45,
                    StartTime = DateTime.UtcNow.AddHours(4),
                    TeacherFirebaseId = "teacher002",
                    CreatedById = "admin001",
                }
            );

            #endregion

            #region EntranceTestStudent Model
                var entranceTestStudentGuid1 = Guid.NewGuid();
                var entranceTestStudentGuid2 = Guid.NewGuid();
                var entranceTestStudentGuid3 = Guid.NewGuid();
                modelBuilder.Entity<EntranceTestStudent>().HasData(
                    new EntranceTestStudent()
                    {
                        Id = entranceTestStudentGuid1,
                        BandScore = (decimal)Random.Shared.NextInt64(3, 10),
                        EntranceTestId = entranceTestGuid1,
                        StudentFirebaseId = "learner003",
                        Rank = 1,
                        Year = 2024,
                        IsScoreAnnounced = true,
                        CreatedById = "admin001",
                        
                    },
                    new EntranceTestStudent()
                    {
                        Id = entranceTestStudentGuid2,
                        BandScore = (decimal)Random.Shared.NextInt64(3, 10),
                        EntranceTestId = entranceTestGuid1,
                        StudentFirebaseId = "learner003",
                        Rank = 2,
                        Year = 2024,
                        IsScoreAnnounced = true,
                        CreatedById = "admin001",

                    },
                    new EntranceTestStudent()
                    {
                        Id = entranceTestStudentGuid3,
                        BandScore = (decimal)Random.Shared.NextInt64(3, 10),
                        EntranceTestId = entranceTestGuid1,
                        StudentFirebaseId = "learner003",
                        Rank = 3,
                        Year = 2024,
                        IsScoreAnnounced = true,
                        CreatedById = "admin001",

                    }
                    


                );
                #endregion
                
            #region EntranceTestResult Model

                var entranceTestResultGuid = Guid.NewGuid();
                modelBuilder.Entity<EntranceTestResult>().HasData(
                    new EntranceTestResult()
                    {
                        Id = entranceTestResultGuid,
                         CriteriaId = criterialTestId,
                         EntranceTestStudentId = entranceTestStudentGuid1,
                         CreatedById = "admin001",
                         
                    }

                );
                #endregion
                
        }
    }
}
