using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Difficulty;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Performance;

namespace osu.Rulesets.Difficulty.Statistics.Rulesets;

public interface IExtendedRuleset
{
    public IExtendedDifficultyCalculator CreateExtendedDifficultyCalculator(WorkingBeatmap beatmap);

    public IExtendedPerformanceCalculator CreateExtendedPerformanceCalculator();

    public DrawableRuleset CreateDebuggerRuleset(IBeatmap beatmap, IReadOnlyList<Mod> mods = null);
}