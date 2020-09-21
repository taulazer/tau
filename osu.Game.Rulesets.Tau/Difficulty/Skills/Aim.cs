using System;
using System.Collections.Generic;
using System.Linq;

using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Tau.Difficulty.Preprocessing;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Difficulty.Skills
{
    /// <summary>
    /// Represents the skill required to correctly aim at every object in the map with a uniform CircleSize and normalized distances.
    /// </summary>
    public class Aim : Skill
    {
        private const double angle_bonus_begin = Math.PI / 6;
        private const double timing_threshold = 107;

        // public List<Double> strainPeaks = new List<Double>();
        protected override double SkillMultiplier => 60;//26.25;
        protected override double StrainDecayBase => 0.15;

        protected override double StrainValueOf(DifficultyHitObject current)
        {
        
            var tauCurrent = (TauDifficultyHitObject)current;

            double result = 0;
            double angleBonus;
            if (Previous.Count > 0)
            {
                var osuPrevious = (TauDifficultyHitObject)Previous[0];

                if (tauCurrent.Angle != null && tauCurrent.Angle.Value > angle_bonus_begin)
                {
                    const double scale = 5;

                    angleBonus = Math.Sqrt(
                        Math.Max(osuPrevious.JumpDistance - scale, 0)
                        * Math.Pow(Math.Sin(tauCurrent.Angle.Value - angle_bonus_begin), 2)
                        * Math.Max(tauCurrent.JumpDistance - scale, 0));
                    result = 30 * applyDiminishingExp(Math.Max(0, angleBonus)) / Math.Max(timing_threshold, osuPrevious.StrainTime);
                }
            }

            double jumpDistanceExp = applyDiminishingExp(tauCurrent.JumpDistance);
            double travelDistanceExp = applyDiminishingExp(tauCurrent.TravelDistance);

            double angle_strain = (result + (jumpDistanceExp + travelDistanceExp + Math.Sqrt(travelDistanceExp * jumpDistanceExp)) / Math.Max(tauCurrent.StrainTime, timing_threshold));
            double flat_strain = (Math.Sqrt(travelDistanceExp * jumpDistanceExp) + jumpDistanceExp + travelDistanceExp) / tauCurrent.StrainTime;

            // strainPeaks.Add(Math.Max(option1,option2));
            return Math.Max(
                angle_strain,
                flat_strain
            );
        }

        private double applyDiminishingExp(double val) => Math.Pow(val, 0.99);

    
    }
}