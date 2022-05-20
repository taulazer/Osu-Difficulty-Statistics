using osu.Game.Beatmaps;
using osu.Game.Rulesets.Osu;
using osu.Rulesets.Difficulty.Extended.Osu.Difficulty;
using osu.Rulesets.Difficulty.Extended.Osu.Performance;
using osu.Rulesets.Difficulty.Statistics.Rulesets;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Difficulty;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Performance;

namespace osu.Rulesets.Difficulty.Extended.Osu;

public class ExtendedOsuRuleset : OsuRuleset, IExtendedRuleset
{
    public IExtendedDifficultyCalculator CreateExtendedDifficultyCalculator(WorkingBeatmap beatmap)
        => new ExtendedOsuDifficultyCalculator(RulesetInfo, beatmap);

    public IExtendedPerformanceCalculator CreateExtendedPerformanceCalculator()
        => new ExtendedOsuPerformanceCalculator();
}