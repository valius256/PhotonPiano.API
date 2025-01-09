namespace PhotonPiano.BusinessLogic.BusinessModel.Auth;

public record FirebaseErrorResponseModel
{
    public int Code { get; init; }
    
    public required string Message { get; init; }
}