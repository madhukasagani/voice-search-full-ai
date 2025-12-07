using System.IO;
using System.Threading.Tasks;

namespace VoiceSearch.Api.Services;

public interface ISttService
{
    Task<string> RecognizeAsync(Stream audioStream, string language = "en-US");
}
