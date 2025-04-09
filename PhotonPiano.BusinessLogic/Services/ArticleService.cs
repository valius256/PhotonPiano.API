using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.News;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _unitOfWork;

    public ArticleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private async Task<string> GenerateUniqueSlugAsync(
        string title,
        Func<string, Task<bool>> slugExistsAsync)
    {
        var slugHelper = new Slugify.SlugHelper();
        string baseSlug = slugHelper.GenerateSlug(title);
        string uniqueSlug = baseSlug;
        int suffix = 1;

        while (await slugExistsAsync(uniqueSlug))
        {
            uniqueSlug = $"{baseSlug}-{suffix++}";
        }

        return uniqueSlug;
    }

    public async Task<PagedResult<ArticleModel>> GetArticles(QueryPagedArticlesModel queryModel,
        AccountModel? currentAccount)
    {
        var (page, size, column, desc, keyword) = queryModel;

        var likeKeyword = queryModel.GetLikeKeyword();

        return await _unitOfWork.ArticleRepository.GetPaginatedWithProjectionAsync<ArticleModel>(page, size, column,
            desc, expressions:
            [
                a => currentAccount != null && currentAccount.Role == Role.Staff || a.IsPublished == true,
                a => string.IsNullOrEmpty(keyword) || 
                     EF.Functions.ILike(EF.Functions.Unaccent(a.Title), likeKeyword) ||
                     EF.Functions.ILike(EF.Functions.Unaccent(a.Content), likeKeyword)
            ]);
    }

    public async Task<ArticleDetailsModel> GetArticleDetailsBySlug(string slug)
    {
        var article = await _unitOfWork.ArticleRepository.FindSingleProjectedAsync<ArticleDetailsModel>(
            a => a.Slug == slug,
            hasTrackings: false,
            option: TrackingOption.IdentityResolution);

        if (article is null)
        {
            throw new NotFoundException("Article not found");
        }

        return article;
    }

    public async Task<ArticleModel> CreateArticle(CreateArticleModel createModel, AccountModel currentAccount)
    {
        var article = createModel.Adapt<Article>();

        article.Slug = await GenerateUniqueSlugAsync(article.Title,
            slug => _unitOfWork.ArticleRepository.AnyAsync(a => a.Slug == slug));

        article.CreatedById = currentAccount.AccountFirebaseId;

        article.PublishedAt = article.IsPublished ? DateTime.UtcNow.AddHours(7) : null;

        await _unitOfWork.ArticleRepository.AddAsync(article);

        await _unitOfWork.SaveChangesAsync();

        return article.Adapt<ArticleModel>();
    }

    public async Task UpdateArticle(string slug, UpdateArticleModel updateModel, AccountModel currentAccount)
    {
        var article = await _unitOfWork.ArticleRepository.FindFirstAsync(a => a.Slug == slug);

        if (article is null)
        {
            throw new NotFoundException("Article not found");
        }

        updateModel.Adapt(article);

        article.PublishedAt = article.IsPublished ? DateTime.UtcNow.AddHours(7) : null;

        article.Slug = await GenerateUniqueSlugAsync(article.Title,
            s => _unitOfWork.ArticleRepository.AnyAsync(a => a.Slug == s));

        article.UpdatedAt = DateTime.UtcNow.AddHours(7);
        article.UpdateById = currentAccount.AccountFirebaseId;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteArticle(string slug, AccountModel currentAccount)
    {
        var article = await _unitOfWork.ArticleRepository.FindFirstAsync(a => a.Slug == slug);

        if (article is null)
        {
            throw new NotFoundException("Article not found");
        }

        article.DeletedAt = DateTime.UtcNow.AddHours(7);
        article.RecordStatus = RecordStatus.IsDeleted;
        article.DeletedById = currentAccount.AccountFirebaseId;

        await _unitOfWork.SaveChangesAsync();
    }
}