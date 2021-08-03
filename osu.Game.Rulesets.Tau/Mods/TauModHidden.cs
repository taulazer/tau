using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModHidden : ModHidden
    {
        public override string Description => @"Play with no beats and fading sliders.";
        public override double ScoreMultiplier => 1.06;

        public override Type[] IncompatibleMods => new[] { typeof(TauModAutoHold) };

        private const double fade_in_duration_multiplier = 0.4;
        private const double fade_out_duration_multiplier = 0.3;

        protected override bool IsFirstAdjustableObject(HitObject hitObject) => !(hitObject is HardBeat);

        public override void ApplyToDrawableHitObjects(IEnumerable<DrawableHitObject> drawables)
        {
            foreach (var d in drawables)
                d.HitObjectApplied += applyFadeInAdjustment;

            base.ApplyToDrawableHitObjects(drawables);
        }

        private void applyFadeInAdjustment(DrawableHitObject hitObject)
        {
            if (!(hitObject is DrawableTauHitObject d))
                return;

            d.HitObject.TimeFadeIn = d.HitObject.TimePreempt * fade_in_duration_multiplier;

            foreach (var nested in d.NestedHitObjects)
                applyFadeInAdjustment(nested);
        }

        protected override void ApplyNormalVisibilityState(DrawableHitObject drawable, ArmedState state)
        {
            if (!(drawable is DrawableTauHitObject d))
                return;

            var h = d.HitObject;

            var fadeOutStartTime = h.StartTime - h.TimePreempt + h.TimeFadeIn;
            double fadeOutDuration = 0;

            // future proofing yet again.
            switch (drawable)
            {
                case DrawableSlider slider:
                    fadeOutDuration = (h.TimePreempt + slider.HitObject.Duration) * 0.5;

                    using (drawable.BeginAbsoluteSequence(fadeOutStartTime))
                        slider.FadeOut(fadeOutDuration);

                    break;

                case DrawableTauHitObject beat:
                    fadeOutDuration = h.TimePreempt * fade_out_duration_multiplier;

                    using (drawable.BeginAbsoluteSequence(fadeOutStartTime, true))
                        beat.FadeOut(fadeOutDuration);

                    break;
            }
        }

        protected override void ApplyIncreasedVisibilityState(DrawableHitObject hitObject, ArmedState state)
        {
        }
    }
}
