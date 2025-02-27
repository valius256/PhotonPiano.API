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
            // Ensure per-message debouncing
            lock (_lastMessageSent)
            {
                if (_lastMessageSent.TryGetValue(topic, out var lastSent) && now - lastSent < _debounceInterval)
                {
                    // Console.WriteLine($"[PubSub] Skipping duplicate message for topic: {topic}");
                    continue;
                }

                _lastMessageSent[topic] = now;
            }

            // Send message asynchronously and collect tasks
            tasks.Add(_hub.Clients.All.PubSub(new PubSubMessage(topics, message)));
        }

        await Task.WhenAll(tasks);
    }
}