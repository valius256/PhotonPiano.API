using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.News;
using PhotonPiano.BusinessLogic.BusinessModel.News;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers
{
    [Route("api/articles")]
    [ApiController]
    public class ArticlesController : BaseController
    {
        private readonly IServiceFactory _serviceFactory;

        public ArticlesController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpGet]
        [EndpointDescription("Get articles with paging")]
        public async Task<ActionResult<List<ArticleModel>>> GetArticles([FromQuery] QueryPagedArticlesRequest request)
        {
            var pagedResult =
                await _serviceFactory.ArticleService.GetArticles(request.Adapt<QueryPagedArticlesModel>(),
                    base.CurrentAccount);

            HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

            return pagedResult.Items;
        }

        [HttpGet("{slug}")]
        [EndpointDescription("Get article details by slug")]
        public async Task<ActionResult<ArticleModel>> GetArticle([FromRoute] string slug)
        {
            return await _serviceFactory.ArticleService.GetArticleDetailsBySlug(slug);
        }

        [HttpPost]
        [CustomAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Create new article")]
        public async Task<ActionResult<ArticleModel>> CreateNewArticle([FromBody] CreateArticleRequest request)
        {
            return Created(nameof(CreateNewArticle),
                await _serviceFactory.ArticleService.CreateArticle(request.Adapt<CreateArticleModel>(),
                    base.CurrentAccount!));
        }

        [HttpPut("{slug}")]
        [CustomAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Update article")]
        public async Task<ActionResult> UpdateArticle([FromRoute] string slug, [FromBody] UpdateArticleRequest request)
        {
            await _serviceFactory.ArticleService.UpdateArticle(slug, request.Adapt<UpdateArticleModel>(),
                base.CurrentAccount!);

            return NoContent();
        }

        [HttpDelete("{slug}")]
        [CustomAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Delete article")]
        public async Task<ActionResult> DeleteArticle([FromRoute] string slug)
        {
            await _serviceFactory.ArticleService.DeleteArticle(slug, base.CurrentAccount!);

            return NoContent();
        }
    }
}