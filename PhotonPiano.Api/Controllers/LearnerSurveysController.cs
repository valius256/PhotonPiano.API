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
    [Route("api/learner-surveys")]
    [ApiController]
    public class LearnerSurveysController : ControllerBase
    {
        private readonly IServiceFactory _serviceFactory;

        public LearnerSurveysController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpGet]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Get surveys with paging")]
        public async Task<ActionResult<List<LearnerSurveyModel>>> GetSurveys([FromQuery] QueryPagedSurveysRequest request)
        {
            var pagedResult =
                await _serviceFactory.LearnerSurveyService.GetPagedSurveys(request.Adapt<QueryPagedSurveysModel>());

            HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

            return pagedResult.Items;
        }

        [HttpPost]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Create new survey")]
        public async Task<ActionResult> CreateSurvey()
        {
            return Created();
        }
    }
}