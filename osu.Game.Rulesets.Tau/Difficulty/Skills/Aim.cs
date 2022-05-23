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

        protected override int HistoryLength => 2;
        protected override double SkillMultiplier => 50;
        private const double slider_multiplier = 1.5;
        protected override double StrainDecayBase => 0.2;

        public Aim(Mod[] mods, params Type[] allowedObjectTypes)
            : base(mods)
        {
            AllowedObjectTypes = allowedObjectTypes;
        }

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            if (Previous.Count <= 1)
                return 0;

            if (current is not TauAngledDifficultyHitObject || Previous[0] is not TauAngledDifficultyHitObject)
                return 0;

            var tauCurrObj = (TauAngledDifficultyHitObject)current;
            var tauLastObj = (TauAngledDifficultyHitObject)Previous[0]; // TODO: "Last Object" should be the last available angled hit object.

            if (tauCurrObj.Distance == 0)
                return 0;

            if (!(tauCurrObj.Distance >= tauCurrObj.AngleRange)) return 0;

            double currVelocity = tauCurrObj.Distance / tauCurrObj.StrainTime;

            if (AllowedObjectTypes.Any(t => t == typeof(Slider)) && tauLastObj.BaseObject is Slider && tauLastObj.TravelDistance < tauCurrObj.AngleRange)
            {
                double travelVelocity = tauLastObj.TravelDistance / tauLastObj.TravelTime; // calculate the slider velocity from slider head to slider end.
                double movementVelocity = tauCurrObj.Distance / tauCurrObj.StrainTime; // calculate the movement velocity from slider end to current object

                currVelocity = Math.Max(currVelocity, movementVelocity + travelVelocity); // take the larger total combined velocity.
            }

            double sliderBonus = 0;
            double aimStrain = currVelocity;

            // Sliders should be treated as beats if their travel distance is short enough.
            if (tauLastObj.TravelTime != 0 && tauLastObj.TravelDistance >= tauCurrObj.AngleRange)
            {
                // Reward sliders based on velocity.
                sliderBonus = tauLastObj.TravelDistance / tauLastObj.TravelTime;
            }

            // Add in additional slider velocity bonus.
            if (AllowedObjectTypes.Any(t => t == typeof(Slider)))
                aimStrain += sliderBonus * slider_multiplier;
            return aimStrain;
        }

        #region PP Calculation

        public static double ComputePerformance(TauPerformanceContext context)
        {
            double rawAim = context.DifficultyAttributes.AimDifficulty;
            double aimValue = Math.Pow(5.0 * Math.Max(1.0, rawAim / 0.0675) - 4.0, 3.0) / 100000.0; // TODO: Figure values here.

            double lengthBonus = 0.95 +
                                 0.4 * Math.Min(1.0, context.TotalHits / 2000.0) +
                                 (context.TotalHits > 2000 ? Math.Log10(context.TotalHits / 2000.0) * 0.5 : 0.0); // TODO: Figure values here.
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
                aimValue *= 0.97 *
                            Math.Pow(1 - Math.Pow(context.EffectiveMissCount / context.TotalHits, 0.775), context.EffectiveMissCount); // TODO: Figure values here.

            // We assume 15% of sliders in a map are difficult since there's no way to tell from the performance calculator.
            double estimateDifficultSliders = attributes.SliderCount * 0.15;

            if (attributes.SliderCount > 0)
            {
                double estimateSliderEndsDropped = Math.Clamp(Math.Min(context.CountOk + context.CountMiss, attributes.MaxCombo - context.ScoreMaxCombo), 0,
                    estimateDifficultSliders);
                double sliderNerfFactor = (1 - attributes.SliderFactor) * Math.Pow(1 - estimateSliderEndsDropped / estimateDifficultSliders, 3) +
                                          attributes.SliderFactor;
                aimValue *= sliderNerfFactor;
            }

            aimValue *= context.Accuracy;

            return aimValue;
        }

        #endregion
    }
}
