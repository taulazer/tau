using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects.Drawables.Pieces;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Screens.Edit;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Edit;

/// <summary>
/// A grid which displays coloured beat divisor lines in proximity to the selection or placement cursor.
/// </summary>
public class TauBeatSnapGrid : Component
{
    private const double visible_range = 750;

    /// <summary>
    /// The range of time values of the current selection.
    /// </summary>
    public (double start, double end)? SelectionTimeRange
    {
        set
        {
            if (value == selectionTimeRange)
                return;

            selectionTimeRange = value;
            lineCache.Invalidate();
        }
    }

    [Resolved]
    private EditorBeatmap beatmap { get; set; }

    [Resolved]
    private OsuColour colours { get; set; }

    [Resolved]
    private BindableBeatDivisor beatDivisor { get; set; }

    private readonly List<HitObjectContainer> grids = new List<HitObjectContainer>();

    private readonly Cached lineCache = new Cached();

    private (double start, double end)? selectionTimeRange;

    [BackgroundDependencyLoader]
    private void load(HitObjectComposer composer)
    {
        var lineContainer = new HitObjectContainer();
        grids.Add(lineContainer);
        beatDivisor.BindValueChanged(_ => createLines(), true);
    }

    private readonly Stack<DrawableGridLine> availableLines = new Stack<DrawableGridLine>();

    private void createLines()
    {
        foreach (var grid in grids)
        {
            foreach (var line in grid.Objects.OfType<DrawableGridLine>())
                availableLines.Push(line);

            grid.Clear();
        }

        if (selectionTimeRange == null)
            return;

        var range = selectionTimeRange.Value;

        var timingPoint = beatmap.ControlPointInfo.TimingPointAt(range.start - visible_range);

        double time = timingPoint.Time;
        int beat = 0;

        // progress time until in the visible range.
        while (time < range.start - visible_range)
        {
            time += timingPoint.BeatLength / beatDivisor.Value;
            beat++;
        }

        while (time < range.end + visible_range)
        {
            var nextTimingPoint = beatmap.ControlPointInfo.TimingPointAt(time);

            // switch to the next timing point if we have reached it.
            if (nextTimingPoint.Time > timingPoint.Time)
            {
                beat = 0;
                time = nextTimingPoint.Time;
                timingPoint = nextTimingPoint;
            }

            Color4 colour = BindableBeatDivisor.GetColourFor(
                BindableBeatDivisor.GetDivisorForBeatIndex(Math.Max(1, beat), beatDivisor.Value), colours);

            foreach (var grid in grids)
            {
                if (!availableLines.TryPop(out var line))
                    line = new DrawableGridLine(colour);

                line.HitObject.StartTime = time;

                grid.Add(line);
            }

            beat++;
            time += timingPoint.BeatLength / beatDivisor.Value;
        }

        foreach (var grid in grids)
        {
            // required to update ScrollingHitObjectContainer's cache.
            grid.UpdateSubTree();

            foreach (var line in grid.Objects.OfType<DrawableGridLine>())
            {
                time = line.HitObject.StartTime;

                if (time >= range.start && time <= range.end)
                    line.Alpha = 1;
                else
                {
                    double timeSeparation = time < range.start ? range.start - time : time - range.end;
                    line.Alpha = (float)Math.Max(0, 1 - timeSeparation / visible_range);
                }
            }
        }
    }

    private class DrawableGridLine : DrawableHitObject
    {
        public DrawableGridLine(Color4 colour)
            : base(new HitObject())
        {
            RelativeSizeAxes = Axes.X;
            Height = 2;

            AddInternal(new HardBeatPiece() {Colour = colour});
        }

        [BackgroundDependencyLoader]
        private void load()
        {

        }

        protected override void UpdateInitialTransforms()
        {
            // don't perform any fading – we are handling that ourselves.
            LifetimeEnd = HitObject.StartTime + visible_range;
        }
    }
}
