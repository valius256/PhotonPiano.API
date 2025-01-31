namespace PhotonPiano.BusinessLogic.BusinessModel.Email;

public class SendEmailModel
{
    public List<string> ToEmail { get; set; } = new();

    public List<string>? CcEmail { get; set; } = new();

    public required string Subject { get; set; }

    public required string Body { get; set; }
}