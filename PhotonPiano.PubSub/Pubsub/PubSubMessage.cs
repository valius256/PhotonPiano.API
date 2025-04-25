using System.Collections.Generic;

namespace PhotonPiano.PubSub.Pubsub;

public class PubSubMessage
{
    public PubSubMessage(List<string> topic, object? content)
    {
        Topic = topic;
        Content = content;
    }

    public List<string> Topic { get; set; }
    public object? Content { get; set; }
}