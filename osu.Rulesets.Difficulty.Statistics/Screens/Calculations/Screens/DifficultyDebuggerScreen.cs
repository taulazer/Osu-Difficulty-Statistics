using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Platform;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osu.Game.Screens;
using osu.Game.Screens.Edit;
using osu.Rulesets.Difficulty.Statistics.Rulesets;

namespace osu.Rulesets.Difficulty.Statistics.Screens.Calculations.Screens;

public class DifficultyDebuggerScreen : Container
{
    protected DebuggerBeatmap DebuggerBeatmap { get; private set; }
    
    [Resolved]
    private GameHost host { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }
    
    private DrawableRuleset playfield;
    
    private (Ruleset ruleset, IExtendedRuleset extendedRuleset) ruleset;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            CreateMainContent()
        });
    }
    
    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        DebuggerBeatmap = parent.Get<DebuggerBeatmap>();
        ruleset = parent.Get<Bindable<ExtendedRulesetInfo>>().Value.CreateInstance();
        playfield = ruleset.extendedRuleset?.CreateDebuggerRuleset(DebuggerBeatmap.PlayableBeatmap, new[] { ruleset.ruleset.GetAutoplayMod() }).With(d =>
        {
            d.Playfield.DisplayJudgements.Value = false;
            d.OnLoadComplete += _ =>
            {
                Scheduler.AddOnce(() => regenerateAutoplay(d));
            };
        })!;

        // make the composer available to the timeline and other components in this screen.
        if (playfield != null)
            dependencies.CacheAs(playfield);

        return dependencies;
    }

    private void regenerateAutoplay(DrawableRuleset drawableRuleset)
    {
        var autoplayMod = drawableRuleset.Mods.OfType<ModAutoplay>().Single();
        drawableRuleset.SetReplayScore(autoplayMod.CreateScoreFromReplayData(DebuggerBeatmap.PlayableBeatmap, drawableRuleset.Mods));
    }
    
    protected Drawable CreateMainContent()
    {
        if (ruleset.ruleset == null || playfield == null)
            return new ScreenWhiteBox.UnderConstructionMessage(ruleset.ruleset == null ? "This beatmap" : $"{ruleset.ruleset.Description}'s composer");

        return wrapSkinnableContent(playfield.With(c =>
        {
            c.ProcessCustomClock = false;
            c.Clock = clock;
        }));
    }
    
    private Drawable wrapSkinnableContent(Drawable content)
    {
        Debug.Assert(ruleset.ruleset != null);

        return new DebuggerSkinProvidingContainer(DebuggerBeatmap).WithChild(content);
    }
}