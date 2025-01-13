using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Room;

public record QueryRoomModel : QueryPagedModel
{
    public string? Keyword { get; set; }
    public List<RoomStatus>? RoomStatus { get; set; } = [];

    public string GetLikeKeyword()
    {
        return string.IsNullOrEmpty(Keyword) ? string.Empty : $"%{Keyword}%";
    }


    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc,
        out string? keyword, out List<RoomStatus>? roomStatus)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
        keyword = Keyword;
        roomStatus = RoomStatus;
    }
}