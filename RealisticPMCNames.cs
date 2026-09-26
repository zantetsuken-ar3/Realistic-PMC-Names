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
    public SemanticVersioning.Version Version { get; init; } = new("1.1.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.0");
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; }
    public string License { get; init; } = "MIT";
    public bool HasPrepatcher { get; init; } = false;
}

[Injectable(TypePriority = OnLoadOrder.RagfairCallbacks - 1)]
public class FleaNamePreloader(ISptLogger<FleaNamePreloader> logger, BotTable botTable) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var baseDir = NameFiles.BaseDir;
            var useVendors = NameFiles.LoadConfig(logger, Path.Combine(baseDir, "config", "config.json"));
            if (!botTable.Types.TryGetValue("usec", out var uBot) || uBot is null ||
                !botTable.Types.TryGetValue("bear", out var bBot) || bBot is null)
            {
                logger.Error("[Realistic PMC Names] Could not find PMC bot templates for Flea seller setup.");
                return Task.CompletedTask;
            }

            if (useVendors)
            {
                var fleaNames = NameFiles.LoadNames(Path.Combine(baseDir, "config", "flea_names.json"));
                NameFiles.ReplaceNames(uBot.FirstNames, fleaNames);
                NameFiles.ReplaceNames(bBot.FirstNames, fleaNames);
                logger.Success($"[Realistic PMC Names] Loaded {fleaNames.Count} Flea vendor identities for Ragfair cache.");
            }
            else
            {
                var uNames = NameFiles.LoadNames(Path.Combine(baseDir, "config", "u_names.json"));
                var bNames = NameFiles.LoadNames(Path.Combine(baseDir, "config", "b_names.json"));
                NameFiles.ReplaceNames(uBot.FirstNames, uNames);
                NameFiles.ReplaceNames(bBot.FirstNames, bNames);
                logger.Success("[Realistic PMC Names] Using realistic PMC identities for Flea sellers.");
            }
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) { logger.Error($"[Realistic PMC Names] Flea seller setup failed: {ex}"); }
        return Task.CompletedTask;
    }
}



// Ragfair's update callback is transient, so its BotHelper owns a fresh per-instance
// PMC-name cache. Put the Flea population into BotTable immediately before Ragfair's
// periodic update and restore the real PMC population immediately afterwards.
// This keeps regenerated Flea offers on the dedicated vendor list without shortening
// or otherwise changing the U-side/B-side datasets.
[Injectable(InjectionType.Transient, int.MaxValue, TypePriority = OnLoadOrder.RagfairCallbacks - 1)]
public class FleaRuntimeNamePreloader(ISptLogger<FleaRuntimeNamePreloader> logger, BotTable botTable) : IOnUpdate
{
    private bool? _useVendors;
    private List<string>? _fleaNames;

    public Task<bool> OnUpdateAsync(long secondsSinceLastRun, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            _useVendors ??= NameFiles.LoadConfig(logger, Path.Combine(NameFiles.BaseDir, "config", "config.json"));
            if (_useVendors != true)
                return Task.FromResult(false);

            _fleaNames ??= NameFiles.LoadNames(Path.Combine(NameFiles.BaseDir, "config", "flea_names.json"));
            if (botTable.Types.TryGetValue("usec", out var uBot) && uBot is not null &&
                botTable.Types.TryGetValue("bear", out var bBot) && bBot is not null)
            {
                NameFiles.ReplaceNames(uBot.FirstNames, _fleaNames);
                NameFiles.ReplaceNames(bBot.FirstNames, _fleaNames);
            }
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) { logger.Error($"[Realistic PMC Names] Runtime Flea vendor setup failed: {ex}"); }

        // False means SPT does not throttle this helper; it must run immediately before every Ragfair update pass.
        return Task.FromResult(false);
    }
}

[Injectable(InjectionType.Transient, int.MaxValue, TypePriority = OnLoadOrder.RagfairCallbacks + 1)]
public class FleaRuntimeNameRestorer(ISptLogger<FleaRuntimeNameRestorer> logger, BotTable botTable) : IOnUpdate
{
    private List<string>? _uNames;
    private List<string>? _bNames;

    public Task<bool> OnUpdateAsync(long secondsSinceLastRun, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!NameFiles.LoadConfig(logger, Path.Combine(NameFiles.BaseDir, "config", "config.json")))
                return Task.FromResult(false);

            _uNames ??= NameFiles.LoadNames(Path.Combine(NameFiles.BaseDir, "config", "u_names.json"));
            _bNames ??= NameFiles.LoadNames(Path.Combine(NameFiles.BaseDir, "config", "b_names.json"));
            if (botTable.Types.TryGetValue("usec", out var uBot) && uBot is not null &&
                botTable.Types.TryGetValue("bear", out var bBot) && bBot is not null)
            {
                NameFiles.ReplaceNames(uBot.FirstNames, _uNames);
                NameFiles.ReplaceNames(bBot.FirstNames, _bNames);
            }
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) { logger.Error($"[Realistic PMC Names] Runtime PMC-name restore failed: {ex}"); }

        return Task.FromResult(false);
    }
}

[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class RealisticPmcNames(ISptLogger<RealisticPmcNames> logger, BotTable botTable) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var uNames = NameFiles.LoadNames(Path.Combine(NameFiles.BaseDir, "config", "u_names.json"));
            var bNames = NameFiles.LoadNames(Path.Combine(NameFiles.BaseDir, "config", "b_names.json"));
            if (!botTable.Types.TryGetValue("usec", out var uBot) || uBot is null ||
                !botTable.Types.TryGetValue("bear", out var bBot) || bBot is null)
            {
                logger.Error("[Realistic PMC Names] Could not find PMC bot templates. No PMC names were changed.");
                return Task.CompletedTask;
            }
            NameFiles.ReplaceNames(uBot.FirstNames, uNames);
            NameFiles.ReplaceNames(bBot.FirstNames, bNames);
            logger.Success($"[Realistic PMC Names] Loaded {uNames.Count} U-side and {bNames.Count} B-side identities.");
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) { logger.Error($"[Realistic PMC Names] Failed to load PMC identities: {ex}"); }
        return Task.CompletedTask;
    }
}

internal sealed class ModConfig { public bool UseFleaVendorNames { get; set; } = true; }

internal static class NameFiles
{
    public static string BaseDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;

    public static bool LoadConfig<T>(ISptLogger<T> logger, string path)
    {
        if (!File.Exists(path)) return true;
        try
        {
            var config = JsonSerializer.Deserialize<ModConfig>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return config?.UseFleaVendorNames ?? true;
        }
        catch (Exception ex)
        {
            logger.Warning($"[Realistic PMC Names] Could not read config.json; defaulting useFleaVendorNames to true. {ex.Message}");
            return true;
        }
    }

    public static List<string> LoadNames(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException($"Name file not found: {path}");
        var names = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(path)) ?? throw new InvalidDataException($"Could not read names from: {path}");
        names = names.Select(n => n.Trim()).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (names.Count == 0) throw new InvalidDataException($"Name list is empty: {path}");
        return names;
    }

    public static void ReplaceNames(ICollection<string> target, IEnumerable<string> names)
    {
        target.Clear();
        foreach (var name in names) target.Add(name);
    }
}
