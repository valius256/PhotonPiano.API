using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Tution;

public record QueryTutionModel : QueryPagedModel
{
    public List<Guid>? StudentClassId { get; init; } = [];
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public List<PaymentStatus>? PaymentStatus { get; init; }

    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc,
        out List<Guid>? studentClassId, out DateTime? startDate, out DateTime? endDate,
        out List<PaymentStatus>? paymentStatus)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
        studentClassId = StudentClassId;
        startDate = StartDate;
        endDate = EndDate;
        paymentStatus = PaymentStatus;
    }
}