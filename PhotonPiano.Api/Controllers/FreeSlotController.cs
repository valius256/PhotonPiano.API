using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Requests.Account;
using PhotonPiano.Api.Requests.FreeSlot;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.FreeSlot;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers
{
    [ApiController]
    [Route("api/free-slots")]
    public class FreeSlotController : BaseController
    {
        private readonly IServiceFactory _serviceFactory;

        public FreeSlotController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpGet("all")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Get free slots of all students")]
        public async Task<ActionResult<List<FreeSlotModel>>> GetAllFreeSlot()
        {
            return await _serviceFactory.FreeSlotService.GetAllFreeSlots();
        }

        [HttpGet]
        [FirebaseAuthorize(Roles = [Role.Student])]
        [EndpointDescription("Get free slots of a student")]
        public async Task<ActionResult<List<FreeSlotModel>>> GetFreeSlot()
        {
            return await _serviceFactory.FreeSlotService.GetFreeSlots(CurrentUserFirebaseId);
        }

        [HttpPost]
        [FirebaseAuthorize(Roles = [Role.Student])]
        [EndpointDescription("Upsert student's free slot")]
        public async Task<IActionResult> UpsertFreeSlots([FromBody] CreateFreeSlotRequest request)
        {
            await _serviceFactory.FreeSlotService.UpsertFreeSlots(request.CreateFreeSlotModels,CurrentUserFirebaseId);
            return Ok();
        }
    }
}
