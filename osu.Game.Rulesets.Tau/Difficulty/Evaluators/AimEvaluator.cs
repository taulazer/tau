using System;
using System.Linq;
using osu.Game.Rulesets.Tau.Difficulty.Preprocessing;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Difficulty.Evaluators
{
    public static class AimEvaluator
    {
        private const double slider_multiplier = 1.2;

        public static double EvaluateDifficulty(TauAngledDifficultyHitObject current, TauAngledDifficultyHitObject last, Type[] allowedHitObjects)
        {
            double velocity = calculateVelocity(current.Distance, current.StrainTime);

            if (!allowedHitObjects.Any(t => t == typeof(Slider) && last.BaseObject is Slider))
                return velocity;

            // Slider calculation

            if (last.TravelDistance < current.AngleRange)
                return velocity;

            double travelVelocity = calculateVelocity(last.LazyTravelDistance, last.TravelTime);
            double movementVelocity = calculateVelocity(current.Distance, current.StrainTime);

            velocity = Math.Max(velocity, movementVelocity + travelVelocity);
            velocity += calculateVelocity(last.LazyTravelDistance, last.TravelTime) * slider_multiplier;

            return velocity;
        }

        // https://www.desmos.com/calculator/5yu0ov3zka
        private static double calculateVelocity(double distance, double time)
            => distance / (Math.Pow((Math.Pow(time, 1.4) - 77) / 100, 2) * 0.1 + 25);

        /// <summary>
        /// Calculates the computed Aim skill performance value using the AimDifficulty, length bonus, miss count check and slider nerf values based on the play.
        /// </summary>
        /// <param name="context">The performance context for the play.</param>
        public static double EvaluatePerformance(TauPerformanceContext context)
        {
            double rawAim = context.DifficultyAttributes.AimDifficulty;
            double aimValue = Math.Pow(5.0 * Math.Max(1.0, rawAim / 0.0675) - 4.0, 3.0) / 100000.0;

            // Length bonus is added on for beatmaps with more than 2,000 hitobjects.
            double lengthBonus = 0.95 + 0.4 * Math.Min(1.0, context.TotalHits / 2000.0) + (context.TotalHits > 2000 ? Math.Log10(context.TotalHits / 2000.0) * 0.5 : 0.0);
            aimValue *= lengthBonus;

            aimValue *= computeArFactor(context) * lengthBonus;

            // Penalize misses by assessing # of misses relative to the total # of objects. Default a 3% reduction for any # of misses.
            if (context.EffectiveMissCount > 0)
                aimValue *= 0.97 * Math.Pow(1 - Math.Pow(context.EffectiveMissCount / context.TotalHits, 0.775), context.EffectiveMissCount);

            if (context.DifficultyAttributes.SliderCount > 0)
                aimValue *= computeSliderNerf(context);

            return aimValue * context.Accuracy;
        }

        /// <summary>
        /// Calculates an additional factor based on the Approach Rate of the beatmap.
        /// </summary>
        /// <param name="context">The performance context for the play.</param>
        private static double computeArFactor(TauPerformanceContext context)
        {
            var attributes = context.DifficultyAttributes;

            double approachRateFactor = attributes.ApproachRate switch
            {
                > 10.33 => 0.3 * (attributes.ApproachRate - 10.33),
                < 8.0 => 0.1 * (8.0 - attributes.ApproachRate),
                _ => 0.0
            };

            return 1.0 + approachRateFactor;
        }

        /// <summary>
        /// Estimates and buffs 15% of all sliders in the beatmap if the player has completed them correctly.
        /// </summary>
        /// <param name="context">The performance context for the play.</param>
        private static double computeSliderNerf(TauPerformanceContext context)
        {
            var attributes = context.DifficultyAttributes;

            // We assume 15% of sliders in a map are difficult since there's no way to tell from the performance calculator.
            double estimateDifficultSliders = attributes.SliderCount * 0.15;
            double estimateSliderEndsDropped = Math.Clamp(Math.Min(context.CountOk + context.CountMiss, attributes.MaxCombo - context.ScoreMaxCombo), 0,
                estimateDifficultSliders);

            return (1 - attributes.SliderFactor) * Math.Pow(1 - estimateSliderEndsDropped / estimateDifficultSliders, 3) + attributes.SliderFactor;
        }
    }
}
