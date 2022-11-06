using osu.Framework;
using osu.Framework.Platform;

namespace osu.Rulesets.Difficulty.Statistics.Tests;

public static class Program
{
    public static void Main(string[] args)
    {
        using DesktopGameHost host = Host.GetSuitableDesktopHost(@"RulesetDifficultyStatistics");
        host.Run(new StatsTestBrowser());
    }
}
