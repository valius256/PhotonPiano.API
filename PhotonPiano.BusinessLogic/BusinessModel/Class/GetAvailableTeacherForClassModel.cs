using PhotonPiano.BusinessLogic.BusinessModel.Query;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public record GetAvailableTeacherForClassModel : QueryPagedModel
{
    public Guid ClassId { get; init; }

    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc,
        out Guid classId)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
        classId = ClassId;
    }
}