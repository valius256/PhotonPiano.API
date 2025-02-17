using PhotonPiano.BusinessLogic.BusinessModel.Query;

namespace PhotonPiano.BusinessLogic.BusinessModel.Notification;

public record QueryPagedNotificationsModel : QueryPagedModel
{
    public bool? IsViewed { get; init; }
    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc, out bool? isViewed)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
        isViewed = IsViewed;
    }
}