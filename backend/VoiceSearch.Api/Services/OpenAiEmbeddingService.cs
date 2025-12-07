using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace VoiceSearch.Api.Services;

public class OpenAiEmbeddingService : IEmbeddingService
{
    private readonly string? _apiKey;
    private readonly IHttpClientFactory _factory;
    private readonly string _baseUrl = "https://api.openai.com/v1/embeddings";

    public OpenAiEmbeddingService(IConfiguration config, IHttpClientFactory factory)
    {
        _apiKey = config["OpenAI:ApiKey"] ?? config["OPENAI_API_KEY"];
        _factory = factory;
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException("OpenAI API key not configured.");

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

        var payload = new { model = "text-embedding-3-small", input = text };
        var res = await client.PostAsJsonAsync(_baseUrl, payload);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var arr = doc.RootElement.GetProperty("data")[0].GetProperty("embedding");
        var outArr = new float[arr.GetArrayLength()];
        for (int i = 0; i < arr.GetArrayLength(); i++) outArr[i] = arr[i].GetSingle();
        return outArr;
    }
}
