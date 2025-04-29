namespace PhotonPiano.BusinessLogic.BusinessModel.Tuition;

public class PayTuitionModel
{
    public Guid TuitionId { get; set; }
    public string ReturnUrl { get; set; }
    public string IpAddress { get; set; }
    public string ApiBaseUrl { get; set; }
}