namespace PhotonPiano.PubSub;

public interface IPubSubService
{
    void SendToAll(List<string> topic, object? arg1);
}