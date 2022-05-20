using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Screens.Select;
using osu.Rulesets.Difficulty.Statistics.Rulesets;
using osu.Rulesets.Difficulty.Statistics.Screens.Select.Details;

namespace osu.Rulesets.Difficulty.Statistics.Screens.Select;

public class DifficultyBeatmapDetailArea : BeatmapDetailArea
{
    private BeatmapDifficultyInformation diffInfo;
    private BeatmapPerformanceInformation perfInfo;
    
    [Resolved]
    private Bindable<ExtendedRulesetInfo> currentRuleset { get; set; }

    public override WorkingBeatmap Beatmap
    {
        get => base.Beatmap;
        set
        {
            base.Beatmap = value;

            diffInfo.WorkingBeatmap = value;
            perfInfo.WorkingBeatmap = value;
        }
    }

    public DifficultyBeatmapDetailArea()
    {
        Add(diffInfo = new BeatmapDifficultyInformation { RelativeSizeAxes = Axes.Both });
        Add(perfInfo = new BeatmapPerformanceInformation { RelativeSizeAxes = Axes.Both });
    }

    protected override void OnTabChanged(BeatmapDetailAreaTabItem tab, bool selectedMods)
    {
        base.OnTabChanged(tab, selectedMods);

        switch (tab)
        {
            case DifficultyBeatmapDetailAreaTabItem _:
                diffInfo.Show();
                perfInfo.Hide();
                break;

            case PerformanceBeatmapDetailAreaTabItem _:
                perfInfo.Show();
                diffInfo.Hide();
                break;
            
            default:
                diffInfo.Hide();
                perfInfo.Hide();
                break;
        }
    }

    protected override BeatmapDetailAreaTabItem[] CreateTabItems()
        => base.CreateTabItems().Concat(new BeatmapDetailAreaTabItem[]
        {
            new DifficultyBeatmapDetailAreaTabItem(),
            new PerformanceBeatmapDetailAreaTabItem()
        }).ToArray();
    
    public class DifficultyBeatmapDetailAreaTabItem : BeatmapDetailAreaTabItem
    {
        public override string Name => "Difficulty";
    }

    public class PerformanceBeatmapDetailAreaTabItem : BeatmapDetailAreaTabItem
    {
        public override string Name => "Performance";
    }
}
