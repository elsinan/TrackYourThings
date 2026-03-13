using backend.Auth;
using backend.TrackedItems.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.TrackedItems;

[Route("api/[controller]")]
[ApiController]
public class TrackedItemsController : ControllerBase
{

    private readonly ITrackedItemService _itemService;
    private readonly IPasskeyService _passkeyService;

    public TrackedItemsController(ITrackedItemService itemService, IPasskeyService passkeyService)
    {
        _itemService = itemService;
        _passkeyService = passkeyService;

    }


    /// <summary>
    /// Retrieves all items.
    /// </summary>
    /// <returns>A list of TrackedItemResponse objects.</returns>
    [ProducesResponseType<List<TrackedItemResponse>>(200)]
    [ProducesResponseType<string>(500)]
    [HttpGet]
    public async Task<ActionResult<List<TrackedItemResponse>>> GetAllTrackedItems()
    {
        var sessionId = Request.Cookies["sid"];
        if (sessionId == null) return Unauthorized();

        var user = await _passkeyService.GetUserBySessionAsync(sessionId);
        if (user == null) return Unauthorized();

        var items = await _itemService.GetAllTrackedItems(user.Id);

        var response = items.Select(item => new TrackedItemResponse
        (
            ItemId: item.Item1,
            Title: item.Item2

        )).ToList();

        return Ok(response);

    }

    /// <summary>
    /// Creates a new tracked item.
    /// </summary>
    /// <param name="name">The name of the tracked item to create.</param>
    /// <returns>The created TrackedItemResponse object.</returns>
    [ProducesResponseType<TrackedItemResponse>(200)]
    [ProducesResponseType(401)]
    [HttpPost]
    [Route("create/{name}")]
    public async Task<ActionResult<TrackedItemResponse>> CreateTrackedItem([FromRoute] string name)
    {
        var sessionId = Request.Cookies["sid"];
        if (sessionId == null) return Unauthorized();

        var user = await _passkeyService.GetUserBySessionAsync(sessionId);
        if (user == null) return Unauthorized();

        var item = await _itemService.CreateTrackedItem(user.Id, name);

        var response = new TrackedItemResponse
        (
            ItemId: item.Id,
            Title: item.Title
        );

        return Ok(response);
    }
}