﻿

namespace PhotonPiano.DataAccess.Models.Entity
{
    public class Level : BaseEntityWithId
    {
        //From parent : Guid Id
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<string> SkillsEarned { get; set; } = new List<string>();

        public int SlotPerWeek { get; set; }

        public int TotalSlots { get; set; }

        public decimal PricePerSlot { get; set; }

        public decimal MinimumTheoreticalScore { get; set; }
        public decimal MinimumPracticalScore { get; set; }
        public decimal MinimumGPA { get; set; }

        public bool IsGenreDivided { get; set; }

        public Guid? NextLevelId { get; set; }

        public string? ThemeColor { get; set; }
        
        public bool RequiresEntranceTest { get; set; }

        public virtual Level? NextLevel { get; set; } // Navigation property

        public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
        
        public virtual ICollection<Account> SelfEvaluatedAccounts { get; set; } = [];

        public virtual ICollection<EntranceTestStudent> EntranceTestStudents { get; set; } = new List<EntranceTestStudent>();

    }
}
