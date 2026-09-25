using System.Reflection;
using System.Text.Json;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace RealisticPMCNames;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.zantetsukenar3.realisticpmcnames";
    public string Name { get; init; } = "Realistic PMC Names";
    public string Author { get; init; } = "ZantetsukenAR3";
    public List<string>? Contributors { get; init; }
    public SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.0");
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; }
    public string License { get; init; } = "MIT";
    public bool HasPrepatcher { get; init; } = false;
}

[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class RealisticPmcNames(
    ISptLogger<RealisticPmcNames> logger,
    BotTable botTable) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                          ?? AppContext.BaseDirectory;

            var uNames = LoadNames(Path.Combine(baseDir, "config", "u_names.json"));
            var bNames = LoadNames(Path.Combine(baseDir, "config", "b_names.json"));

            // These are SPT's internal database keys and must not be renamed.
            if (!botTable.Types.TryGetValue("usec", out var uBot) || uBot is null)
            {
                logger.Error("[Realistic PMC Names] Could not find U*** bot template. No names were changed.");
                return Task.CompletedTask;
            }

            if (!botTable.Types.TryGetValue("bear", out var bBot) || bBot is null)
            {
                logger.Error("[Realistic PMC Names] Could not find B*** bot template. No names were changed.");
                return Task.CompletedTask;
            }

            uBot.FirstNames.Clear();
            foreach (var name in uNames)
                uBot.FirstNames.Add(name);

            bBot.FirstNames.Clear();
            foreach (var name in bNames)
                bBot.FirstNames.Add(name);

            logger.Success($"[Realistic PMC Names] Loaded {uNames.Count} U*** names and {bNames.Count} B*** names.");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.Error($"[Realistic PMC Names] Failed to load: {ex}");
        }

        return Task.CompletedTask;
    }

    private static List<string> LoadNames(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Name file not found: {path}");

        var names = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(path))
                    ?? throw new InvalidDataException($"Could not read names from: {path}");

        names = names
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (names.Count == 0)
            throw new InvalidDataException($"Name list is empty: {path}");

        return names;
    }
}
