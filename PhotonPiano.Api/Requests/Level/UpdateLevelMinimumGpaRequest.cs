namespace PhotonPiano.Api.Requests.Level;

public record UpdateLevelMinimumGpaRequest
{
    public decimal MinimumGpa { get; set; }
}