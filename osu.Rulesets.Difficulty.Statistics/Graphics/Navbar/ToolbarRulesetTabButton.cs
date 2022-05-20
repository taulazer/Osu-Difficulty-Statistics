using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Overlays.Toolbar;
using osu.Rulesets.Difficulty.Statistics.Rulesets;
using osuTK;
using osuTK.Graphics;

namespace osu.Rulesets.Difficulty.Statistics.Graphics.Navbar;

public class ToolbarRulesetTabButton : TabItem<ExtendedRulesetInfo>
{
    private readonly RulesetButton ruleset;

    public ToolbarRulesetTabButton(ExtendedRulesetInfo value)
        : base(value)
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Child = ruleset = new RulesetButton
        {
            Width = 48,
            Active = false,
        };

        var rInstance = value.CreateInstance().ruleset;

        ruleset.TooltipMain = rInstance.Description;
        ruleset.TooltipSub = $"play some {rInstance.Description}";
        ruleset.SetIcon(rInstance.CreateIcon());
    }

    protected override void OnActivated() => ruleset.Active = true;

    protected override void OnDeactivated() => ruleset.Active = false;

    private class RulesetButton : ToolbarButton
    {
        public bool Active
        {
            set => IconContainer.Colour = value ? Color4Extensions.FromHex(@"00FFAA") : Color4.White;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            IconContainer.Size = new Vector2(24);
        }

        protected override bool OnClick(ClickEvent e)
        {
            Parent.TriggerClick();
            return base.OnClick(e);
        }
    }
}