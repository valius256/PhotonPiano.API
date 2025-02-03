namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IEmailService
{
    Task SendAsync(string templateName, List<string> toAddress, List<string>? ccAddresses,
        Dictionary<string, string> param, bool isInQueue = false);
}