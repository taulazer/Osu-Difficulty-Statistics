using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Game.Screens.Play;
using osuTK.Graphics;

namespace osu.Rulesets.Difficulty.Statistics.Screens.Calculations;

public class CalculationsScreen : ScreenWithBeatmapBackground
{
    public override float BackgroundParallaxAmount => 0.5f;

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);
        dimBackground();
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        base.OnResuming(e);
        dimBackground();
    }
    
    private void dimBackground()
    {
        ApplyToBackground(b =>
        {
            // todo: temporary. we want to be applying dim using the UserDimContainer eventually.
            b.FadeColour(Color4.DarkGray, 500);

            b.IgnoreUserSettings.Value = true;
            b.BlurAmount.Value = 25;
        });
    }
}