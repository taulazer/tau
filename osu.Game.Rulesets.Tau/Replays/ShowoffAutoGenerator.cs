﻿using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Tau.Replays;

public class ShowoffAutoGenerator : AutoGenerator
{
    private Beatmap<TauHitObject> beatmap => (Beatmap<TauHitObject>)Beatmap;
    private readonly Vector2 centre = TauPlayfield.BASE_SIZE / 2;
    private const float cursor_distance = 250;

    private readonly float paddleHalfSize;
    private readonly int paddleCount = 1;
    private readonly RotationDirection? rotationDirection;

    public ShowoffAutoGenerator(IBeatmap beatmap, IReadOnlyList<Mod> mods)
        : base(beatmap)
    {
        var props = new TauCachedProperties();
        props.SetRange(beatmap.Difficulty.CircleSize);
        paddleHalfSize = (float)(props.AngleRange.Value / 2) * 0.65f; // it doesnt look good if we catch with the literal edge

        if (mods.GetMod(out TauModDual dual))
            paddleCount = dual.PaddleCount.Value;

        if (mods.GetMod(out TauModRoundabout round))
            rotationDirection = round.Direction.Value;
    }

    public override Replay Generate()
    {
        var frames = createMovementFrames();
        applyInputFrames(frames);

        var replay = new Replay();
        replay.Frames.AddRange(frames);
        return replay;
    }

    private LinkedList<TauReplayFrame> createMovementFrames()
    {
        double lastTime = double.NegativeInfinity;
        float lastAngle = 0;
        int paddleIndex;
        Vector2 lastPosition = centre + new Vector2(0, -cursor_distance);
        LinkedList<TauReplayFrame> frames = new();

        foreach (var i in beatmap.HitObjects)
        {
            switch (i)
            {
                case Beat beat:
                    waitUntil(beat.StartTime - beat.TimePreempt);
                    choosePaddleFor(beat.Angle);
                    moveTo(beat.Angle, beat.StartTime, lazy: isLazy(beat.Angle));
                    break;

                case Slider slider:
                    waitUntil(slider.StartTime - slider.TimePreempt);
                    choosePaddleFor(slider.Angle);
                    moveTo(slider.Angle, slider.StartTime, lazy: isLazy(slider.Angle));

                    foreach (var node in slider.Path.Nodes)
                    {
                        moveTo(slider.Angle + node.Angle, slider.StartTime + node.Time, smooth: true, lazy: true);
                    }

                    break;
            }
        }

        float getRotationLockedDelta(float to, float from)
        {
            float delta = Extensions.GetDeltaAngle(to, from);

            if (rotationDirection is RotationDirection.Clockwise)
            {
                if (delta < 0)
                    delta += 360;
            }
            else if (rotationDirection is RotationDirection.Counterclockwise)
            {
                if (delta > 0)
                    delta -= 360;
            }

            return delta;
        }

        void choosePaddleFor(float angle)
        {
            paddleIndex = Enumerable.Range(0, paddleCount)
                                    .MinBy(x =>
                                         (MathF.Abs(Extensions.GetDeltaAngle(angle, lastAngle + x * 360f / paddleCount)) <= paddleHalfSize ? 0 : 1) *
                                         MathF.Abs(getRotationLockedDelta(angle, lastAngle + x * 360f / paddleCount))
                                     );
        }

        float getRawDeltaAngle(float to)
            => Extensions.GetDeltaAngle(to, lastAngle + paddleIndex * 360f / paddleCount);

        float getDeltaAngle(float to)
            => getRotationLockedDelta(to, lastAngle + paddleIndex * 360f / paddleCount);

        bool isLazy(float angle)
            => MathF.Abs(getDeltaAngle(angle)) < 45;

        void addFrame(double time, Vector2 position)
        {
            frames.AddLast(new TauReplayFrame(time, position));
            lastPosition = position;
            lastAngle = MathF.Atan2(position.Y - centre.Y, position.X - centre.X) / MathF.PI * 180 + 90;
            lastTime = time;
        }

        void addAngleFrame(double time, float angle, float distance)
        {
            angle = (angle - 90) * MathF.PI / 180;
            addFrame(time, centre + new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * distance);
        }

        void waitUntil(double time)
        {
            if (lastTime < time)
            {
                addFrame(time, lastPosition);
            }
        }

        void moveTo(float angle, double time, bool smooth = false, bool lazy = false)
        {
            double duration = time - lastTime;
            float rawDelta = getRawDeltaAngle(angle);
            float delta = getDeltaAngle(angle);

            if (MathF.Abs(rawDelta) <= paddleHalfSize)
            {
                addAngleFrame(time, lastAngle, cursor_distance);
                return;
            }

            if (lazy && MathF.Abs(delta) < 180 /* roundabout doesnt let you catch in the opposite direction */)
            {
                angle = lastAngle + delta - MathF.Sign(delta) * paddleHalfSize;
            }

            if (duration == 0 || (!smooth && rotationDirection is null))
            {
                addAngleFrame(time, lastAngle + delta, cursor_distance);
            }
            else
            {
                int steps = (int)(MathF.Abs(delta) / (smooth ? 5 : 45));
                double startTime = lastTime;
                float startAngle = lastAngle;

                for (int i = 0; i <= steps; i++)
                {
                    addAngleFrame(startTime + duration / (steps + 1) * (i + 1), startAngle + delta / (steps + 1) * (i + 1), cursor_distance);
                }
            }
        }

        return frames;
    }

    private void applyInputFrames(LinkedList<TauReplayFrame> frames)
    {
        int nextIndex = 0;
        int nextHardIndex = 0;
        double currentTime;
        // currentFrame will never be null on non-empty maps and no input can ever be before the first frame
        // ... ------ (currentFrame.Time) [------ (currentTime) ------) (currentFrame.Next?.Time) ------>
        LinkedListNode<TauReplayFrame> currentFrame = frames.First;
        List<(double time, TauAction action)> taps = new();

        foreach (var i in beatmap.HitObjects)
        {
            switch (i)
            {
                case Beat beat:
                    waitUntil(beat.StartTime);
                    tap();
                    break;

                case Slider slider:
                    waitUntil(slider.StartTime);
                    var action = down();
                    waitUntil(slider.EndTime);
                    up(action);
                    break;

                case HardBeat hardBeat:
                    waitUntil(hardBeat.StartTime);
                    tap(hard: true);
                    break;
            }
        }

        if (taps.Any())
            waitUntil(taps.Last().time + KEY_UP_DELAY);

        // ... ------ (currentFrame.Time) [------ (currentTime) ------) (currentFrame.Next?.Time) ------>
        void addFrame(params TauAction[] actions)
        {
            if (currentFrame.Next != null)
            {
                float t = (float)((currentTime - currentFrame.Value.Time) / (currentFrame.Next.Value.Time - currentFrame.Value.Time));
                currentFrame = frames.AddAfter(currentFrame, new TauReplayFrame(
                    currentTime,
                    currentFrame.Value.Position * (1 - t) + currentFrame.Next.Value.Position * t,
                    actions
                ));
            }
            else
            {
                currentFrame = frames.AddAfter(currentFrame, new TauReplayFrame(
                    currentTime,
                    currentFrame.Value.Position,
                    actions
                ));
            }
        }

        void waitUntil(double time)
        {
            void seek(double seekTo)
            {
                while (currentFrame?.Next != null && currentFrame.Next.Value.Time <= seekTo)
                {
                    currentFrame.Next.Value.Actions.Clear();
                    currentFrame.Next.Value.Actions.AddRange(currentFrame.Value.Actions);
                    currentFrame = currentFrame.Next;
                }

                currentTime = seekTo;
            }

            while (taps.Any() && taps[0].time + KEY_UP_DELAY <= time)
            {
                var tapAction = taps[0];
                taps.RemoveAt(0);
                seek(tapAction.time + KEY_UP_DELAY);
                up(tapAction.action);
            }

            seek(time);
        }

        void tap(bool hard = false)
        {
            var action = down(hard);
            taps.Add((currentTime, action));
        }

        TauAction down(bool hard = false)
        {
            var action = hard
                             ? (nextHardIndex++ % 2 == 0 ? TauAction.HardButton1 : TauAction.HardButton2)
                             : (nextIndex++ % 2 == 0 ? TauAction.LeftButton : TauAction.RightButton);

            if (taps.Any(x => x.action == action))
            {
                taps.Remove(taps.First(x => x.action == action));
                up(action);
            }

            addFrame(currentFrame.Value.Actions.Append(action).ToArray());
            return action;
        }

        void up(TauAction action)
            => addFrame(currentFrame.Value.Actions.Except(action.Yield()).ToArray());
    }
}
