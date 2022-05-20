using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Rulesets.Difficulty.Statistics.Rulesets;
using osuTK;

namespace osu.Rulesets.Difficulty.Statistics.Graphics.Navbar;

public class TopBar : Container
{
    private ToolbarRulesetSelector rulesetSelector;

    [Resolved]
    private Bindable<ExtendedRulesetInfo> ruleset { get; set; }
    
    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4Extensions.FromHex(@"222A28")
            },
            new FillFlowContainer
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Direction = FillDirection.Horizontal,
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X,
                Spacing = new Vector2(5),
                Children = new Drawable[]
                {
                    rulesetSelector = new ToolbarRulesetSelector(),
                }
            },
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        
        rulesetSelector.Current.BindTo(ruleset);
    }
}