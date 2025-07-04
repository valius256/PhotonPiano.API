using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.SurveyQuestion;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
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
                await _serviceFactory.SurveyQuestionService.GetPagedSurveyQuestions(
                    request.Adapt<QueryPagedSurveyQuestionsModel>());

            HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

            return pagedResult.Items;
        }

        [HttpGet("{id}")]
        [CustomAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Get Survey Question details")]
        public async Task<ActionResult<SurveyQuestionDetailsModel>> GetSurveyQuestionDetails([FromRoute] Guid id)
        {
            return await _serviceFactory.SurveyQuestionService.GetSurveyQuestionDetails(id);
        }

        [HttpGet("{id}/answers")]
        [CustomAuthorize(Roles = [Role.Staff, Role.Student])]
        [EndpointDescription("Get answers of a question")]
        public async Task<ActionResult<List<LearnerAnswerWithLearnerModel>>> GetAnswersOfQuestion([FromRoute] Guid id,
            [FromQuery] QueryPagedAnswersRequest request)
        {
            var pagedResult = await _serviceFactory.SurveyQuestionService.GetQuestionAnswers(id,
                request.Adapt<QueryPagedAnswersModel>(), base.CurrentAccount!);
            
            HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);
            
            return pagedResult.Items;
        }
        
        [HttpPost]
        [CustomAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Create Survey Question")]
        public async Task<ActionResult<SurveyQuestionModel>> CreateSurveyQuestion(
            [FromBody] CreateSurveyQuestionRequest request)
        {
            return Created(nameof(CreateSurveyQuestion),
                await _serviceFactory.SurveyQuestionService.CreateSurveyQuestion(
                    request.Adapt<CreateSurveyQuestionModel>(),
                    base.CurrentAccount!
                ));
        }

        [HttpPut("{id}")]
        [CustomAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Update Survey Question")]
        public async Task<ActionResult> UpdateSurveyQuestion([FromRoute] Guid id,
            [FromBody] UpdateSurveyQuestionRequest request)
        {
            await _serviceFactory.SurveyQuestionService.UpdateSurveyQuestion(id,
                request.Adapt<UpdateSurveyQuestionModel>(),
                base.CurrentAccount!);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CustomAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Delete Survey Question")]
        public async Task<ActionResult> DeleteSurveyQuestion([FromRoute] Guid id)
        {
            await _serviceFactory.SurveyQuestionService.DeleteSurveyQuestion(id, base.CurrentAccount!);

            return NoContent();
        }
    }
}