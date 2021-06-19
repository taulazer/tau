using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
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

        public override bool HasImplementation => false;

        public override void ApplyToDrawableHitObject(DrawableHitObject drawableHitObject)
        {
            drawableHitObject.HitObjectApplied += applyFadeInAdjustment;

            base.ApplyToDrawableHitObject(drawableHitObject);
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
            var fadeOutDuration = h.TimePreempt * fade_out_duration_multiplier;

            // future proofing yet again.
            switch (drawable)
            {
                case DrawableTauHitObject beat:
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
