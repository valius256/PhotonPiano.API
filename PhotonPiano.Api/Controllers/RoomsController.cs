using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Room;
using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : BaseController
{
    private readonly IServiceFactory _serviceFactory;

    public RoomsController(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    [HttpGet]
    [CustomAuthorize(Roles = [Role.Staff, Role.Administrator, Role.Instructor])]
    [EndpointDescription("Get rooms with paging")]
    public async Task<ActionResult<List<RoomDetailModel>>> GetRooms(
        [FromQuery] QueryRoomRequest request)
    {
        var pagedResult = await _serviceFactory.RoomService.GetPagedRooms(request.Adapt<QueryRoomModel>());

        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);
        return pagedResult.Items;
    }

    [HttpGet("{id}")]
    [EndpointDescription("Get room by id")]
    public async Task<ActionResult<RoomDetailModel>> GetEntranceTestById([FromRoute] Guid id)
    {
        return Ok(await _serviceFactory.RoomService.GetRoomDetailById(id));
    }

    [HttpPost]
    [CustomAuthorize(Roles = [Role.Administrator])]
    [EndpointDescription("Create a room")]
    public async Task<ActionResult<RoomDetailModel>> CreateRoom(
        [FromBody] CreateRoomRequest request)
    {
        var result =
            await _serviceFactory.RoomService.CreateRoom(
                request.Adapt<RoomModel>(), CurrentAccountId);
        return Created(nameof(CreateRoom), result);
    }

    [HttpDelete("{id}")]
    [CustomAuthorize(Roles = [Role.Administrator])]
    [EndpointDescription("Delete a room")]
    public async Task<ActionResult> DeleteRoom([FromRoute] Guid id)
    {
        await _serviceFactory.RoomService.DeleteRoom(id, CurrentAccountId);
        return Ok();
    }

    [HttpPut("{id}")]
    [CustomAuthorize(Roles = [Role.Administrator])]
    [EndpointDescription("Update a room")]
    public async Task<ActionResult> UpdateRoom([FromRoute] Guid id, [FromBody] UpdateRoomRequest request)
    {
        await _serviceFactory.RoomService.UpdateRoom(id, request.Adapt<UpdateRoomModel>(), CurrentAccountId);

        return NoContent();
    }
} 