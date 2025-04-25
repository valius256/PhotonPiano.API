using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace PhotonPiano.PubSub.Notification;

public class NotificationHub : Hub
{
    private static readonly HashSet<string> _connectedClients = new();

    public async Task SendNotification(string user, string message)
    {
        await Clients.Group(user).SendAsync("ReceiveNotification", message);
    }

    public override async Task OnConnectedAsync()
    {
        _connectedClients.Add(Context.ConnectionId);
        var firebaseId = Context.GetHttpContext().Request.Query["firebaseId"];
        if (!string.IsNullOrEmpty(firebaseId)) await Groups.AddToGroupAsync(Context.ConnectionId, firebaseId);
        Console.WriteLine(
            $"[PubSub] Client disconnected: {Context.ConnectionId} (Remaining: {_connectedClients.Count})");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var firebaseId = Context.GetHttpContext().Request.Query["firebaseId"];
        if (!string.IsNullOrEmpty(firebaseId)) await Groups.RemoveFromGroupAsync(Context.ConnectionId, firebaseId);
        await base.OnDisconnectedAsync(exception);
    }
}