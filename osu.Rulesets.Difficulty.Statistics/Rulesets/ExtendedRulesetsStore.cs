using System.Reflection;
using osu.Framework;
using osu.Framework.Logging;
using osu.Game.Rulesets;

namespace osu.Rulesets.Difficulty.Statistics.Rulesets;

public class ExtendedRulesetsStore : IDisposable
{
    private const string ruleset_library_prefix = @"osu.Rulesets.Difficulty.Extended";

    protected readonly Dictionary<Assembly, Type> LoadedAssemblies = new Dictionary<Assembly, Type>();

    /// <summary>
    /// All available rulesets.
    /// </summary>
    public IEnumerable<ExtendedRulesetInfo> AvailableRulesets { get; set; }

    public List<ExtendedRulesetInfo> RulesetInfos { get; } = new();

    public ExtendedRulesetsStore()
    {
        loadFromAppDomain();
        
        if (RuntimeInfo.StartupDirectory != null)
            loadFromDisk();
        
        AppDomain.CurrentDomain.AssemblyResolve += resolveRulesetDependencyAssembly;
        
        loadRulesets();
    }

    private void loadRulesets()
    {
        var instances = LoadedAssemblies.Values.Select(r => (Ruleset)Activator.CreateInstance(r)).ToList();

        foreach (var r in instances)
        {
            RulesetInfos.Add((ExtendedRulesetInfo) r!.RulesetInfo);
        }
        
        foreach (var r in RulesetInfos)
        {
            try
            {
                var resolvedType = Type.GetType(r.InstantiationInfo)
                                   ?? throw new RulesetLoadException(@"Type could not be resolved");

                var instanceInfo = (Activator.CreateInstance(resolvedType) as Ruleset)?.RulesetInfo
                                   ?? throw new RulesetLoadException(@"Instantiation failure");

                r.Name = instanceInfo.Name;
                r.ShortName = instanceInfo.ShortName;
                r.InstantiationInfo = instanceInfo.InstantiationInfo;
                r.Available = true;
            }
            catch
            {
                r.Available = false;
            }
        }
        
        AvailableRulesets = RulesetInfos.Where(r => r.Available).ToList();
    }
    
    private void loadFromAppDomain()
    {
        foreach (var ruleset in AppDomain.CurrentDomain.GetAssemblies())
        {
            string? rulesetName = ruleset.GetName().Name;

            if (rulesetName == null)
                continue;

            if (!rulesetName.StartsWith(ruleset_library_prefix, StringComparison.InvariantCultureIgnoreCase) || rulesetName.Contains(@"Tests"))
                continue;

            addRuleset(ruleset);
        }
    }

    private Assembly? resolveRulesetDependencyAssembly(object? sender, ResolveEventArgs args)
    {
        var asm = new AssemblyName(args.Name);

        var domainAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a =>
            {
                string? name = a.GetName().Name;
                if (name == null)
                    return false;

                return args.Name.Contains(name, StringComparison.Ordinal);
            }).OrderBy(a => a.GetName().Version)
            .FirstOrDefault();

        if (domainAssembly != null)
            return domainAssembly;

        return LoadedAssemblies.Keys.FirstOrDefault(a => a.FullName == asm.FullName);
    }
    
    private void loadFromDisk()
    {
        try
        {
            string[] files = Directory.GetFiles(RuntimeInfo.StartupDirectory, $"{ruleset_library_prefix}.*.dll");

            foreach (string file in files.Where(f => !Path.GetFileName(f).Contains("Tests")))
                loadRulesetFromFile(file);
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Could not load rulesets from directory {RuntimeInfo.StartupDirectory}");
        }
    }
    
    private void loadRulesetFromFile(string file)
    {
        string filename = Path.GetFileNameWithoutExtension(file);

        if (LoadedAssemblies.Values.Any(t => Path.GetFileNameWithoutExtension(t.Assembly.Location) == filename))
            return;

        try
        {
            addRuleset(Assembly.LoadFrom(file));
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Failed to load ruleset {filename}");
        }
    }

    private void addRuleset(Assembly assembly)
    {
        if (LoadedAssemblies.ContainsKey(assembly))
            return;

        // the same assembly may be loaded twice in the same AppDomain (currently a thing in certain Rider versions https://youtrack.jetbrains.com/issue/RIDER-48799).
        // as a failsafe, also compare by FullName.
        if (LoadedAssemblies.Any(a => a.Key.FullName == assembly.FullName))
            return;

        try
        {
            LoadedAssemblies[assembly] = assembly.GetTypes().First(t => t.IsPublic && t.IsSubclassOf(typeof(Ruleset)));
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Failed to add ruleset {assembly}");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        AppDomain.CurrentDomain.AssemblyResolve -= resolveRulesetDependencyAssembly;
    }
}