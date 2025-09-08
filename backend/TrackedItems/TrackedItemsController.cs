using backend.TrackedItems.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.TrackedItems;

public class TrackedItemsController(ITrackedItemService itemService) : ControllerBase
{

    private readonly ITrackedItemService _itemService = itemService;

    /// <summary>
    /// Retrieves all items.
    /// </summary>
    /// <returns>A list of TrackedItemResponse objects.</returns>
    [ProducesResponseType<List<TrackedItemResponse>>(200)]
    [ProducesResponseType<string>(500)]
    [HttpGet]
    public async Task<ActionResult<List<TrackedItemResponse>>> GetAllTrackedItems()
    {
        var items = await _itemService.GetAllTrackedItems();

        var response = items.Select(item => new TrackedItemResponse
        (
            ItemId: item.Item1,
            Title: item.Item2
            
        )).ToList();

        return Ok(response);
    }
}