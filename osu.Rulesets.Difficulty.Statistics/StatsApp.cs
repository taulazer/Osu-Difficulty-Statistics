using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Threading;
using osu.Game;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Overlays.Volume;
using osu.Game.Screens;
using osu.Rulesets.Difficulty.Statistics.Graphics.Navbar;
using osu.Rulesets.Difficulty.Statistics.Overlays;
using osu.Rulesets.Difficulty.Statistics.Screens.Select;
using osuTK.Graphics;

namespace osu.Rulesets.Difficulty.Statistics;

public class StatsApp : StatsAppBase, IOverlayManager
{
    private Container overlayContent;

    private OsuScreenStack screenStack;

    private VolumeOverlay volume;

    private Container leftFloatingOverlayContent;

    private DependencyContainer dependencies;

    private TopBar toolbar;

    protected BackButton BackButton;

    protected ScalingContainer ScreenContainer { get; private set; }

    protected Container ScreenOffsetContainer { get; private set; }

    private Container overlayOffsetContainer;
    private Container topMostOverlayContent;

    /// <summary>
    /// Whether overlays should be able to be opened game-wide. Value is sourced from the current active screen.
    /// </summary>
    public readonly IBindable<OverlayActivation> OverlayActivationMode = new Bindable<OverlayActivation>();

    private readonly List<OsuFocusedOverlayContainer> focusedOverlays = new List<OsuFocusedOverlayContainer>();
    private readonly List<OverlayContainer> visibleBlockingOverlays = new List<OverlayContainer>();
    private readonly List<OverlayContainer> externalOverlays = new List<OverlayContainer>();

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
        dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    private float toolbarOffset => (toolbar?.Position.Y ?? 0) + (toolbar?.DrawHeight ?? 0);

    [BackgroundDependencyLoader]
    private void load()
    {
        BackButton.Receptor receptor;

        AddRange(new Drawable[]
        {
            new VolumeControlReceptor
            {
                RelativeSizeAxes = Axes.Both,
                ActionRequested = action => volume.Adjust(action),
                ScrollActionRequested = (action, amount, isPrecise) => volume.Adjust(action, amount, isPrecise),
            },
            ScreenOffsetContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    ScreenContainer = new ScalingContainer(ScalingMode.ExcludeOverlays)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            receptor = new BackButton.Receptor(),
                            screenStack = new OsuScreenStack { RelativeSizeAxes = Axes.Both },
                            BackButton = new BackButton
                            {
                                Anchor = Anchor.BottomLeft,
                                Origin = Anchor.BottomLeft,
                                Action = () =>
                                {
                                    if (!(screenStack.CurrentScreen is IOsuScreen currentScreen))
                                        return;

                                    if (!((Drawable) currentScreen).IsLoaded || (currentScreen.AllowBackButton && !currentScreen.OnBackButton()))
                                        screenStack.Exit();
                                }
                            }
                        }
                    }
                }
            },
            overlayOffsetContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    overlayContent = new Container { RelativeSizeAxes = Axes.Both },
                    leftFloatingOverlayContent = new Container { RelativeSizeAxes = Axes.Both, },
                }
            },
            topMostOverlayContent = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    toolbar = new TopBar
                    {
                        Height = 48,
                        RelativeSizeAxes = Axes.X
                    }
                }
            },
        });
        
        screenStack.ScreenPushed += screenPushed;
        screenStack.ScreenExited += screenExited;

        loadComponentSingleFile(volume = new VolumeOverlay(), leftFloatingOverlayContent.Add, true);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        var select = new BeatmapSelect();
        Scheduler.AddDelayed(() => screenStack.Push(select), 1);
    }

    private Task asyncLoadStream;

    private T loadComponentSingleFile<T>(T component, Action<T> loadCompleteAction, bool cache = false)
        where T : Drawable
    {
        if (cache)
            dependencies.CacheAs(component);

        // schedule is here to ensure that all component loads are done after LoadComplete is run (and thus all dependencies are cached).
        // with some better organisation of LoadComplete to do construction and dependency caching in one step, followed by calls to loadComponentSingleFile,
        // we could avoid the need for scheduling altogether.
        Schedule(() =>
        {
            var previousLoadStream = asyncLoadStream;

            // chain with existing load stream
            asyncLoadStream = Task.Run(async () =>
            {
                if (previousLoadStream != null)
                    await previousLoadStream.ConfigureAwait(false);

                try
                {
                    Logger.Log($"Loading {component}...");

                    // Since this is running in a separate thread, it is possible for OsuGame to be disposed after LoadComponentAsync has been called
                    // throwing an exception. To avoid this, the call is scheduled on the update thread, which does not run if IsDisposed = true
                    Task task = null;
                    var del = new ScheduledDelegate(() => task = LoadComponentAsync(component, loadCompleteAction));
                    Scheduler.Add(del);

                    // The delegate won't complete if OsuGame has been disposed in the meantime
                    while (!IsDisposed && !del.Completed)
                        await Task.Delay(10).ConfigureAwait(false);

                    // Either we're disposed or the load process has started successfully
                    if (IsDisposed)
                        return;

                    Debug.Assert(task != null);

                    await task.ConfigureAwait(false);

                    Logger.Log($"Loaded {component}!");
                }
                catch (OperationCanceledException)
                {
                }
            });
        });

        return component;
    }

    #region IOverlayManager

    IBindable<OverlayActivation> IOverlayManager.OverlayActivationMode => OverlayActivationMode;

    private void updateBlockingOverlayFade() =>
        ScreenContainer.FadeColour(visibleBlockingOverlays.Any() ? OsuColour.Gray(0.5f) : Color4.White, 500, Easing.OutQuint);

    IDisposable IOverlayManager.RegisterBlockingOverlay(OverlayContainer overlayContainer)
    {
        if (overlayContainer.Parent != null)
            throw new ArgumentException($@"Overlays registered via {nameof(IOverlayManager.RegisterBlockingOverlay)} should not be added to the scene graph.");

        if (externalOverlays.Contains(overlayContainer))
            throw new ArgumentException($@"{overlayContainer} has already been registered via {nameof(IOverlayManager.RegisterBlockingOverlay)} once.");

        externalOverlays.Add(overlayContainer);
        overlayContent.Add(overlayContainer);

        if (overlayContainer is OsuFocusedOverlayContainer focusedOverlayContainer)
            focusedOverlays.Add(focusedOverlayContainer);

        return new InvokeOnDisposal(() => unregisterBlockingOverlay(overlayContainer));
    }

    void IOverlayManager.ShowBlockingOverlay(OverlayContainer overlay)
    {
        if (!visibleBlockingOverlays.Contains(overlay))
            visibleBlockingOverlays.Add(overlay);
        updateBlockingOverlayFade();
    }

    void IOverlayManager.HideBlockingOverlay(OverlayContainer overlay) => Schedule(() =>
    {
        visibleBlockingOverlays.Remove(overlay);
        updateBlockingOverlayFade();
    });

    /// <summary>
    /// Unregisters a blocking <see cref="OverlayContainer"/> that was not created by <see cref="OsuGame"/> itself.
    /// </summary>
    private void unregisterBlockingOverlay(OverlayContainer overlayContainer)
    {
        externalOverlays.Remove(overlayContainer);

        if (overlayContainer is OsuFocusedOverlayContainer focusedOverlayContainer)
            focusedOverlays.Remove(focusedOverlayContainer);

        overlayContainer.Expire();
    }

    #endregion
    
    /// <summary>
    /// Close all game-wide overlays.
    /// </summary>
    /// <param name="hideToolbar">Whether the toolbar should also be hidden.</param>
    public void CloseAllOverlays(bool hideToolbar = true)
    {
        foreach (var overlay in focusedOverlays)
            overlay.Hide();

        if (hideToolbar) toolbar.Hide();
    }

    private void screenChanged(IScreen current, IScreen newScreen)
    {
        if (current is IOsuScreen currentOsuScreen)
        {
            OverlayActivationMode.UnbindFrom(currentOsuScreen.OverlayActivationMode);
        }
        
        if (newScreen is IOsuScreen newOsuScreen)
        {
            OverlayActivationMode.BindTo(newOsuScreen.OverlayActivationMode);

            if (newOsuScreen.HideOverlaysOnEnter)
                CloseAllOverlays();
            else
                toolbar.Show();

            if (newOsuScreen.AllowBackButton)
                BackButton.Show();
            else
                BackButton.Hide();
        }
    }
    
    private void screenPushed(IScreen lastScreen, IScreen newScreen) => screenChanged(lastScreen, newScreen);

    private void screenExited(IScreen lastScreen, IScreen newScreen)
    {
        screenChanged(lastScreen, newScreen);

        if (newScreen == null)
            Exit();
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        ScreenOffsetContainer.Padding = new MarginPadding { Top = toolbarOffset };
        overlayOffsetContainer.Padding = new MarginPadding { Top = toolbarOffset };
    }
}