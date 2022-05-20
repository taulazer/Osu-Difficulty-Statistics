using osu.Framework.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Rulesets.Difficulty.Statistics.Rulesets;

namespace osu.Rulesets.Difficulty.Statistics.Screens.Select.Details;

public class BeatmapDifficultyInformation : TableInformation
{
    protected override Drawable[,] CreateContent()
    {
        var rInstance = CurrentRuleset.CreateInstance();
        var calculator = rInstance.extendedRuleset.CreateExtendedDifficultyCalculator(WorkingBeatmap);
        var attributes = calculator.Calculate(SelectedMods).ToReadableEnumerable().ToList();

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