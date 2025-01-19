namespace PhotonPiano.DataAccess.Models.Paging;

public record PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int Limit { get; set; }
    public int Page { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(decimal.Divide(TotalCount, Limit));
}