using osu.Framework.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Scoring;
using osu.Rulesets.Difficulty.Statistics.Rulesets;

namespace osu.Rulesets.Difficulty.Statistics.Screens.Select.Details;

public class BeatmapPerformanceInformation : TableInformation
{
    protected override Drawable[,] CreateContent()
    {
        var rInstance = CurrentRuleset.CreateInstance();
        var difficultyCalculator = rInstance.extendedRuleset.CreateExtendedDifficultyCalculator(WorkingBeatmap);
        var difficultyAttributes = difficultyCalculator.Calculate();
        var performanceCalculator = rInstance.extendedRuleset.CreateExtendedPerformanceCalculator();
        var attributes = performanceCalculator.Calculate(new ScoreInfo(WorkingBeatmap.BeatmapInfo, CurrentRuleset)
        {
            Accuracy = 1,
            MaxCombo = difficultyAttributes.MaxCombo,
            Mods = SelectedMods.ToArray(),
            Ruleset = CurrentRuleset,
            // TODO: Force? other rulesets to calculate statistics
            // Statistics = statistics,
            // TotalScore = score,
        }, difficultyAttributes).ToReadableEnumerable().ToList();

        var content = new Drawable[attributes.Count, 2];

        for (int i = 0; i < attributes.Count; i++)
        {
            var attribute = attributes[i];

            content[i, 0] = new OsuSpriteText { Text = attribute.Key };
            content[i, 1] = new OsuSpriteText { Text = $"{attribute.Value:N2}" };
        }

        return content;
    }
}