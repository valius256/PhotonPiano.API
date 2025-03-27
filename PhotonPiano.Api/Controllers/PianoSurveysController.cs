using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Survey;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers
{
    [Route("api/piano-surveys")]
    [ApiController]
    public class PianoSurveysController : BaseController
    {
        private readonly IServiceFactory _serviceFactory;

        public PianoSurveysController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpGet]
        [FirebaseAuthorize(Roles = [Role.Staff, Role.Student])]
        [EndpointDescription("Get surveys with paging")]
        public async Task<ActionResult<List<PianoSurveyModel>>> GetSurveys([FromQuery] QueryPagedSurveysRequest request)
        {
            var pagedResult =
                await _serviceFactory.PianoSurveyService.GetSurveys(request.Adapt<QueryPagedSurveysModel>(),
                    base.CurrentAccount!);

            HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

            return pagedResult.Items;
        }

        [HttpGet("{id}")]
        [FirebaseAuthorize(Roles = [Role.Staff, Role.Student])]
        [EndpointDescription("Get survey by id")]
        public async Task<ActionResult<PianoSurveyDetailsModel>> GetPianoSurveyDetails([FromRoute] Guid id)
        {
            return await _serviceFactory.PianoSurveyService.GetSurveyDetails(id, base.CurrentAccount!);
        }

        [HttpGet("entrance-survey")]
        [EndpointDescription("Get entrance survey")]
        public async Task<ActionResult<PianoSurveyDetailsModel>> GetEntranceSurveyInfo()
        {
            return await _serviceFactory.PianoSurveyService.GetEntranceSurvey();
        }

        [HttpPost]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Create piano survey")]
        public async Task<ActionResult<PianoSurveyDetailsModel>> CreatePianoSurvey(
            [FromBody] CreatePianoSurveyRequest request)
        {
            return Created(nameof(CreatePianoSurvey),
                await _serviceFactory.PianoSurveyService.CreatePianoSurvey(request.Adapt<CreatePianoSurveyModel>(),
                    base.CurrentAccount!));
        }

        [HttpPut("{id}")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Update piano survey")]
        public async Task<ActionResult> UpdatePianoSurvey([FromRoute] Guid id,
            [FromBody] UpdatePianoSurveyRequest request)
        {
            await _serviceFactory.PianoSurveyService.UpdatePianoSurvey(id, request.Adapt<UpdatePianoSurveyModel>(),
                base.CurrentAccount!);

            return NoContent();
        }

        [HttpPost("entrance-survey/answers")]
        [EndpointDescription("Send learner answers for entrance survey")]
        public async Task<ActionResult<PianoSurveyDetailsModel>> CreateAnswersForPianoSurvey(
            [FromBody] SendEntranceSurveyAnswersRequest request)
        {
            await _serviceFactory.PianoSurveyService.SendEntranceSurveyAnswers(
                request.Adapt<SendEntranceSurveyAnswersModel>());
            return Ok();
        }
    }
}