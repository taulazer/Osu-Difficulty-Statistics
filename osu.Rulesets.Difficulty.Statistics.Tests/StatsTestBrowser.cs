using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Framework.Testing;
using osu.Game.Graphics;
using osu.Game.Screens.Backgrounds;

namespace osu.Rulesets.Difficulty.Statistics.Tests;

public class StatsTestBrowser : StatsAppBase
{
    protected override void LoadComplete()
    {
        base.LoadComplete();

        LoadComponentAsync(new ScreenStack(new BackgroundScreenDefault { Colour = OsuColour.Gray(0.5f) })
        {
            Depth = 10,
            RelativeSizeAxes = Axes.Both
        }, AddInternal);
        
        Add(new TestBrowser());
    }

    public override void SetHost(GameHost host)
    {
        base.SetHost(host);
        host.Window.CursorState |= CursorState.Hidden;
    }
}
