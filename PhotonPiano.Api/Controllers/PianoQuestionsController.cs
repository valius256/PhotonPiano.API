using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Requests.SurveyQuestion;
using PhotonPiano.BusinessLogic.BusinessModel.PianoQuestion;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers
{
    [Route("api/piano-questions")]
    [ApiController]
    public class PianoQuestionsController : BaseController
    {
        private readonly IServiceFactory _serviceFactory;

        public PianoQuestionsController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpGet]
        [EndpointDescription("Get all piano questions")]
        public async Task<ActionResult<List<PianoQuestionModel>>> GetSurveyQuestions()
        {
            return await _serviceFactory.PianoQuestionService.GetCachedAllSurveyQuestions();
        }

        [HttpGet("{id}")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Get piano question details")]
        public async Task<ActionResult<PianoQuestionDetailsModel>> GetSurveyQuestionDetails([FromRoute] Guid id)
        {
            return await _serviceFactory.PianoQuestionService.GetSurveyQuestionDetails(id);
        }

        [HttpPost]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Create piano question")]
        public async Task<ActionResult> CreatePianoQuestion([FromBody] CreatePianoQuestionRequest request)
        {
            return Created(nameof(CreatePianoQuestion),
                await _serviceFactory.PianoQuestionService.CreatePianoQuestion(
                    request.Adapt<CreatePianoQuestionModel>(),
                    base.CurrentAccount!
                ));
        }

        [HttpPut("{id}")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Update piano question")]
        public async Task<ActionResult> UpdateSurveyQuestion([FromRoute] Guid id,
            [FromBody] UpdatePianoQuestionRequest request)
        {
            await _serviceFactory.PianoQuestionService.UpdatePianoQuestion(id,
                request.Adapt<UpdatePianoQuestionModel>(),
                base.CurrentAccount!);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Delete piano question")]
        public async Task<ActionResult> DeletePianoQuestion([FromRoute] Guid id)
        {
            await _serviceFactory.PianoQuestionService.DeletePianoQuestion(id, base.CurrentAccount!);
            
            return NoContent();
        }
    }
}