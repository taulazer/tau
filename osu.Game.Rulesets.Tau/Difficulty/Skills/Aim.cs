using System;
using System.Linq;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Difficulty.Preprocessing;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Difficulty.Skills
{
    public class Aim : StrainDecaySkill
    {
        private Type[] AllowedObjectTypes;

        protected override int HistoryLength => 10;
        protected override double SkillMultiplier => 50;
        private const double slider_multiplier = 1.5;
        protected override double StrainDecayBase => 0.2;

        public Aim(Mod[] mods, params Type[] allowedObjectTypes)
            : base(mods)
        {
            AllowedObjectTypes = allowedObjectTypes;
        }

        private double calculateStrain(TauAngledDifficultyHitObject current, TauAngledDifficultyHitObject last)
        {
            double velocity = current.Distance / current.StrainTime;

            if (!AllowedObjectTypes.Any(t => t == typeof(Slider) && last.BaseObject is Slider))
                return velocity;

            // Slider calculation

            if (!(last.TravelDistance >= current.AngleRange))
                return velocity;

            double travelVelocity = last.LazyTravelDistance / last.TravelTime;
            double movementVelocity = current.Distance / current.StrainTime;

            velocity = Math.Max(velocity, movementVelocity + travelVelocity);
            velocity += (last.TravelDistance / last.TravelTime) * slider_multiplier;

            return velocity;
        }

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            if (Previous.Count <= 1 || current is not TauAngledDifficultyHitObject)
                return 0;

            // Get the last available angled hit object in the history.
            var prevAngled = Previous.Where(p => p is TauAngledDifficultyHitObject);
            if (!prevAngled.Any())
                return 0;

            var tauCurrObj = (TauAngledDifficultyHitObject)current;
            var tauLastObj = (TauAngledDifficultyHitObject)prevAngled.First();

            if (tauCurrObj.Distance == 0 || !(tauCurrObj.Distance >= tauCurrObj.AngleRange))
                return 0;

            return calculateStrain(tauCurrObj, tauLastObj);
        }

        #region PP Calculation

        public static double ComputePerformance(TauPerformanceContext context)
        {
            double rawAim = context.DifficultyAttributes.AimDifficulty;
            double aimValue = Math.Pow(5.0 * Math.Max(1.0, rawAim / 0.0675) - 4.0, 3.0) / 100000.0; // TODO: Figure values here.

            double lengthBonus = 0.95 + 0.4 * Math.Min(1.0, context.TotalHits / 2000.0) + (context.TotalHits > 2000 ? Math.Log10(context.TotalHits / 2000.0) * 0.5 : 0.0); // TODO: Figure values here.
            aimValue *= lengthBonus;
            TauDifficultyAttributes attributes = context.DifficultyAttributes;

            double approachRateFactor = 0.0;
            if (attributes.ApproachRate > 10.33)
                approachRateFactor = 0.3 * (attributes.ApproachRate - 10.33);
            else if (attributes.ApproachRate < 8.0)
                approachRateFactor = 0.1 * (8.0 - attributes.ApproachRate);

            aimValue *= 1.0 + approachRateFactor * lengthBonus; // Buff for longer maps with high AR.

            // Penalize misses by assessing # of misses relative to the total # of objects. Default a 3% reduction for any # of misses.
            if (context.EffectiveMissCount > 0)
                aimValue *= 0.97 * Math.Pow(1 - Math.Pow(context.EffectiveMissCount / context.TotalHits, 0.775), context.EffectiveMissCount); // TODO: Figure values here.

            // We assume 15% of sliders in a map are difficult since there's no way to tell from the performance calculator.
            double estimateDifficultSliders = attributes.SliderCount * 0.15;

            if (attributes.SliderCount > 0)
            {
                double estimateSliderEndsDropped = Math.Clamp(Math.Min(context.CountOk + context.CountMiss, attributes.MaxCombo - context.ScoreMaxCombo), 0,
                    estimateDifficultSliders);
                double sliderNerfFactor = (1 - attributes.SliderFactor) * Math.Pow(1 - estimateSliderEndsDropped / estimateDifficultSliders, 3) + attributes.SliderFactor;
                aimValue *= sliderNerfFactor;
            }

            aimValue *= context.Accuracy;

            return aimValue;
        }

        #endregion
    }
}
