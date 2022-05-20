using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Edit;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Play;
using osu.Rulesets.Difficulty.Statistics.Screens.Calculations.Screens;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Rulesets.Difficulty.Statistics.Screens.Calculations;

[Cached(typeof(IBeatSnapProvider))]
[Cached]
public class CalculationsScreen : ScreenWithBeatmapBackground, IBeatSnapProvider, ISamplePlaybackDisabler
{
    public override float BackgroundParallaxAmount => 0.5f;
    
    public IBindable<bool> SamplePlaybackDisabled => samplePlaybackDisabled;

    private readonly Bindable<bool> samplePlaybackDisabled = new Bindable<bool>();
    
    private readonly BindableBeatDivisor beatDivisor = new BindableBeatDivisor();
    private EditorClock clock;

    private IBeatmap playableBeatmap;
    private EditorBeatmap editorBeatmap;
    
    private DependencyContainer dependencies;
    
    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
    
    [BackgroundDependencyLoader]
    private void load()
    {
        var loadableBeatmap = Beatmap.Value;

        try
        {
            playableBeatmap = loadableBeatmap.GetPlayableBeatmap(Ruleset.Value);

            // clone these locally for now to avoid incurring overhead on GetPlayableBeatmap usages.
            // eventually we will want to improve how/where this is done as there are issues with *not* cloning it in all cases.
            playableBeatmap.ControlPointInfo = playableBeatmap.ControlPointInfo.DeepClone();
        }
        catch (Exception e)
        {
            Logger.Error(e, "Could not load beatmap successfully!");
            // couldn't load, hard abort!
            this.Exit();
            return;
        }
        
        clock = new EditorClock(playableBeatmap, beatDivisor) { IsCoupled = false };
        clock.ChangeSource(loadableBeatmap.Track);
        
        dependencies.CacheAs(clock);
        AddInternal(clock);

        clock.SeekingOrStopped.BindValueChanged(_ => updateSampleDisabledState());

        // todo: remove caching of this and consume via editorBeatmap?
        dependencies.Cache(beatDivisor);
        
        AddInternal(editorBeatmap = new EditorBeatmap(playableBeatmap, loadableBeatmap.Skin, loadableBeatmap.BeatmapInfo));
        dependencies.CacheAs(editorBeatmap);
        
        beatDivisor.Value = editorBeatmap.BeatmapInfo.BeatDivisor;
        beatDivisor.BindValueChanged(divisor => editorBeatmap.BeatmapInfo.BeatDivisor = divisor.NewValue);
        
        Schedule(() =>
        {
            // we need to avoid changing the beatmap from an asynchronous load thread. it can potentially cause weirdness including crashes.
            // this assumes that nothing during the rest of this load() method is accessing Beatmap.Value (loadableBeatmap should be preferred).
            // generally this is quite safe, as the actual load of editor content comes after menuBar.Mode.ValueChanged is fired in its own LoadComplete.
            Beatmap.Value = loadableBeatmap;
        });
        
        AddRangeInternal(new Drawable[]
        {
            new DifficultyDebuggerScreen
            {
                RelativeSizeAxes = Axes.Both
            }
        });
    }
    
    protected override void Update()
    {
        base.Update();
        clock.ProcessFrame();
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);
        dimBackground();
        resetTrack(true);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        base.OnResuming(e);
        dimBackground();
    }
    
    protected override bool OnKeyDown(KeyDownEvent e)
    {
        switch (e.Key)
        {
            case Key.Left:
                seek(e, -1);
                return true;

            case Key.Right:
                seek(e, 1);
                return true;
        }

        return true;
    }

    private double scrollAccumulation;

    protected override bool OnScroll(ScrollEvent e)
    {
        if (e.ControlPressed || e.AltPressed || e.SuperPressed)
            return false;

        const double precision = 1;

        double scrollComponent = e.ScrollDelta.X + e.ScrollDelta.Y;

        double scrollDirection = Math.Sign(scrollComponent);

        // this is a special case to handle the "pivot" scenario.
        // if we are precise scrolling in one direction then change our mind and scroll backwards,
        // the existing accumulation should be applied in the inverse direction to maintain responsiveness.
        if (scrollAccumulation != 0 && Math.Sign(scrollAccumulation) != scrollDirection)
            scrollAccumulation = scrollDirection * (precision - Math.Abs(scrollAccumulation));

        scrollAccumulation += scrollComponent * (e.IsPrecise ? 0.1 : 1);

        // because we are doing snapped seeking, we need to add up precise scrolls until they accumulate to an arbitrary cut-off.
        while (Math.Abs(scrollAccumulation) >= precision)
        {
            if (scrollAccumulation > 0)
                seek(e, -1);
            else
                seek(e, 1);

            scrollAccumulation = scrollAccumulation < 0 ? Math.Min(0, scrollAccumulation + precision) : Math.Max(0, scrollAccumulation - precision);
        }

        return true;
    }
    
    private void dimBackground()
    {
        ApplyToBackground(b =>
        {
            // todo: temporary. we want to be applying dim using the UserDimContainer eventually.
            b.FadeColour(Color4.DarkGray, 500);

            b.IgnoreUserSettings.Value = true;
            b.BlurAmount.Value = 25;
        });
    }
    
    private void resetTrack(bool seekToStart = false)
    {
        Beatmap.Value.Track.Stop();

        if (seekToStart)
        {
            double targetTime = 0;

            if (Beatmap.Value.Beatmap.HitObjects.Count > 0)
            {
                // seek to one beat length before the first hitobject
                targetTime = Beatmap.Value.Beatmap.HitObjects[0].StartTime;
                targetTime -= Beatmap.Value.Beatmap.ControlPointInfo.TimingPointAt(targetTime).BeatLength;
            }

            clock.Seek(Math.Max(0, targetTime));
        }
    }
    
    private void updateSampleDisabledState()
    {
        samplePlaybackDisabled.Value = clock.SeekingOrStopped.Value;
    }

    private void seek(UIEvent e, int direction)
    {
        double amount = e.ShiftPressed ? 4 : 1;

        bool trackPlaying = clock.IsRunning;

        if (trackPlaying)
        {
            // generally users are not looking to perform tiny seeks when the track is playing,
            // so seeks should always be by one full beat, bypassing the beatDivisor.
            // this multiplication undoes the division that will be applied in the underlying seek operation.
            amount *= beatDivisor.Value;
        }

        if (direction < 1)
            clock.SeekBackward(!trackPlaying, amount);
        else
            clock.SeekForward(!trackPlaying, amount);
    }
    
    public double SnapTime(double time, double? referenceTime) => editorBeatmap.SnapTime(time, referenceTime);

    public double GetBeatLengthAtTime(double referenceTime) => editorBeatmap.GetBeatLengthAtTime(referenceTime);

    public int BeatDivisor => beatDivisor.Value;
}