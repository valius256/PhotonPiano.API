namespace PhotonPiano.Shared.Models;

public interface IApiResult<T>
{
    public string? Status { get; set; }
    public string? Op { get; set; }
    public T? Data { get; set; }
}

public class ApiResult<T> : IApiResult<T>
{
    public string? Status { get; set; }
    public string? Op { get; set; }
    public T? Data { get; set; }
}