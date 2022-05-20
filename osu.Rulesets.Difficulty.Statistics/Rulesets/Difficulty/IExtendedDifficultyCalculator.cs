using JetBrains.Annotations;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;

namespace osu.Rulesets.Difficulty.Statistics.Rulesets.Difficulty;

public interface IExtendedDifficultyCalculator
{
    public Skill[] GetSkills(IBeatmap beatmap, Mod[] mods, double clockRate);

    public DifficultyAttributes GetDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate);

    public DifficultyAttributes Calculate(CancellationToken cancellationToken = default);

    public DifficultyAttributes Calculate([NotNull] IEnumerable<Mod> mods, CancellationToken cancellationToken = default);
}