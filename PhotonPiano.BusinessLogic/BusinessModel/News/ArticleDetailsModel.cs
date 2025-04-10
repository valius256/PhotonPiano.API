using PhotonPiano.BusinessLogic.BusinessModel.Account;

namespace PhotonPiano.BusinessLogic.BusinessModel.News;

public record ArticleDetailsModel : ArticleModel
{
    public AccountModel CreatedBy { get; init; } = default!;
    
    public AccountModel UpdatedBy { get; init; } = default!;
    
    public AccountModel DeletedBy { get; init; } = default!;
}