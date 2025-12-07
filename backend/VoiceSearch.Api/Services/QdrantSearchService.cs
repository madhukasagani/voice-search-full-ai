using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using VoiceSearch.Api.Data;
using Microsoft.EntityFrameworkCore;
using VoiceSearch.Api.Models;

namespace VoiceSearch.Api.Services;

public class QdrantSearchService : ISearchService
{
    private readonly string _qdrantUrl;
    private readonly string _collection;
    private readonly IHttpClientFactory _factory;
    private readonly ApplicationDbContext _db;

    public QdrantSearchService(IConfiguration config, IHttpClientFactory factory, ApplicationDbContext db)
    {
        _qdrantUrl = config["Qdrant:Url"] ?? config["QDRANT_URL"] ?? "http://localhost:6333";
        _collection = config["Qdrant:Collection"] ?? "products";
        _factory = factory;
        _db = db;
    }

    private HttpClient Client => _factory.CreateClient();

    public async Task EnsureCollectionExistsAsync(int vectorSize)
    {
        var resp = await Client.GetAsync($"{_qdrantUrl}/collections/{_collection}");
        if (resp.IsSuccessStatusCode) return;

        var createPayload = new
        {
            vectors = new { size = vectorSize, distance = "Cosine" }
        };
        var createRes = await Client.PutAsJsonAsync($"{_qdrantUrl}/collections/{_collection}", createPayload);
        createRes.EnsureSuccessStatusCode();
    }

    public async Task IndexProductEmbeddingAsync(int productId, float[] vector)
    {
        var point = new
        {
            points = new[] {
                new { id = productId, vector = vector, payload = new { productId = productId } }
            }
        };
        var upsertRes = await Client.PostAsJsonAsync($"{_qdrantUrl}/collections/{_collection}/points?wait=true", point);
        upsertRes.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<Product>> SemanticSearchAsync(float[] queryVector, int limit = 10)
    {
        var searchPayload = new { vector = queryVector, top = limit };
        var res = await Client.PostAsJsonAsync($"{_qdrantUrl}/collections/{_collection}/points/search", searchPayload);
        if (!res.IsSuccessStatusCode) return new List<Product>();

        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var list = new List<Product>();
        if (!doc.RootElement.TryGetProperty("result", out var arr)) return list;

        foreach (var it in arr.EnumerateArray())
        {
            var id = it.GetProperty("id").GetInt32();
            var p = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (p != null) list.Add(p);
        }
        return list;
    }
}
