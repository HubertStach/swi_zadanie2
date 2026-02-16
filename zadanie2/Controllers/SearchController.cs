using Microsoft.AspNetCore.Mvc;
using Zadanie2.Services;

namespace Zadanie2.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{
    private readonly SearchService _searchService;

    public SearchController(SearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string term, [FromQuery] int? limit)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest("Search term is required.");
        }

        var results = await _searchService.SearchAllAsync(term, limit);
        return Ok(results);
    }
}