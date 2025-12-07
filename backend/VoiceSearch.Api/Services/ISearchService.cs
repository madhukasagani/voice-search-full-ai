using System.Collections.Generic;
using System.Threading.Tasks;
using VoiceSearch.Api.Models;

namespace VoiceSearch.Api.Services;

public interface ISearchService
{
    Task<IEnumerable<Product>> SemanticSearchAsync(float[] queryVector, int limit = 10);
    Task IndexProductEmbeddingAsync(int productId, float[] vector);
}
