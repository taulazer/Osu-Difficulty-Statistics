using Humanizer;
using Newtonsoft.Json;
using osu.Game.Rulesets.Difficulty;

namespace osu.Rulesets.Difficulty.Statistics.Rulesets;

public static class CalculationAttributesExtensions
{
    public static IEnumerable<KeyValuePair<string, object>> ToReadableEnumerable(this DifficultyAttributes attributes)
    {
        var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(attributes)) ?? new Dictionary<string, object>();
        return json.Select(x => new KeyValuePair<string, object>(x.Key.Humanize().ToLowerInvariant(), x.Value));
    }
    
    public static IEnumerable<KeyValuePair<string, object>> ToReadableEnumerable(this PerformanceAttributes attributes)
    {
        var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(attributes)) ?? new Dictionary<string, object>();
        return json.Select(x => new KeyValuePair<string, object>(x.Key.Humanize().ToLowerInvariant(), x.Value));
    }
}