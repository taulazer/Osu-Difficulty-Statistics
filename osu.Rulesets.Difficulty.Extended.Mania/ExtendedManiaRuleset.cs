using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Mania.Edit;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osu.Rulesets.Difficulty.Extended.Mania.Difficulty;
using osu.Rulesets.Difficulty.Extended.Mania.Performance;
using osu.Rulesets.Difficulty.Statistics.Rulesets;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Difficulty;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Performance;

namespace osu.Rulesets.Difficulty.Extended.Mania;

public class ExtendedManiaRuleset : ManiaRuleset, IExtendedRuleset
{
    public IExtendedDifficultyCalculator CreateExtendedDifficultyCalculator(WorkingBeatmap beatmap)
        => new ExtendedManiaDifficultyCalculator(RulesetInfo, beatmap);

    public IExtendedPerformanceCalculator CreateExtendedPerformanceCalculator()
        => new ExtendedManiaPerformanceCalculator();

    public DrawableRuleset CreateDebuggerRuleset(IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
        => new DrawableManiaEditorRuleset(this, beatmap, mods);
}