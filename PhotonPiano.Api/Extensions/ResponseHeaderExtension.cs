using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.Api.Extensions;

public static class ResponseHeaderExtension
{
    public static void AppendPagedResultMetaData<T>(this IHeaderDictionary headers, PagedResult<T> pagedResult)
        where T : class
    {
        headers.Append("X-Total-Count", pagedResult.TotalCount.ToString());
        headers.Append("X-Total-Pages", pagedResult.TotalPages.ToString());
        headers.Append("X-Page", pagedResult.Page.ToString());
        headers.Append("X-Page-Size", pagedResult.Limit.ToString());
    }
}