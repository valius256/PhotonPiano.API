using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.SurveyQuestion;
using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers
{
    [Route("api/survey-questions")]
    [ApiController]
    public class SurveyQuestionsController : BaseController
    {
        private readonly IServiceFactory _serviceFactory;

        public SurveyQuestionsController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpGet]
        [EndpointDescription("Get Survey Questions")]
        public async Task<ActionResult<List<SurveyQuestionModel>>> GetSurveyQuestions(
            [FromQuery] QueryPagedSurveyQuestionRequest request)
        {
            var pagedResult =
                await _serviceFactory.SurveryQuestionService.GetPagedSurveyQuestions(
                    request.Adapt<QueryPagedSurveyQuestionsModel>());

            HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

            return pagedResult.Items;
        }

        [HttpGet("{id}")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Get Survey Question details")]
        public async Task<ActionResult<SurveyQuestionDetailsModel>> GetSurveyQuestionDetails([FromRoute] Guid id)
        {
            return await _serviceFactory.SurveryQuestionService.GetSurveyQuestionDetails(id);
        }

        [HttpPost]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Create Survey Question")]
        public async Task<ActionResult> CreateSurveyQuestion([FromBody] CreateSurveyQuestionRequest request)
        {
            return Created(nameof(CreateSurveyQuestion),
                await _serviceFactory.SurveryQuestionService.CreateSurveyQuestion(
                    request.Adapt<CreateSurveyQuestionModel>(),
                    base.CurrentAccount!
                ));
        }

        [HttpPut("{id}")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Update Survey Question")]
        public async Task<ActionResult> UpdateSurveyQuestion([FromRoute] Guid id,
            [FromBody] UpdateSurveyQuestionRequest request)
        {
            await _serviceFactory.SurveryQuestionService.UpdateSurveyQuestion(id,
                request.Adapt<UpdateSurveyQuestionModel>(),
                base.CurrentAccount!);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Delete Survey Question")]
        public async Task<ActionResult> DeleteSurveyQuestion([FromRoute] Guid id)
        {
            await _serviceFactory.SurveryQuestionService.DeleteSurveyQuestion(id, base.CurrentAccount!);
            
            return NoContent();
        }
    }
}