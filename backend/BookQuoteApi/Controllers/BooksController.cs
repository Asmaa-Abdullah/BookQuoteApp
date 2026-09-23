using BookQuoteApi.DTOs.Books;
using BookQuoteApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookQuoteApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookResponse>>> GetAll()
    {
        var books = await _bookService.GetAllAsync();

        return Ok(books);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookResponse>> GetById(int id)
    {
        var book = await _bookService.GetByIdAsync(id);

        if (book is null)
        {
            return NotFound(new { message = "Book not found." });
        }

        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookResponse>> Create(
        CreateBookRequest request)
    {
        var book = await _bookService.CreateAsync(request);

        return Created(
            $"/api/books/{book.Id}",
            book);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookResponse>> Update(
        int id,
        UpdateBookRequest request)
    {
        var book = await _bookService.UpdateAsync(id, request);

        if (book is null)
        {
            return NotFound(new { message = "Book not found." });
        }

        return Ok(book);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _bookService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new { message = "Book not found." });
        }

        return NoContent();
    }
}