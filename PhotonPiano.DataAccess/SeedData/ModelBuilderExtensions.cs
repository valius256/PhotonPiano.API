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
                    Role  = Role.Teacher
                },
                new Account()
                {
                    AccountFirebaseId = "learner003",
                    Email = "learner003@gmail.com",
                    Role = Role.Learner
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
                    }

                );

            #endregion
            
            #region EntranceTest Model
            var entranceTestGuid1 = Guid.NewGuid();
            modelBuilder.Entity<EntranceTest>().HasData(
                new EntranceTest()
                {
                    Id = entranceTestGuid1,
                    RoomId = roomGuid1,
                    AnnouncedTime = DateTime.UtcNow.AddHours(1),
                    Shift = Shift.Shift1_7h_8h30,
                    StartTime = DateTime.UtcNow.AddHours(4),
                    TeacherFirebaseId = "teacher002",
                }
            );

            #endregion

            #region EntranceTestStudent Model
                var entranceTestStudentGuid1 = Guid.NewGuid();

                modelBuilder.Entity<EntranceTestStudent>().HasData(
                    new EntranceTestStudent()
                    {
                        Id = entranceTestStudentGuid1,
                        BandScore = (decimal)Random.Shared.NextInt64(3, 10),
                        EntranceTestId = entranceTestGuid1,
                        LearnerFirebaseId = "learner003",
                        Rank = 1,
                        Year = 2024,
                        IsScoreAnnounced = true,
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
                         
                    }

                );
                #endregion
                
        }
    }
}
