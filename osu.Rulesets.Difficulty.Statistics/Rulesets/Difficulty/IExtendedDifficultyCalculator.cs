using JetBrains.Annotations;
using Newtonsoft.Json.Serialization;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;

namespace osu.Rulesets.Difficulty.Statistics.Rulesets.Difficulty;

/// <summary>
/// An extended difficulty calculator used to access protected fields and or custom debug information.
/// </summary>
public interface IExtendedDifficultyCalculator
{
    /// <summary>
    /// Fetches the skills for this difficulty calculator.
    /// Commonly, calling <see cref="DifficultyCalculator.CreateSkills"/> is sufficient enough. 
    /// </summary>
    public Skill[] GetSkills(IBeatmap beatmap, Mod[] mods, double clockRate);

    /// <summary>
    /// Fetches difficulty attributes to display on the beatmap selection screen.
    /// Anything that has the <see cref="JsonProperty"/> will be displayed.
    /// Commonly, <see cref="DifficultyCalculator.CreateDifficultyAttributes"/> is sufficient enough.
    /// </summary>
    public DifficultyAttributes GetDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate);

    public DifficultyAttributes Calculate(CancellationToken cancellationToken = default);

    public DifficultyAttributes Calculate([NotNull] IEnumerable<Mod> mods, CancellationToken cancellationToken = default);
}