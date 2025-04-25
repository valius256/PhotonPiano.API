using System.Collections.Generic;

namespace PhotonPiano.PubSub.Pubsub;

public interface IPubSubService
{
    void SendToAll(List<string> topic, object? arg1);
}