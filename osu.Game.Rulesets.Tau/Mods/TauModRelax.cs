using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using osu.Game.Input.Handlers;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Play;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModRelax : ModRelax, IUpdatableByPlayfield, IApplicableToDrawableRuleset<TauHitObject>, IApplicableToPlayer
    {
        public override string Description => @"You don't need to click. Give your clicking/tapping fingers a break from the heat of things.";
        public override Type[] IncompatibleMods => base.IncompatibleMods.Concat(new[] { typeof(TauModAutopilot) }).ToArray();

        /// <summary>
        /// How early before a hitobject's start time to trigger a hit.
        /// </summary>
        private const float relax_leniency = 3;

        private (bool isDown, bool wasLeft) normal;
        private (bool isDown, bool wasLeft) hardBeat;

        private TauInputManager tauInputManager;

        private ReplayInputHandler.ReplayState<TauAction> state;
        private double lastStateChangeTime;

        private bool hasReplay;

        public void ApplyToDrawableRuleset(DrawableRuleset<TauHitObject> drawableRuleset)
        {
            tauInputManager = (TauInputManager)drawableRuleset.KeyBindingInputManager;
        }

        public void ApplyToPlayer(Player player)
        {
            if (tauInputManager.ReplayInputHandler != null)
            {
                hasReplay = true;
                return;
            }

            tauInputManager.AllowUserPresses = false;
        }

        public void Update(Playfield playfield)
        {
            if (hasReplay)
                return;

            checkNormal(playfield, playfield.HitObjectContainer.AliveObjects.OfType<DrawableHitObject<TauHitObject>>().Where(o => o is not DrawableHardBeat));
            checkHardBeat(playfield, playfield.HitObjectContainer.AliveObjects.OfType<DrawableHardBeat>());
        }

        private void checkNormal(Playfield playfield, IEnumerable<DrawableHitObject<TauHitObject>> hitObjects)
        {
            bool requiresHold = false;
            bool requiresHit = false;
            bool requiresHard = false;
            double time = playfield.Clock.CurrentTime;

            foreach (var h in hitObjects)
            {
                // we are not yet close enough to the object.
                if (time < h.HitObject.StartTime - relax_leniency)
                    break;

                // already hit or beyond the hittable end time.
                if (h.IsHit || (h.HitObject is IHasDuration hasEnd && time > hasEnd.EndTime))
                    continue;

                switch (h)
                {
                    case DrawableBeat beat:
                        handleAngled(beat);
                        break;

                    case DrawableSlider slider:
                        switch (slider.SliderHead)
                        {
                            case DrawableSliderHead head:
                                if (!head.IsHit)
                                    handleAngled(head);
                                break;

                            case DrawableSliderHardBeat head:
                                if (!head.IsHit)
                                    handleAngled(head, true);
                                break;
                        }

                        requiresHold |= slider.IsWithinPaddle();
                        break;

                    case DrawableStrictHardBeat strict:
                        handleAngled(strict, true);
                        break;
                }
            }

            if (requiresHit)
            {
                changeNormalState(false, time);
                changeNormalState(true, time);
            }

            if (requiresHard)
            {
                changeHardBeatState(false, time);
                changeHardBeatState(true, time);
            }

            if (hardBeat.isDown && time - lastStateChangeTime > AutoGenerator.KEY_UP_DELAY)
                changeHardBeatState(false, time);

            if (requiresHold)
                changeNormalState(true, time);
            else if (normal.isDown && time - lastStateChangeTime > AutoGenerator.KEY_UP_DELAY)
                changeNormalState(false, time);

            void handleAngled<T>(DrawableAngledTauHitObject<T> obj, bool isHard = false)
                where T : AngledTauHitObject
            {
                if (!obj.IsWithinPaddle())
                    return;

                Debug.Assert(obj.HitObject.HitWindows != null);
                if (isHard)
                    requiresHard |= obj.HitObject.HitWindows.CanBeHit(time - obj.HitObject.StartTime);
                else
                    requiresHit |= obj.HitObject.HitWindows.CanBeHit(time - obj.HitObject.StartTime);
            }
        }

        private void checkHardBeat(Playfield playfield, IEnumerable<DrawableHardBeat> hitObjects)
        {
            bool requiresHit = false;
            double time = playfield.Clock.CurrentTime;

            foreach (var hb in hitObjects)
            {
                // we are not yet close enough to the object.
                if (time < hb.HitObject.StartTime - relax_leniency)
                    break;

                if (hb.IsHit)
                    continue;

                Debug.Assert(hb.HitObject.HitWindows != null);
                requiresHit |= hb.HitObject.HitWindows.CanBeHit(time - hb.HitObject.StartTime);
            }

            if (requiresHit)
            {
                changeHardBeatState(false, time);
                changeHardBeatState(true, time);
            }

            if (hardBeat.isDown && time - lastStateChangeTime > AutoGenerator.KEY_UP_DELAY)
                changeHardBeatState(false, time);
        }

        private void changeState(bool down, double time, ref (bool isDown, bool wasLeft) hitObject, TauAction left, TauAction right)
        {
            if (hitObject.isDown == down)
                return;

            hitObject.isDown = down;
            lastStateChangeTime = time;

            state = new ReplayInputHandler.ReplayState<TauAction>()
            {
                PressedActions = new List<TauAction>()
            };

            if (down)
            {
                state.PressedActions.Add(hitObject.wasLeft ? left : right);
                hitObject.wasLeft = !hitObject.wasLeft;
            }

            state?.Apply(tauInputManager.CurrentState, tauInputManager);
        }

        private void changeHardBeatState(bool down, double time)
            => changeState(down, time, ref hardBeat, TauAction.HardButton1, TauAction.HardButton2);

        private void changeNormalState(bool down, double time)
            => changeState(down, time, ref normal, TauAction.LeftButton, TauAction.RightButton);
    }
}
