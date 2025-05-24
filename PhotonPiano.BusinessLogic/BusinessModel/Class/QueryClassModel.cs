
using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record QueryClassModel : QueryPagedModel
    {
        public List<ClassStatus> ClassStatus { get; init; } = [];

        public List<Guid> Levels { get; init; } = [];

        public string? Keyword { get; init; }

        public bool? IsScorePublished { get; init; }

        public bool? IsPublic { get; init; }

        public string? TeacherId { get; init; }

        public string? StudentId { get; init; }

        public string GetLikeKeyword()
        {
            return string.IsNullOrEmpty(Keyword) ? string.Empty : $"%{Keyword}%";
        }

        public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc,
            out List<ClassStatus> classStatus, out List<Guid> levels, out string? keyword, out bool? isScorePublished,
            out string? teacherId, out string? studentId, out bool? isPublic)
        {
            page = Page;
            pageSize = PageSize;
            sortColumn = SortColumn;
            orderByDesc = OrderByDesc;
            classStatus = ClassStatus;
            levels = Levels;
            keyword = Keyword;
            isScorePublished = IsScorePublished;
            teacherId = TeacherId;
            studentId = StudentId;
            isPublic = IsPublic;
        }
    }
}
