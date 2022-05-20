using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Taiko;
using osu.Game.Rulesets.Taiko.UI;
using osu.Game.Rulesets.UI;
using osu.Rulesets.Difficulty.Extended.Taiko.Difficulty;
using osu.Rulesets.Difficulty.Extended.Taiko.Performance;
using osu.Rulesets.Difficulty.Statistics.Rulesets;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Difficulty;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Performance;

namespace osu.Rulesets.Difficulty.Extended.Taiko;

public class ExtendedTaikoRuleset : TaikoRuleset, IExtendedRuleset
{
    public IExtendedDifficultyCalculator CreateExtendedDifficultyCalculator(WorkingBeatmap beatmap)
        => new ExtendedTaikoDifficultyCalculator(RulesetInfo, beatmap);

    public IExtendedPerformanceCalculator CreateExtendedPerformanceCalculator()
        => new ExtendedTaikoPerformanceCalculator();

    public DrawableRuleset CreateDebuggerRuleset(IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
        => new DrawableTaikoRuleset(this, beatmap, mods);
}