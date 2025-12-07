using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoiceSearch.Api.Data;
using VoiceSearch.Api.Services;

namespace VoiceSearch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IEmbeddingService _embed;
    private readonly ISearchService _search;
    public SearchController(ApplicationDbContext db, IEmbeddingService embed, ISearchService search)
    {
        _db = db; _embed = embed; _search = search;
    }

    [HttpGet("text")]
    public async Task<IActionResult> TextSearch(string q, int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(q)) return BadRequest("q required");
        var res = await _db.Products
            .Where(p => EF.Functions.Like(p.Name, $"%{q}%") || EF.Functions.Like(p.Description, $"%{q}%"))
            .Take(limit).ToListAsync();
        return Ok(res);
    }

    [HttpGet("semantic")]
    public async Task<IActionResult> Semantic(string q, int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(q)) return BadRequest("q required");
        var vec = await _embed.GetEmbeddingAsync(q);
        var results = await _search.SemanticSearchAsync(vec, limit);
        return Ok(results);
    }
}
