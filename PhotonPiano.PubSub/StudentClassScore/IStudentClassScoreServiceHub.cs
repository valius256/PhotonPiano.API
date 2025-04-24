namespace PhotonPiano.PubSub.StudentClassScore;

public interface IStudentClassScoreServiceHub
{
    Task SendScorePublishedNotificationAsync(string firebaseId, object message);
}