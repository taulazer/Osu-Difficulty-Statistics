using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Scoring;

namespace osu.Rulesets.Difficulty.Statistics.Rulesets.Performance;

/// <summary>
/// An extended performance calculator used to access protected fields and or custom debug information.
/// </summary>
public interface IExtendedPerformanceCalculator
{
    public PerformanceAttributes Calculate(ScoreInfo score, DifficultyAttributes attributes);
    public PerformanceAttributes Calculate(ScoreInfo score, IWorkingBeatmap beatmap);
}