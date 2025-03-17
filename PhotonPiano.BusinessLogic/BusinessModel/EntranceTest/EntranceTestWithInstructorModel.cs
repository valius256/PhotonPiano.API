using PhotonPiano.BusinessLogic.BusinessModel.Account;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record EntranceTestWithInstructorModel : EntranceTestModel
{
    public AccountModel Instructor { get; init; } = default!;
}