

using PhotonPiano.BusinessLogic.BusinessModel.Account;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record StudentClassModel
    {
        public required Guid Id { get; set; }
        public required Guid ClassId { get; set; }
        public required string StudentFirebaseId { get; set; }
        public string? StudentFullName { get; set; }
        public string? ClassName { get; set; }
        public required string CreatedById { get; set; }
        public string? UpdateById { get; set; }
        public string? DeletedById { get; set; }
        public string? CertificateUrl { get; set; }
        public bool IsPassed { get; set; }
        public decimal? GPA { get; set; }
        public string? InstructorComment { get; set; }
        public AccountModel Student { get; set; } = default!;
    }
}
