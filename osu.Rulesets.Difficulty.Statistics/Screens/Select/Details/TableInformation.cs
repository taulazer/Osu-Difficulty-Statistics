using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Rulesets.Difficulty.Statistics.Rulesets;
using osuTK.Graphics;

namespace osu.Rulesets.Difficulty.Statistics.Screens.Select.Details;

public abstract class TableInformation : Container
{
    [Resolved]
    private Bindable<IReadOnlyList<Mod>> selectedMods { get; set; }

    [Resolved]
    private Bindable<ExtendedRulesetInfo> currentRuleset { get; set; }

    protected ExtendedRulesetInfo CurrentRuleset => currentRuleset.Value;
    protected IReadOnlyList<Mod> SelectedMods => selectedMods.Value;

    private WorkingBeatmap workingBeatmap;

    public WorkingBeatmap WorkingBeatmap
    {
        get => workingBeatmap;
        set
        {
            if (value == workingBeatmap) return;

            workingBeatmap = value;

            ClearTable();
            SetContent(CreateContent());
        }
    }

    private TableContainer tableContainer;

    public TableInformation()
    {
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.Black,
                Alpha = 0.5f
            },
            tableContainer = new TableContainer
            {
                RelativeSizeAxes = Axes.Both,
                Margin = new MarginPadding(5),
            }
        };

        tableContainer.Columns = new TableColumn[]
        {
            new("Name"),
            new("Value"),
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        currentRuleset.BindValueChanged(_ =>
        {
            ClearTable();
            SetContent(CreateContent());
        });

        selectedMods.BindValueChanged(_ =>
        {
            ClearTable();
            SetContent(CreateContent());
        });
    }

    protected abstract Drawable[,] CreateContent();

    protected void ClearTable()
    {
        tableContainer.Content = new Drawable[,] { };
    }

    protected void SetContent(Drawable[,] content)
    {
        tableContainer.Content = content;
    }
}