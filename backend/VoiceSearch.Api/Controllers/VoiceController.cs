using Microsoft.AspNetCore.Mvc;
using VoiceSearch.Api.Services;

namespace VoiceSearch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VoiceController : ControllerBase
{
    private readonly ISttService _stt;
    private readonly IEmbeddingService _embed;
    private readonly ISearchService _search;
    public VoiceController(ISttService stt, IEmbeddingService embed, ISearchService search)
    {
        _stt = stt; _embed = embed; _search = search;
    }

    [HttpPost("search")]
    public async Task<IActionResult> PostSearch([FromForm] IFormFile audioFile, [FromForm] string language = "en-US")
    {
        if (audioFile == null) return BadRequest("audioFile required");
        using var ms = new MemoryStream();
        await audioFile.CopyToAsync(ms);
        ms.Position = 0;
        var text = await _stt.RecognizeAsync(ms, language);
        if (string.IsNullOrWhiteSpace(text)) return Ok(new { query = "", results = new object[0] });
        var vec = await _embed.GetEmbeddingAsync(text);
        var results = await _search.SemanticSearchAsync(vec, 10);
        return Ok(new { query = text, results });
    }
}
