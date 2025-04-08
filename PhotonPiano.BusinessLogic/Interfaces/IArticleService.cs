using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.News;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IArticleService
{
    Task<PagedResult<ArticleModel>> GetArticles(QueryPagedArticlesModel queryModel);
    
    Task<ArticleDetailsModel> GetArticleDetailsBySlug(string slug);

    Task<ArticleModel> CreateArticle(CreateArticleModel createModel, AccountModel currentAccount);
    
    Task UpdateArticle(string slug, UpdateArticleModel updateModel, AccountModel currentAccount);
    
    Task DeleteArticle(string slug, AccountModel currentAccount);
}