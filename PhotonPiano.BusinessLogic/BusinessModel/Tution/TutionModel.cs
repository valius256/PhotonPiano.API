using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Tution;

public record TutionModel
{
    public required Guid Id { get; init; }
    public required Guid StudentClassId { get; init; }
    public decimal Amount { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public PaymentStatus PaymentStatus { get; init; }
}