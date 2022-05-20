using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Overlays.Toolbar;
using osu.Rulesets.Difficulty.Statistics.Rulesets;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Rulesets.Difficulty.Statistics.Graphics.Navbar;

public class ToolbarRulesetSelector : TabControl<ExtendedRulesetInfo>
{
    [Resolved]
    private ExtendedRulesetsStore ExtendedRulesetses { get; set; }

    protected Drawable ModeButtonLine { get; private set; }

    private readonly Dictionary<string, Sample> selectionSamples = new Dictionary<string, Sample>();

    public ToolbarRulesetSelector()
    {
        RelativeSizeAxes = Axes.Y;
        AutoSizeAxes = Axes.X;
    }

    protected override Dropdown<ExtendedRulesetInfo> CreateDropdown() => null!;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        AddRangeInternal(new[]
        {
            new Box()
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4Extensions.FromHex(@"1F2624"),
                Depth = 1,
            },
            ModeButtonLine = new CircularContainer()
            {
                Size = new Vector2(Toolbar.HEIGHT / 2, 3),
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.TopLeft,
                Masking = true,
                Y = -6,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                }
            }
        });
        
        foreach (var r in ExtendedRulesetses.AvailableRulesets)
            AddItem(r);

        foreach (var ruleset in ExtendedRulesetses.AvailableRulesets)
            selectionSamples[ruleset.ShortName] = audio.Samples.Get($"UI/ruleset-select-{ruleset.ShortName}");
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Current.BindDisabledChanged(_ => Scheduler.AddOnce(currentDisabledChanged));
        currentDisabledChanged();

        Current.BindValueChanged(_ => moveLineToCurrent());
        // Scheduled to allow the button flow layout to be computed before the line position is updated
        ScheduleAfterChildren(moveLineToCurrent);
    }

    private void currentDisabledChanged()
    {
        this.FadeColour(Current.Disabled ? Color4.Gray : Color4.White, 300);
    }

    private bool hasInitialPosition;

    private void moveLineToCurrent()
    {
        if (SelectedTab != null)
        {
            ModeButtonLine.MoveToX(SelectedTab.DrawPosition.X + Toolbar.HEIGHT / 4 + 4, !hasInitialPosition ? 0 : 200, Easing.OutQuint);

            if (hasInitialPosition)
                selectionSamples[SelectedTab.Value.ShortName]?.Play();

            hasInitialPosition = true;
        }
    }

    public override bool HandleNonPositionalInput => !Current.Disabled && base.HandleNonPositionalInput;

    public override bool HandlePositionalInput => !Current.Disabled && base.HandlePositionalInput;

    public override bool PropagatePositionalInputSubTree => !Current.Disabled && base.PropagatePositionalInputSubTree;

    protected override TabItem<ExtendedRulesetInfo> CreateTabItem(ExtendedRulesetInfo value) => new ToolbarRulesetTabButton(value);

    protected override TabFillFlowContainer CreateTabFlow() => new TabFillFlowContainer
    {
        RelativeSizeAxes = Axes.Y,
        AutoSizeAxes = Axes.X,
        Direction = FillDirection.Horizontal,
    };


    protected override bool OnKeyDown(KeyDownEvent e)
    {
        base.OnKeyDown(e);

        if (e.ControlPressed && !e.Repeat && e.Key >= Key.Number1 && e.Key <= Key.Number9)
        {
            int requested = e.Key - Key.Number1;

            ExtendedRulesetInfo found = ExtendedRulesetses.AvailableRulesets.ElementAtOrDefault(requested);
            if (found != null)
                Current.Value = found;
            return true;
        }

        return false;
    }
}