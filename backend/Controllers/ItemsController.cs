using LostAndFound.Api.DTOs;
using LostAndFound.Api.Models;
using LostAndFound.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

[ApiController]
public sealed class ItemsController(IItemService itemService) : ControllerBase
{
    [HttpPost("/lost-items")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ItemResponse>> CreateLostItem(
        CreateItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await itemService.CreateLostItemAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("/found-items")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ItemResponse>> CreateFoundItem(
        CreateItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await itemService.CreateFoundItemAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("/items")]
    [ProducesResponseType(typeof(IReadOnlyList<ItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<ItemResponse>>> GetItems(
        [FromQuery] ItemStatus? status,
        [FromQuery] string? location,
        [FromQuery] string? faculty,
        [FromQuery] DateOnly? date,
        CancellationToken cancellationToken)
    {
        var items = await itemService.GetItemsAsync(status, location, faculty, date, cancellationToken);
        return Ok(items);
    }

    [HttpGet("/items/{id:guid}")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemResponse>> GetItem(Guid id, CancellationToken cancellationToken)
    {
        var item = await itemService.GetItemAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost("/items/{id:guid}/claim")]
    [ProducesResponseType(typeof(ClaimResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClaimResponse>> ClaimItem(
        Guid id,
        CreateClaimRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var claim = await itemService.ClaimItemAsync(id, request, cancellationToken);
            return Created($"/items/{id}", claim);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("/items/{id:guid}/verify-owner")]
    [ProducesResponseType(typeof(ClaimResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClaimResponse>> VerifyOwner(
        Guid id,
        VerifyOwnerRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var claim = await itemService.VerifyOwnerAsync(id, request, cancellationToken);
            return claim is null ? NotFound() : Ok(claim);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("/items/{id:guid}/return")]
    [ProducesResponseType(typeof(ReturnRecordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReturnRecordResponse>> ReturnItem(
        Guid id,
        ReturnItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var returnRecord = await itemService.ReturnItemAsync(id, request, cancellationToken);
            return returnRecord is null ? NotFound() : Ok(returnRecord);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
