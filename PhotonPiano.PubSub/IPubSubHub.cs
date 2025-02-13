namespace PhotonPiano.PubSub;

public interface IPubSubHub
{
    Task PubSub(PubSubMessage message);
}