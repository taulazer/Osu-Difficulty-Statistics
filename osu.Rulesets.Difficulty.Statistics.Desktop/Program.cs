using osu.Framework;
using osu.Framework.Platform;

namespace osu.Rulesets.Difficulty.Statistics.Desktop;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        using DesktopGameHost host = Host.GetSuitableDesktopHost("RulesetDifficultyStatistics");
        host.Run(new DesktopStatsApp());
    }
}