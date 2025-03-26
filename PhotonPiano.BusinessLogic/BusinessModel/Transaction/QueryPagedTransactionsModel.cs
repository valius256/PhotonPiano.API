using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Transaction;

public record QueryPagedTransactionsModel : QueryPagedModel
{
    public DateTime? StartDate { get; init; }

    public DateTime? EndDate { get; init; }

    public string? Code { get; init; }

    public Guid? Id { get; init; }

    public List<PaymentStatus> PaymentStatuses { get; init; } = [];

    public List<PaymentMethod> PaymentMethods { get; set; } = [];

    public void Deconstruct(out DateTime? startDate, out DateTime? endDate, out string? code, out Guid? id, out List<PaymentStatus> paymentStatuses, out List<PaymentMethod> paymentMethods)
    {
        startDate = StartDate;
        endDate = EndDate;
        code = Code;
        id = Id;
        paymentStatuses = PaymentStatuses;
        paymentMethods = PaymentMethods;
    }
}