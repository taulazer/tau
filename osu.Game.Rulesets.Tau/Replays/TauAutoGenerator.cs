using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;

namespace osu.Game.Rulesets.Tau.Replays
{
    public class TauAutoGenerator : TauAutoGeneratorBase
    {
        public new Beatmap<TauHitObject> Beatmap => (Beatmap<TauHitObject>)base.Beatmap;

        #region Constants

        /// <summary>
        /// What easing to use when moving between hitobjects
        /// </summary>
        private Easing preferredEasing => Easing.Out;

        #endregion

        #region Construction / Initialization

        public TauAutoGenerator(IBeatmap beatmap, IReadOnlyList<Mod> mods)
            : base(beatmap, mods)
        {
        }

        #endregion

        #region Generator

        private int buttonIndex1;

        private int buttonIndex2;

        public override Replay Generate()
        {
            if (Beatmap.HitObjects.Count == 0)
                return Replay;

            buttonIndex1 = 0;
            buttonIndex2 = 0;

            AddFrameToReplay(new TauReplayFrame(-100000, new Vector2(Offset, Offset + 150)));

            foreach (var h in Beatmap.HitObjects)
                addHitObjectReplay(h);

            return Replay;
        }

        private void addHitObjectReplay(TauHitObject h)
        {
            var lastFrame = (TauReplayFrame)Frames[^1];
            var startPosition = h switch
            {
                IHasAngle ang => getGameplayPositionFromAngle(ang.Angle),
                _ => lastFrame.Position
            };

            // Do some nice easing for cursor movements
            if (Frames.Count > 0)
            {
                moveToHitObject(h, startPosition, preferredEasing);
            }

            addHitObjectClickFrames(h, startPosition);
        }

        private void moveToHitObject(TauHitObject h, Vector2 targetPos, Easing easing)
        {
            var lastFrame = (TauReplayFrame)Frames[^1];

            var waitTime = h.StartTime - Math.Max(0.0, h.TimePreempt - getReactionTime(h.StartTime - h.TimePreempt));
            var hasWaited = false;

            if (waitTime > lastFrame.Time)
            {
                lastFrame = new TauReplayFrame(waitTime, lastFrame.Position) { Actions = lastFrame.Actions };
                hasWaited = true;
                AddFrameToReplay(lastFrame);
            }

            var timeDifference = ApplyModsToTimeDelta(lastFrame.Time, h.StartTime);
            var lastLastFrame = Frames.Count >= 2 ? (TauReplayFrame)Frames[^2] : null;

            if (timeDifference > 0)
            {
                if (lastLastFrame != null && lastFrame is TauKeyUpReplayFrame && !hasWaited)
                {
                    lastFrame.Position = Interpolation.ValueAt(lastFrame.Time, lastFrame.Position, targetPos, lastLastFrame.Time, h.StartTime, easing);
                }

                var lastPosition = lastFrame.Position;

                for (double time = lastFrame.Time + GetFrameDelay(lastFrame.Time); time < h.StartTime; time += GetFrameDelay(time))
                {
                    Vector2 currentPosition = Interpolation.ValueAt(time, lastPosition, targetPos, lastFrame.Time, h.StartTime, easing);
                    AddFrameToReplay(new TauReplayFrame((int)time, new Vector2(currentPosition.X, currentPosition.Y)) { Actions = lastFrame.Actions });
                }
            }

            if (h is HardBeat)
            {
                // Start alternating once the time separation is too small (faster than ~225BPM).
                if (timeDifference is > 0 and < 266)
                    buttonIndex2++;
                else
                    buttonIndex2 = 0;
            }
            else
            {
                // Start alternating once the time separation is too small (faster than ~225BPM).
                if (timeDifference is > 0 and < 266)
                    buttonIndex1++;
                else
                    buttonIndex1 = 0;
            }
        }

        private void addHitObjectClickFrames(TauHitObject h, Vector2 startPosition)
        {
            var action = buttonIndex1 % 2 == 0 ? TauAction.LeftButton : TauAction.RightButton;
            if (h is HardBeat or StrictHardBeat)
                action = buttonIndex2 % 2 == 0 ? TauAction.HardButton1 : TauAction.HardButton2;

            var startFrame = new TauReplayFrame(h.StartTime, startPosition, action);

            double hEndTime = h.GetEndTime() + KEY_UP_DELAY;
            var endFrame = new TauKeyUpReplayFrame(hEndTime, startPosition);

            var index = FindInsertionIndex(startFrame) - 1;

            // This is commented out due to an error being thrown while attempting to generate an Autoplay for this specific map: https://osu.ppy.sh/beatmapsets/1508499
            // Currently i'm just being lazy to actually fix the issue lmao
            // ~ Nora
            if (index >= 0)
            {
                var previousFrame = (TauReplayFrame)Frames[index];
                var previousActions = previousFrame.Actions;

                if (previousActions.Any())
                {
                    if (previousActions.Contains(action))
                    {
                        if (h is HardBeat or StrictHardBeat)
                            action = action == TauAction.HardButton1 ? TauAction.HardButton2 : TauAction.HardButton1;
                        else
                            action = action == TauAction.LeftButton ? TauAction.RightButton : TauAction.LeftButton;

                        startFrame.Actions.Clear();
                        startFrame.Actions.Add(action);
                    }

                    var endIndex = FindInsertionIndex(endFrame);

                    if (index < Frames.Count - 1)
                        Frames.RemoveRange(index + 1, Math.Max(0, endIndex - (index + 1)));

                    for (int j = index + 1; j < Frames.Count; ++j)
                    {
                        var frame = (TauReplayFrame)Frames[j];

                        if (j < Frames.Count - 1 || frame.Actions.SequenceEqual(previousActions))
                        {
                            frame.Actions.Clear();
                            frame.Actions.Add(action);
                        }
                    }
                }
            }

            AddFrameToReplay(startFrame);

            if (h is Slider s)
            {
                if (s.IsHard)
                    action = buttonIndex2 % 2 == 0 ? TauAction.HardButton1 : TauAction.HardButton2;

                foreach (var node in s.Path.Nodes)
                {
                    var pos = getGameplayPositionFromAngle(s.GetAbsoluteAngle(node));
                    AddFrameToReplay(new TauReplayFrame(h.StartTime + node.Time, pos, action));
                }

                endFrame.Position = getGameplayPositionFromAngle(s.GetAbsoluteAngle(s.Path.EndNode));
            }

            if (Frames[^1].Time <= endFrame.Time)
                AddFrameToReplay(endFrame);
        }

        #endregion

        #region Helper subroutines

        /// <summary>
        /// Calculates the "reaction time" in ms between "seeing" a new hit object and moving to "react" to it.
        /// </summary>
        /// <remarks>
        /// Already superhuman, but still somewhat realistic.
        /// </remarks>
        private double getReactionTime(double timeInstant) => ApplyModsToRate(timeInstant, 100);

        private Vector2 getGameplayPositionFromAngle(float angle) => Extensions.FromPolarCoordinates(CURSOR_DISTANCE, angle) + new Vector2(Offset);

        #endregion

        private class TauKeyUpReplayFrame : TauReplayFrame
        {
            public TauKeyUpReplayFrame(double time, Vector2 position)
                : base(time, position)
            {
            }
        }
    }
}
