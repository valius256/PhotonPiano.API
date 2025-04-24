using System.Threading.Tasks;

namespace PhotonPiano.PubSub.Pubsub;

public interface IPubSubHub
{
    Task PubSub(PubSubMessage message);
}