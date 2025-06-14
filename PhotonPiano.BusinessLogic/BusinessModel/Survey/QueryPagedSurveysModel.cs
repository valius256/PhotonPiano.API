﻿using PhotonPiano.BusinessLogic.BusinessModel.Query;

namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record QueryPagedSurveysModel : QueryPagedModel
{
    public string? Keyword { get; init; }
    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
    }
}