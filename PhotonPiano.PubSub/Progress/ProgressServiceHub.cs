
using Microsoft.AspNetCore.SignalR;
using PhotonPiano.PubSub.Notification;

namespace PhotonPiano.PubSub.Progress
{
    public class ProgressServiceHub : IProgressServiceHub
    {
        private readonly IHubContext<ProgressHub> _hubContext;

        public ProgressServiceHub(IHubContext<ProgressHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendProgress(string userFirebaseId, string message, double progress)
        {
            await _hubContext.Clients.Group(userFirebaseId).SendAsync("ReceiveProgress",
                progress, message);
        }
    }
}
