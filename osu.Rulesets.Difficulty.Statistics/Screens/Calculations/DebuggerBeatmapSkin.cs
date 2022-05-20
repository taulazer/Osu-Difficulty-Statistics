using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL.Textures;
using osu.Framework.Graphics.Textures;
using osu.Game.Audio;
using osu.Game.Skinning;
using osuTK.Graphics;

namespace osu.Rulesets.Difficulty.Statistics.Screens.Calculations;

public class DebuggerBeatmapSkin : ISkin
{
    public event Action BeatmapSkinChanged;

    /// <summary>
    /// The underlying beatmap skin.
    /// </summary>
    protected internal readonly Skin Skin;

    /// <summary>
    /// The combo colours of this skin.
    /// If empty, the default combo colours will be used.
    /// </summary>
    public BindableList<Colour4> ComboColours { get; }

    public DebuggerBeatmapSkin(Skin skin)
    {
        Skin = skin;

        ComboColours = new BindableList<Colour4>();
        if (Skin.Configuration.ComboColours != null)
            ComboColours.AddRange(Skin.Configuration.ComboColours.Select(c => (Colour4)c));
        ComboColours.BindCollectionChanged((_, __) => updateColours());
    }

    private void invokeSkinChanged() => BeatmapSkinChanged?.Invoke();

    private void updateColours()
    {
        Skin.Configuration.CustomComboColours = ComboColours.Select(c => (Color4)c).ToList();
        invokeSkinChanged();
    }

    #region Delegated ISkin implementation

    public Drawable GetDrawableComponent(ISkinComponent component) => Skin.GetDrawableComponent(component);
    public Texture GetTexture(string componentName, WrapMode wrapModeS, WrapMode wrapModeT) => Skin.GetTexture(componentName, wrapModeS, wrapModeT);
    public ISample GetSample(ISampleInfo sampleInfo) => Skin.GetSample(sampleInfo);
    public IBindable<TValue> GetConfig<TLookup, TValue>(TLookup lookup) => Skin.GetConfig<TLookup, TValue>(lookup);

    #endregion
}