using Microsoft.AspNetCore.SignalR;

namespace PhotonPiano.PubSub.StudentClassScore;

public class StudentClassScoreServiceHub : IStudentClassScoreServiceHub
{
    private readonly IHubContext<StudentClassScoreHub> _hubContext;

    public StudentClassScoreServiceHub(IHubContext<StudentClassScoreHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task SendScorePublishedNotificationAsync(string firebaseId, object message)
    {
        await _hubContext.Clients.Group(firebaseId).SendAsync("ScorePublished", message);
    }
}