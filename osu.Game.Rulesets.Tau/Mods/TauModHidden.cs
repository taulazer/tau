using System;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModHidden : ModHidden, IApplicableToDrawableRuleset<TauHitObject>
    {
        public override string Description => @"Play with no beats and fading sliders.";
        public override double ScoreMultiplier => 1.06;
        public override IconUsage? Icon => TauIcon.ModHidden;
        public override Type[] IncompatibleMods => new[] { typeof(TauModInverse) };

        public virtual void ApplyToDrawableRuleset(DrawableRuleset<TauHitObject> drawableRuleset)
        {
            TauPlayfield tauPlayfield = (TauPlayfield)drawableRuleset.Playfield;

            var HOC = tauPlayfield.HitObjectContainer;
            Container HOCParent = (Container)tauPlayfield.HitObjectContainer.Parent;

            HOCParent.Remove(HOC);

            HOCParent.Add(new PlayfieldMaskingContainer(HOC, Mode)
            {
                Coverage = InitialCoverage,
            });
        }

        protected override void ApplyIncreasedVisibilityState(DrawableHitObject hitObject, ArmedState state)
        {
        }

        protected override void ApplyNormalVisibilityState(DrawableHitObject hitObject, ArmedState state)
        {
        }

        protected virtual MaskingMode Mode => MaskingMode.FadeOut;

        protected virtual float InitialCoverage => 0.4f;
    }
}
