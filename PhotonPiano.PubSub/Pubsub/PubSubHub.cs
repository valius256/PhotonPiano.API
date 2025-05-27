using Microsoft.AspNetCore.SignalR;

namespace PhotonPiano.PubSub.Pubsub;

public class PubSubHub : Hub<IPubSubHub>, IPubSubHub
{
    private static readonly HashSet<string> _connectedClients = new();

    public Task PubSub(PubSubMessage message)
    {
        return Clients.Others.PubSub(message);
    }

    public override async Task OnConnectedAsync()
    {
        _connectedClients.Add(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connectedClients.Remove(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}