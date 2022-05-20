using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osu.Game.Overlays.Mods;
using osu.Game.Rulesets.Mods;
using osu.Game.Screens.Select;
using osu.Rulesets.Difficulty.Statistics.Overlays;
using osu.Rulesets.Difficulty.Statistics.Screens.Calculations;

namespace osu.Rulesets.Difficulty.Statistics.Screens.Select;

public class BeatmapSelect : SongSelect
{
    public override bool AllowBackButton => false;

    protected override BeatmapDetailArea CreateBeatmapDetailArea()
        => new DifficultyBeatmapDetailArea();

    protected override IEnumerable<(FooterButton, OverlayContainer)> CreateFooterButtons()
        => new (FooterButton, OverlayContainer)[] { };

    protected override bool OnStart()
    {
        this.Push(new CalculationsScreen());
        
        return true;
    }

    #region Mod overlay
    
    [CanBeNull]
    private IDisposable modSelectOverlayRegistration;

    [Resolved(CanBeNull = true)]
    internal IOverlayManager OverlayManager { get; private set; }
    
    [Resolved]
    private Bindable<IReadOnlyList<Mod>> selectedMods { get; set; }
    
    private ModSelectOverlay modSelect { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        // preload the mod select overlay for later use in `LoadComplete()`.
        // therein it will be registered at the `OsuGame` level to properly function as a blocking overlay.
        LoadComponent(modSelect = CreateModSelectOverlay());

        if (Footer != null)
        {
            Footer.AddButton(new FooterButtonMods() { Current = Mods }, modSelect);
            Footer.AddButton(new FooterButtonRandom
            {
                NextRandom = () => Carousel.SelectNextRandom(),
                PreviousRandom = Carousel.SelectPreviousRandom
            }, null);
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        modSelectOverlayRegistration = OverlayManager?.RegisterBlockingOverlay(modSelect);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);
        
        modSelect.SelectedMods.BindTo(selectedMods);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        base.OnResuming(e);
        
        // required due to https://github.com/ppy/osu-framework/issues/3218
        modSelect.SelectedMods.Disabled = false;
        modSelect.SelectedMods.BindTo(selectedMods);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        base.OnSuspending(e);
        
        modSelect.SelectedMods.UnbindFrom(selectedMods);
        modSelect.Hide();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        modSelect.Hide();

        return base.OnExiting(e);
    }

    public override bool OnBackButton()
    {
        if (modSelect.State.Value == Visibility.Visible)
        {
            modSelect.Hide();
            return true;
        }
        
        return base.OnBackButton();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        
        modSelectOverlayRegistration?.Dispose();
    }

    #endregion
}
