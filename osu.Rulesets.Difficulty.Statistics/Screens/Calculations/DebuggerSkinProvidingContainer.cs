using osu.Game.Skinning;

namespace osu.Rulesets.Difficulty.Statistics.Screens.Calculations;

public class DebuggerSkinProvidingContainer : RulesetSkinProvidingContainer
{
    private readonly DebuggerBeatmapSkin? beatmapSkin;

    public DebuggerSkinProvidingContainer(DebuggerBeatmap debuggerBeatmap)
        : base(debuggerBeatmap.Ruleset.CreateInstance().ruleset, debuggerBeatmap.PlayableBeatmap, debuggerBeatmap.BeatmapSkin?.Skin)
    {
        beatmapSkin = debuggerBeatmap.BeatmapSkin;
    }
    
    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (beatmapSkin != null)
            beatmapSkin.BeatmapSkinChanged += TriggerSourceChanged;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (beatmapSkin != null)
            beatmapSkin.BeatmapSkinChanged -= TriggerSourceChanged;
    }
}