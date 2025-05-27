using Microsoft.AspNetCore.SignalR;

namespace PhotonPiano.PubSub.Pubsub;

public class PubSubService : IPubSubService
{
    private static readonly Dictionary<string, DateTime> _lastMessageSent = new();
    private static readonly TimeSpan _debounceInterval = TimeSpan.FromSeconds(1);
    private readonly IHubContext<PubSubHub, IPubSubHub> _hub;

    public PubSubService(IHubContext<PubSubHub, IPubSubHub> hubContext)
    {
        _hub = hubContext;
    }

    public async void SendToAll(List<string> topics, object? message)
    {
        var now = DateTime.UtcNow;
        List<Task> tasks = new();

        foreach (var topic in topics)
        {
            lock (_lastMessageSent)
            {
                if (_lastMessageSent.TryGetValue(topic, out var lastSent) &&
                    now - lastSent < _debounceInterval) continue;

                _lastMessageSent[topic] = now;
            }

            tasks.Add(_hub.Clients.All.PubSub(new PubSubMessage(topics, message)));
        }

        await Task.WhenAll(tasks);
    }
}