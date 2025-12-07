using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace VoiceSearch.Api.Services;

public class AzureSttService : ISttService
{
    private readonly string? _key;
    private readonly string? _region;
    public AzureSttService(IConfiguration config)
    {
        _key = config["AzureSpeech:Key"] ?? config["AZURE_SPEECH_KEY"];
        _region = config["AzureSpeech:Region"] ?? config["AZURE_SPEECH_REGION"];
    }

    public async Task<string> RecognizeAsync(Stream audioStream, string language = "en-US")
    {
        if (string.IsNullOrWhiteSpace(_key) || string.IsNullOrWhiteSpace(_region))
            return string.Empty; // no azure configured - fallback to empty

        var speechConfig = SpeechConfig.FromSubscription(_key, _region);
        speechConfig.SpeechRecognitionLanguage = language;

        // NOTE: for brevity this sample uses RecognizeOnceAsync and requires a proper WAV stream.
        // In production adapt PushAudioInputStream to write bytes from audioStream.
        using var audioInput = AudioConfig.FromWavFileInput("temp.wav"); // placeholder
        using var recognizer = new SpeechRecognizer(speechConfig, audioInput);
        var result = await recognizer.RecognizeOnceAsync();
        if (result.Reason == ResultReason.RecognizedSpeech)
            return result.Text;
        return string.Empty;
    }
}
