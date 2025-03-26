using Microsoft.AspNetCore.SignalR;

namespace PhotonPiano.PubSub.Pubsub;

public class PubSubHub : Hub<IPubSubHub>, IPubSubHub
{
    private static readonly HashSet<string> _connectedClients = new();

    public Task PubSub(PubSubMessage message)
    {
        // Console.WriteLine($"[PubSub] Received message: {JsonConvert.SerializeObject(message)}");

        // Prevent duplicate messages
        return Clients.Others.PubSub(message); // Send to all except the sender
    }

    public override async Task OnConnectedAsync()
    {
        _connectedClients.Add(Context.ConnectionId);
        // Console.WriteLine($"[PubSub] Client connected: {Context.ConnectionId} (Total: {_connectedClients.Count})");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connectedClients.Remove(Context.ConnectionId);
        // Console.WriteLine(
        //     $"[PubSub] Client disconnected: {Context.ConnectionId} (Remaining: {_connectedClients.Count})");
        await base.OnDisconnectedAsync(exception);
    }
}