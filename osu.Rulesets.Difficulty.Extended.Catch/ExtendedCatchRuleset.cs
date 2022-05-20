using osu.Game.Beatmaps;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Catch.Edit;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osu.Rulesets.Difficulty.Extended.Catch.Difficulty;
using osu.Rulesets.Difficulty.Extended.Catch.Performance;
using osu.Rulesets.Difficulty.Statistics.Rulesets;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Difficulty;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Performance;

namespace osu.Rulesets.Difficulty.Extended.Catch;

public class ExtendedCatchRuleset : CatchRuleset, IExtendedRuleset
{
    public IExtendedDifficultyCalculator CreateExtendedDifficultyCalculator(WorkingBeatmap beatmap)
        => new ExtendedCatchDifficultyCalculator(RulesetInfo, beatmap);

    public IExtendedPerformanceCalculator CreateExtendedPerformanceCalculator()
        => new ExtendedCatchPerformanceCalculator();

    public DrawableRuleset CreateDebuggerRuleset(IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
        => new DrawableCatchEditorRuleset(this, beatmap, mods);
}