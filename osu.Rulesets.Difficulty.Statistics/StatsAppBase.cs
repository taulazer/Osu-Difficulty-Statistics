using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game;
using osu.Rulesets.Difficulty.Statistics.Rulesets;

namespace osu.Rulesets.Difficulty.Statistics;

public class StatsAppBase : OsuGameBase
{
    [Cached]
    private Bindable<ExtendedRulesetInfo> ruleset = new();

    protected ExtendedRulesetsStore ExtendedRulesetsStore { get; private set; }
    
    private DependencyContainer dependencies;
    
    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
        dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    [BackgroundDependencyLoader]
    private void load()
    {
        dependencies.CacheAs(ExtendedRulesetsStore = new ExtendedRulesetsStore());
        
        ruleset.BindValueChanged(r =>
        {
            Ruleset.Value = r.NewValue;
        });
    }
}