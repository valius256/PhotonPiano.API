using Microsoft.EntityFrameworkCore.Migrations;
using PhotonPiano.DataAccess.Models.Enum;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Property_WantToContinue_To_Table_Account : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinimumGPA",
                table: "Level",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "WantToContinue",
                table: "Account",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "55974743-7c93-47ab-877e-eda4cb9f96c5",
                column: "Name",
                value: "Professional/Master Level"
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "0d232654-2ce8-4193-9bc3-acd3eddf2ff2",
                column: "Name",
                value: "Advanced"
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "3db94220-15db-4880-9b57-b6064b27c11b",
                column: "Name",
                value: "Intermediate"
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "ad04326c-4d91-4d67-bc76-2c1dfb87c2c5",
                column: "Name",
                value: "Elementary/Pre-Intermediate"
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "8fb54d6e-315c-470d-825e-91b8314134f7",
                column: "Name",
                value: "Beginner"
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "55974743-7c93-47ab-877e-eda4cb9f96c5",
                column: "Description",
                value: "Students reach a professional level, can perform on stage and express their musical personality."
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "0d232654-2ce8-4193-9bc3-acd3eddf2ff2",
                column: "Description",
                value: "Students can play complex and technically demanding pieces, while expressing emotion through each melody."
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "3db94220-15db-4880-9b57-b6064b27c11b",
                column: "Description",
                value: "Students have a solid foundation and begin to play more complex pieces with more expressive nuances."
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "ad04326c-4d91-4d67-bc76-2c1dfb87c2c5",
                column: "Description",
                value: "Students can play simple songs and begin to get familiar with using the pedals."
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "8fb54d6e-315c-470d-825e-91b8314134f7",
                column: "Description",
                value: "Beginners learn to play the piano, recognize the keys, notes and correct sitting posture."
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "55974743-7c93-47ab-877e-eda4cb9f96c5",
                column: "SkillsEarned",
                value: new List<string> { "Play difficult and technically complex works such as Rachmaninoff, Liszt, Debussy...", "Perform confidently on stage with a personal style.", "Improvise and be creative in playing.", "Collaborate with other instruments in an orchestra or band.", "Compose and arrange music in your own style.", "Skills in teaching piano to others (if pedagogically oriented)." }
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "0d232654-2ce8-4193-9bc3-acd3eddf2ff2",
                column: "SkillsEarned",
                value: new List<string> { "Play classical works by composers such as Mozart, Beethoven, Chopin...", "High-level legato, staccato, trill, tremolo techniques.", "Control the force of the keystroke to create rich musical depth and nuances.", "Sight play at higher speeds and with high precision.", "Use the pedal professionally to enhance the sound effects.", "Ability to play the piano in many different genres (classical, jazz, pop, accompaniment...)." }
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "3db94220-15db-4880-9b57-b6064b27c11b",
                column: "SkillsEarned",
                value: new List<string> { "Play faster tempos and more technical pieces.", "Use the pedals flexibly to create better sound effects.", "Master intervals and seventh chords.", "Play scales and arpeggios at a variety of speeds.", "Read music faster and practice sight-reading.", "Develop your personal performing style." }
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "ad04326c-4d91-4d67-bc76-2c1dfb87c2c5",
                column: "SkillsEarned",
                value: new List<string> { "Play pieces with two hands independently at different tempos.", "Understand and apply accidentals (♯, ♭) to music.", "Transform basic chords and simple accompaniment.", "Use the sustain pedal in a basic way.", "Develop a sense of touch and control of playing force." }
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "8fb54d6e-315c-470d-825e-91b8314134f7",
                column: "SkillsEarned",
                value: new List<string> { "Basic understanding of the piano keyboard and note positions.", "Read notes on the G and F clefs.", "Exercise your fingers with simple exercises.", "Play basic melodies with both hands.", "Recognize simple rhythms (2/4, 3/4, 4/4).", "Apply basic legato and staccato techniques." }
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "55974743-7c93-47ab-877e-eda4cb9f96c5",
                column: "MinimumGPA",
                value: 5.0
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "0d232654-2ce8-4193-9bc3-acd3eddf2ff2",
                column: "MinimumGPA",
                value: 5.0
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "3db94220-15db-4880-9b57-b6064b27c11b",
                column: "MinimumGPA",
                value: 5.0
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "ad04326c-4d91-4d67-bc76-2c1dfb87c2c5",
                column: "MinimumGPA",
                value: 5.0
            );
            migrationBuilder.UpdateData(
                table: "Level",
                keyColumn: "Id",
                keyValue: "8fb54d6e-315c-470d-825e-91b8314134f7",
                column: "MinimumGPA",
                value: 5.0
            );

            migrationBuilder.InsertData(
                table: "SurveyQuestion",
                columns: new[] { "Id", "Type", "QuestionContent", "Options", "OrderIndex", "AllowOtherAnswer", "CreatedById", "CreatedAt", "RecordStatus" },
                values: new object[]
                {
                    Guid.Parse("ddbd1341-97c4-48eb-b096-0fde6dd37f5c"),
                    0,
                    "What is your current level of piano?",
                    new List<string> {"Never played piano before","Can play simple pieces","Can play pretty much intermediate pieces","Can play hard, complex and advanced pieces","Can play almost any piece and is able to perform nartually on stage."},
                    0,
                    false,
                    "admin001",
                    DateTime.UtcNow.AddHours(7),
                    1
                }
            );
            migrationBuilder.InsertData(
                table: "SurveyQuestion",
                columns: new[] { "Id", "Type", "QuestionContent", "Options", "OrderIndex", "AllowOtherAnswer", "CreatedById", "CreatedAt", "RecordStatus" },
                values: new object[]
                {
                    Guid.Parse("c5d06db6-5377-4f2d-a887-68139163851e"),
                    1,
                    "What is your target of learning piano?",
                    new List<string> {"Entertain and relax myself","Play my favorite pieces or songs","Perform for my family, friends or loved one","Use piano as my career","Perform on all kind of stages","Piano researching"},
                    1,
                    true,
                    "admin001",
                    DateTime.UtcNow.AddHours(7),
                    1
                }
            );
            migrationBuilder.InsertData(
                table: "SurveyQuestion",
                columns: new[] { "Id", "Type", "QuestionContent", "Options", "OrderIndex", "AllowOtherAnswer", "CreatedById", "CreatedAt", "RecordStatus" },
                values: new object[]
                {
                    Guid.Parse("4aac0a4f-834b-4b13-a030-c153cf92c381"),
                    1,
                    "What is your favorite genres of music?",
                    new List<string> {"Classical","POP","Jazz/Blue","Country","Religious","Movies/Games"},
                    1,
                    true,
                    "admin001",
                    DateTime.UtcNow.AddHours(7),
                    1
                }
            );
            migrationBuilder.InsertData(
                table: "PianoSurvey",
                columns: new[] { "Id", "Name", "Description", "CreatedById", "CreatedAt", "RecordStatus" },
                values: new object[]
                {
                    Guid.Parse("f357ef28-7125-4b85-889d-f57d8316ae03"),
                    "Entrance Survey (EN)",
                    "Entrance Survey for all group, focusing on their start point and their goal.",
                    "admin001",
                    DateTime.UtcNow.AddHours(7),
                    1
                }
            );
            migrationBuilder.InsertData(
                table: "PianoSurveyQuestion",
                columns: new[] { "Id", "SurveyId", "QuestionId", "OrderIndex", "IsRequired", "CreatedAt", "RecordStatus" },
                values: new object[]
                {
                    Guid.NewGuid(),
                    Guid.Parse("f357ef28-7125-4b85-889d-f57d8316ae03"),
                    Guid.Parse("ddbd1341-97c4-48eb-b096-0fde6dd37f5c"),
                    0,
                    true,
                    DateTime.UtcNow.AddHours(7),
                    1
                }
            );
            migrationBuilder.InsertData(
                table: "PianoSurveyQuestion",
                columns: new[] { "Id", "SurveyId", "QuestionId", "OrderIndex", "IsRequired", "CreatedAt", "RecordStatus" },
                values: new object[]
                {
                    Guid.NewGuid(),
                    Guid.Parse("f357ef28-7125-4b85-889d-f57d8316ae03"),
                    Guid.Parse("c5d06db6-5377-4f2d-a887-68139163851e"),
                    1,
                    true,
                    DateTime.UtcNow.AddHours(7),
                    1
                }
            );
            migrationBuilder.InsertData(
                table: "PianoSurveyQuestion",
                columns: new[] { "Id", "SurveyId", "QuestionId", "OrderIndex", "IsRequired", "CreatedAt","RecordStatus" },
                values: new object[]
                {
                    Guid.NewGuid(),
                    Guid.Parse("f357ef28-7125-4b85-889d-f57d8316ae03"),
                    Guid.Parse("4aac0a4f-834b-4b13-a030-c153cf92c381"),
                    2,
                    true,
                    DateTime.UtcNow.AddHours(7),
                    1
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumGPA",
                table: "Level");

            migrationBuilder.DropColumn(
                name: "WantToContinue",
                table: "Account");
        }
    }
}
