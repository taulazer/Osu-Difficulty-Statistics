using osu.Game.Beatmaps;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Difficulty;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Performance;

namespace osu.Rulesets.Difficulty.Statistics.Rulesets;

public interface IExtendedRuleset
{
    public IExtendedDifficultyCalculator CreateExtendedDifficultyCalculator(WorkingBeatmap beatmap);

    public IExtendedPerformanceCalculator CreateExtendedPerformanceCalculator();
}