using Microsoft.EntityFrameworkCore.Migrations;
using PhotonPiano.DataAccess.Models.Enum;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_Criteria_Seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"EntranceTestResult\"");
            migrationBuilder.Sql("DELETE FROM \"StudentClassScore\"");
            migrationBuilder.Sql("DELETE FROM \"Criteria\"");

            migrationBuilder.InsertData(
                table: "Criteria",
                columns: new[] { "Id", "CreatedAt", "Name", "Weight", "Description", "CreatedById", "For", "RecordStatus" },
                values: new object[,]
                {
                    {
                        Guid.NewGuid(), DateTime.UtcNow.AddHours(7),
                        "Hand Independence", 20m,
                        "Ability to play different rhythms/melodies with left and right hands separately.",
                        "admin001", (int)CriteriaFor.EntranceTest, (int)RecordStatus.IsActive
                    },
                    {
                        Guid.NewGuid(), DateTime.UtcNow.AddHours(7),
                        "Pedal Control", 15m,
                        "Mastery of using the sustain pedal for smooth phrasing and resonance.",
                        "admin001", (int)CriteriaFor.EntranceTest, (int)RecordStatus.IsActive
                    },
                    {
                        Guid.NewGuid(), DateTime.UtcNow.AddHours(7),
                        "Dynamic Expression", 20m,
                        "Control of volume (soft/loud) with touch — a unique piano skill.",
                        "admin001", (int)CriteriaFor.EntranceTest, (int)RecordStatus.IsActive
                    },
                    {
                        Guid.NewGuid(), DateTime.UtcNow.AddHours(7),
                        "Sight Reading", 20m,
                        "Ability to read and play new music quickly at first sight.",
                        "admin001", (int)CriteriaFor.EntranceTest, (int)RecordStatus.IsActive
                    },
                    {
                        Guid.NewGuid(), DateTime.UtcNow.AddHours(7),
                        "Finger Strength and Agility", 15m,
                        "Strength and flexibility in fingers for complex piano passages.",
                        "admin001", (int)CriteriaFor.EntranceTest, (int)RecordStatus.IsActive
                    }
                }
            );

            migrationBuilder.InsertData(
                table: "Criteria",
                columns: new[] { "Id", "CreatedAt", "Name", "Weight", "Description", "CreatedById", "For", "RecordStatus" },
                values: new object[,]
                {
                    {
                        Guid.NewGuid(), DateTime.UtcNow.AddHours(7), 
                        "Tone Control Practice Assignment", 10m, 
                        "Weekly exercises to practice producing different tones (bright, dark, warm) based on touch.",
                        "admin001", (int)CriteriaFor.Class, (int)RecordStatus.IsActive
                    },
                    {
                        Guid.NewGuid(), DateTime.UtcNow.AddHours(7), 
                        "Rhythmic Stability Test", 10m, 
                        "Short in-class test to check student's ability to maintain steady tempo at various speeds.",
                        "admin001", (int)CriteriaFor.Class, (int)RecordStatus.IsActive
                    },
                    {
                        Guid.NewGuid(), DateTime.UtcNow.AddHours(7), 
                        "Articulation Techniques Workshop", 10m, 
                        "Practice and submit recordings of staccato, legato, slurs, accents, etc.",
                        "admin001", (int)CriteriaFor.Class, (int)RecordStatus.IsActive
                    },
                    {
                        Guid.NewGuid(), DateTime.UtcNow.AddHours(7), 
                        "Musical Expression Performance", 20m, 
                        "Perform a piece demonstrating musical phrasing and emotion, graded on expression quality.",
                        "admin001", (int)CriteriaFor.Class, (int)RecordStatus.IsActive
                    },
                    {
                        Guid.NewGuid(), DateTime.UtcNow.AddHours(7), 
                        "Arpeggios and Hand Stretch Training", 10m, 
                        "Daily practice tasks focusing on wide-hand stretches and flowing arpeggios.",
                        "admin001", (int)CriteriaFor.Class, (int)RecordStatus.IsActive
                    },
                    {
                        Guid.NewGuid(), DateTime.UtcNow.AddHours(7), 
                        "Hand-Pedal Coordination Test", 15m, 
                        "Timed test for playing a short piece combining hand playing with correct pedal timing.",
                        "admin001", (int)CriteriaFor.Class, (int)RecordStatus.IsActive
                    },
                    {
                        Guid.NewGuid(), DateTime.UtcNow.AddHours(7), 
                        "Memorization Assignment", 10m, 
                        "Memorize and perform a short piece without sheet music.",
                        "admin001", (int)CriteriaFor.Class, (int)RecordStatus.IsActive
                    },
                    {
                        Guid.NewGuid(), DateTime.UtcNow.AddHours(7), 
                        "Duet Project (Peer Performance)", 15m, 
                        "Partner with another student to perform a duet piece, focusing on timing and harmony.",
                        "admin001", (int)CriteriaFor.Class, (int)RecordStatus.IsActive
                    }
                }
            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
