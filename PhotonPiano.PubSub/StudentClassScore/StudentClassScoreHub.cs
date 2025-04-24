using Microsoft.AspNetCore.SignalR;

namespace PhotonPiano.PubSub.StudentClassScore;

public class StudentClassScoreHub : Hub
{
    private static readonly HashSet<string> _connectedClients = new();
    
    public override async Task OnConnectedAsync()
    {
        _connectedClients.Add(Context.ConnectionId);
        var firebaseId = Context.GetHttpContext()?.Request.Query["firebaseId"];
        if (firebaseId.HasValue && !string.IsNullOrEmpty(firebaseId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, firebaseId!);
        }

        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var firebaseId = Context.GetHttpContext()?.Request.Query["firebaseId"];
        if (firebaseId.HasValue && !string.IsNullOrEmpty(firebaseId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, firebaseId!);
        }

        _connectedClients.Remove(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}