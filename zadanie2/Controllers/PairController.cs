using Microsoft.AspNetCore.Mvc;
using Zadanie2.Services;

namespace Zadanie2.Controllers;

[ApiController]
[Route("top-pairs")]
public class PairsController : ControllerBase
{
    private readonly PairsService _pairsService;

    public PairsController(PairsService pairsService)
    {
        _pairsService = pairsService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? min, [FromQuery] int? max, [FromQuery] int limit = 20)
    {
        var results = await _pairsService.GetTopPairsAsync(min, max, limit);
        return Ok(results);
    }
}