using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Difficulty;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Performance;
using osu.Rulesets.Difficulty.Statistics.Screens.Calculations.Screens;

namespace osu.Rulesets.Difficulty.Statistics.Rulesets;

/// <summary>
/// An extended version of any ruleset that inherits this interface.
/// This purely only builds on top of the ruleset and exposes most functions for the statistics app to use.
/// </summary>
public interface IExtendedRuleset
{
    /// <summary>
    /// An extended difficulty calculator that could give more debug information for the statistics app to display.
    /// </summary>
    public IExtendedDifficultyCalculator CreateExtendedDifficultyCalculator(WorkingBeatmap beatmap);

    /// <summary>
    /// An extended performance calculator that could give more debug information for the statistics app to display. 
    /// </summary>
    public IExtendedPerformanceCalculator CreateExtendedPerformanceCalculator();

    /// <summary>
    /// A debug drawable ruleset that is used inside of the <see cref="DifficultyDebuggerScreen"/>.
    /// Commonly, the drawable editor ruleset will be sufficient enough. 
    /// </summary>
    public DrawableRuleset CreateDebuggerRuleset(IBeatmap beatmap, IReadOnlyList<Mod> mods = null);
}