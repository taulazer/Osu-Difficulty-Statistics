using osu.Game.Rulesets;

namespace osu.Rulesets.Difficulty.Statistics.Rulesets;

public class ExtendedRulesetInfo : IRulesetInfo
{
    public int OnlineID => -1;
    
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string InstantiationInfo { get; set; } = string.Empty;

    public ExtendedRulesetInfo(string shortName, string name, string instantiationInfo)
    {
        ShortName = shortName;
        Name = name;
        InstantiationInfo = instantiationInfo;
    }
    
    public bool Available { get; set; }

    Ruleset IRulesetInfo.CreateInstance() => CreateInstance().ruleset;

    public (Ruleset ruleset, IExtendedRuleset extendedRuleset) CreateInstance()
    {
        if (!Available)
            throw new RulesetLoadException(@"Ruleset not available");

        var type = Type.GetType(InstantiationInfo);

        if (type == null)
            throw new RulesetLoadException(@"Type lookup failure");

        var rulesetType = Activator.CreateInstance(type);
        var ruleset = rulesetType as Ruleset;
        var extendedRuleset = (IExtendedRuleset)rulesetType!;

        if (ruleset == null || extendedRuleset == null)
            throw new RulesetLoadException(@"Instantiation failure");

        // overwrite the pre-populated RulesetInfo with a potentially database attached copy.
        // TODO: figure if we still want/need this after switching to realm.
        // ruleset.RulesetInfo = this;

        return (ruleset, extendedRuleset);
    }

    public bool Equals(RulesetInfo? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        return ShortName == other.ShortName;
    }

    public bool Equals(IRulesetInfo? other) => other is RulesetInfo r && Equals(r);

    public int CompareTo(RulesetInfo other)
    {
        if (OnlineID >= 0 && other.OnlineID >= 0)
            return OnlineID.CompareTo(other.OnlineID);

        // Official rulesets are always given precedence for the time being.
        if (OnlineID >= 0)
            return -1;
        if (other.OnlineID >= 0)
            return 1;

        return string.Compare(ShortName, other.ShortName, StringComparison.Ordinal);
    }

    public int CompareTo(IRulesetInfo other)
    {
        if (!(other is RulesetInfo ruleset))
            throw new ArgumentException($@"Object is not of type {nameof(RulesetInfo)}.", nameof(other));

        return CompareTo(ruleset);
    }

    public override int GetHashCode()
    {
        // Importantly, ignore the underlying realm hash code, as it will usually not match.
        var hashCode = new HashCode();
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        hashCode.Add(ShortName);
        return hashCode.ToHashCode();
    }

    public static explicit operator ExtendedRulesetInfo(RulesetInfo info)
        => new(info.ShortName, info.Name, info.InstantiationInfo);
    
    public static implicit operator RulesetInfo(ExtendedRulesetInfo info)
        => new(info.ShortName, info.Name, info.InstantiationInfo, -1)
        {
            Available = info.Available
        };

    public override string ToString() => Name;
}