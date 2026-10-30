using KdramasApp_BE.DTOs;
using KdramasApp_BE.Models;
using KdramasApp_BE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KdramasApp_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActorController : ControllerBase
{
    private readonly IActorService _actorService;

    public ActorController(IActorService actorService)
    {
        this._actorService = actorService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddActor(ActorRequest actorRequest)
    {
        //call the service
        ActorResponse response = await _actorService.AddActorAsync(actorRequest);
        //return the result
        return StatusCode(StatusCodes.Status201Created,
            new APIResponse("The actor was added successfully"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllActors()
    {
        //call the service
        List<ActorResponse> actors = await _actorService.GetAllActorsAsync();

        return StatusCode(StatusCodes.Status200OK, actors);
    }

    [HttpGet("get/{id:int}")]
    public async Task<IActionResult> GetActorById([FromRoute] int id)
    {
        // Call the service
        ActorResponse? response = await _actorService.GetActorByIdAsync(id);

        if (response == null)
        {
            return NotFound(new APIResponse("The actor was not found"));
        }

        return Ok(response);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> DeleteActor([FromRoute] int id)
    {
        //call the service
        var response = await _actorService.DeleteActorAsync(id);

        if (!response)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new APIResponse("The actor to delete was not found"));
        }

        return StatusCode(StatusCodes.Status200OK, 
            new APIResponse("The actor was deleted successfully"));
    }

    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> UpdateActor([FromRoute] int id, ActorRequest request)
    {
        //call the service
        var response = await _actorService.UpdateActorAsync(id, request);

        if (!response)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new APIResponse("The actor to update was not found"));
        }

        return StatusCode(StatusCodes.Status200OK,
            new APIResponse("The actor was updated successfully"));
    }
}
