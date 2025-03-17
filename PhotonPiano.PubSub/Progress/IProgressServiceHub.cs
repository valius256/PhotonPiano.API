

namespace PhotonPiano.PubSub.Progress
{
    public interface IProgressServiceHub
    {
        Task SendProgress(string userFirebaseId, string message, double progress);

    }
}
