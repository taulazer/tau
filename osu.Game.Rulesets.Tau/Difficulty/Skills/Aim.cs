using System;
using osu.Game.Beatmaps;
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

        protected override double SkillMultiplier => 370;
        protected override double StrainDecayBase => 0.15;

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            // No need to aim HardBeat
            if (current.BaseObject is HardBeat) { return 0; }

            var tauCurrent = (TauDifficultyHitObject)current;

            var note = (TauHitObject)current.BaseObject;
            var notePrev = (TauHitObject)current.LastObject;

            var noteDif = (TauDifficultyHitObject)current;

            var paddleSize = BeatmapDifficulty.DifficultyRange(noteDif.Beatmap.BeatmapInfo.BaseDifficulty.CircleSize, 1, 4, 8);
            var jumpAngle = Math.Abs(note.Angle - notePrev.Angle) * 0.5f;

            var paddleSizeBonus = (0.01f * Math.Pow(paddleSize - 4, 3)) + 1;

            double result = 0;
            double angleBonus;

            double speedmult = 1;
            if (Previous.Count > 0)
            {
                var tauPrevious = (TauDifficultyHitObject)Previous[0];
                var x = tauPrevious.StrainTime;
                var y = -Math.Log10(tauPrevious.StrainTime);
                speedmult = Math.Max(y + 3.2, 0);
                if (tauCurrent.Angle != null && tauCurrent.Angle.Value > angle_bonus_begin)
                {
                    const double min_jump = 5;

                    angleBonus = Math.Sqrt(
                        Math.Max(tauPrevious.JumpDistance - min_jump, 0)
                        * Math.Pow(Math.Sin(tauCurrent.Angle.Value - angle_bonus_begin), 2)
                        * Math.Max(tauCurrent.JumpDistance - min_jump, 0));
                    result = 30 * applyDiminishingExp(Math.Max(0, angleBonus)) / Math.Max(timing_threshold, tauPrevious.StrainTime);
                }
            }

            double jumpDistanceExp = applyDiminishingExp(tauCurrent.JumpDistance);
            double travelDistanceExp = applyDiminishingExp(tauCurrent.TravelDistance);

            double angleStrain = result + ((jumpDistanceExp + travelDistanceExp + Math.Sqrt(travelDistanceExp * jumpDistanceExp)) / Math.Max(tauCurrent.StrainTime, timing_threshold));
            double flatStrain = (Math.Sqrt(travelDistanceExp * jumpDistanceExp) + jumpDistanceExp + travelDistanceExp) / tauCurrent.StrainTime;

            // strainPeaks.Add(Math.Max(option1,option2));
            return Math.Max(
                angleStrain * paddleSizeBonus,
                flatStrain * paddleSizeBonus
            ) * speedmult;
        }

        private double applyDiminishingExp(double val) => Math.Pow(val, 0.99);
    }
}
