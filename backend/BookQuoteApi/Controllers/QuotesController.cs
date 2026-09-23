using System.Security.Claims;
using BookQuoteApi.DTOs.Quotes;
using BookQuoteApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookQuoteApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;

    public QuotesController(IQuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuoteResponse>>> GetMyQuotes()
    {
        var userId = GetCurrentUserId();

        var quotes = await _quoteService.GetMyQuotesAsync(userId);

        return Ok(quotes);
    }

    [HttpPost]
    public async Task<ActionResult<QuoteResponse>> Create(
        CreateQuoteRequest request)
    {
        var userId = GetCurrentUserId();

        var quote = await _quoteService.CreateAsync(
            userId,
            request);

        return Ok(quote);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<QuoteResponse>> Update(
        int id,
        UpdateQuoteRequest request)
    {
        var userId = GetCurrentUserId();

        var quote = await _quoteService.UpdateAsync(
            userId,
            id,
            request);

        if (quote is null)
        {
            return NotFound(new
            {
                message = "Quote not found."
            });
        }

        return Ok(quote);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();

        var deleted = await _quoteService.DeleteAsync(
            userId,
            id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Quote not found."
            });
        }

        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(ClaimTypes.Name)
            ?? throw new UnauthorizedAccessException(
                "User ID was not found in the token.");

        return int.Parse(userId);
    }
}