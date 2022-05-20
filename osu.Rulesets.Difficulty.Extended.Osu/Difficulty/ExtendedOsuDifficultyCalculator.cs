using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Rulesets.Difficulty.Statistics.Rulesets.Difficulty;

namespace osu.Rulesets.Difficulty.Extended.Osu.Difficulty;

public class ExtendedOsuDifficultyCalculator : OsuDifficultyCalculator, IExtendedDifficultyCalculator
{
    public ExtendedOsuDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap)
        : base(ruleset, beatmap)
    {
    }

    public Skill[] GetSkills(IBeatmap beatmap, Mod[] mods, double clockRate)
        => CreateSkills(beatmap, mods, clockRate);

    public DifficultyAttributes GetDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate)
        => CreateDifficultyAttributes(beatmap, mods, skills, clockRate);
}