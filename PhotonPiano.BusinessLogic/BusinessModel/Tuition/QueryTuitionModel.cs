using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Tuition;

public record QueryTuitionModel : QueryPagedModel
{
    public List<Guid>? StudentClassId { get; init; } = [];
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public List<PaymentStatus>? PaymentStatus { get; init; }

    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc,
        out List<Guid>? studentClassId, out DateOnly? startDate, out DateOnly? endDate,
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