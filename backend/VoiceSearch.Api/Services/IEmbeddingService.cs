using System.Threading.Tasks;

namespace VoiceSearch.Api.Services;

public interface IEmbeddingService
{
    Task<float[]> GetEmbeddingAsync(string text);
}
