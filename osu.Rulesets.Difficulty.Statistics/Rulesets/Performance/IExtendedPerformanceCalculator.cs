using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Scoring;

namespace osu.Rulesets.Difficulty.Statistics.Rulesets.Performance;

public interface IExtendedPerformanceCalculator
{
    public PerformanceAttributes Calculate(ScoreInfo score, DifficultyAttributes attributes);
    public PerformanceAttributes Calculate(ScoreInfo score, IWorkingBeatmap beatmap);
}