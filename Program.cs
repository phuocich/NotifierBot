using NotifierBot;
using System.Text;
using System.Text.Json;

var basePath = Directory.GetCurrentDirectory();

var dataPath = Path.Combine(basePath, "Data", "data.json");
var statePath = Path.Combine(basePath, "Data", "state.json");

Console.WriteLine($"Data path: {dataPath}");
Console.WriteLine($"State path: {statePath}");

if (!File.Exists(dataPath))
{
    Console.WriteLine("data.json not found.");
    return;
}

// load verses
var json = await File.ReadAllTextAsync(dataPath);

var verses = JsonSerializer.Deserialize<List<Verse>>(
    json,
    new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

if (verses == null || verses.Count == 0)
{
    Console.WriteLine("No verses found.");
    return;
}

// ensure state file exists
if (!File.Exists(statePath) || new FileInfo(statePath).Length == 0)
{
    var initState = new State();

    await File.WriteAllTextAsync(
        statePath,
        JsonSerializer.Serialize(initState, new JsonSerializerOptions { WriteIndented = true })
    );
}

// load state
var stateJson = await File.ReadAllTextAsync(statePath);

var state = JsonSerializer.Deserialize<State>(stateJson,
    new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    }) ?? new State();

Console.WriteLine($"LastVerse: {state.LastVerse}");
Console.WriteLine($"LastDate: {state.LastDate}");

// next verse
var nextVerse = state.LastVerse + 1;

if (nextVerse > verses.Count)
    nextVerse = 1;

var verse = verses.FirstOrDefault(x => x.Number == nextVerse);

if (verse == null)
{
    Console.WriteLine("Verse not found.");
    return;
}

// build message
var sb = new StringBuilder();

sb.AppendLine($"📜 Kinh Pháp Cú – Kệ {verse.Number}");
sb.AppendLine(verse.Chapter);
sb.AppendLine();

sb.AppendLine("🪷 Pāḷi (Chánh văn)");
sb.AppendLine();
sb.AppendLine(verse.Pali.Trim());
sb.AppendLine();

sb.AppendLine("📖 Việt dịch – Ngài Thích Minh Châu");
sb.AppendLine();
sb.AppendLine(verse.BhanteThichMinhChau.Trim());
sb.AppendLine();

sb.AppendLine("📖 Việt dịch – Ngài Indacanda");
sb.AppendLine();
sb.AppendLine(verse.BhanteIndacanda.Trim());

var message = sb.ToString();

var audioService = new AudioService();
var telegram = new TelegramService();

var bhanteThichMinhChauStream = await audioService.GetBhanteThichMinhChauAsync(verse);
var bhanteIndacandaStream = await audioService.GetBhanteIndacandaAsync(verse);

await telegram.SendMessage(message);

await telegram.SendAudio(
    bhanteThichMinhChauStream,
    $"{verse.Number}_BhanteThichMinhChau.mp3",
    $"📖 Ngài Thích Minh Châu – Kệ {verse.Number}"
);

await telegram.SendAudio(
    bhanteIndacandaStream,
    $"{verse.Number}_BhanteIndacanda.mp3",
    $"📖 Ngài Indacanda – Kệ {verse.Number}"
);

Console.WriteLine($"Sent verse {nextVerse}");

// update state
state.LastVerse = nextVerse;

await File.WriteAllTextAsync(
    statePath,
    JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true })
);

Console.WriteLine("State updated.");
