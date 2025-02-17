using PhotonPiano.BusinessLogic.BusinessModel.Query;

namespace PhotonPiano.BusinessLogic.BusinessModel.Notification;

public record QueryPagedNotificationsModel : QueryPagedModel
{
    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
    }
}