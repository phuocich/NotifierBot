using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;

namespace NotifierBot;

public class AudioService
{
    private readonly TextToSpeechClient _client;

    public AudioService()
    {
        var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            "key.json"
        );

        var credential = GoogleCredential
            .FromFile(path)
            .CreateScoped(TextToSpeechClient.DefaultScopes);

        _client = new TextToSpeechClientBuilder
        {
            Credential = credential
        }.Build();
    }

    public Task<MemoryStream> GetBhanteThichMinhChauAsync(Verse verse)
        => GenerateAsync(verse, verse.BhanteThichMinhChau);

    public Task<MemoryStream> GetBhanteIndacandaAsync(Verse verse)
        => GenerateAsync(verse, verse.BhanteIndacanda);

    private async Task<MemoryStream> GenerateAsync(Verse verse, string content)
    {
        var input = new SynthesisInput
        {
            Ssml = $@"<speak>
                    {Escape(Normalize(content))}
                    </speak>"
        };

        var voice = new VoiceSelectionParams
        {
            LanguageCode = "vi-VN",
            Name = "vi-VN-Wavenet-B"
        };

        var config = new AudioConfig
        {
            AudioEncoding = AudioEncoding.Mp3
        };

        var response = await _client.SynthesizeSpeechAsync(input, voice, config);

        var stream = new MemoryStream(response.AudioContent.ToByteArray());
        stream.Position = 0;

        return stream;
    }

    private string Normalize(string text)
    {
        return text
            .Replace("\n", ". ")
            .Replace("  ", " ")
            .Trim();
    }

    private string Escape(string text)
    {
        return System.Security.SecurityElement.Escape(text) ?? "";
    }
}