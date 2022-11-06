using osu.Framework.Allocation;
using osu.Framework.Platform;

namespace osu.Rulesets.Difficulty.Statistics.Tests.Visual;

public class TestSceneStatsApp : StatsAppTestScene
{
    private StatsApp app = null!;

    [BackgroundDependencyLoader]
    private void load(GameHost host)
    {
        app = new StatsApp();
        app.SetHost(host);
        
        AddGame(app);
    }
}
