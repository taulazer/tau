using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Difficulty.Preprocessing;

namespace osu.Game.Rulesets.Tau.Difficulty.Skills
{
    public class Aim : StrainDecaySkill
    {
        protected override double SkillMultiplier => 60;
        protected override double StrainDecayBase => 0.2;

        public Aim(Mod[] mods)
            : base(mods)
        {
        }

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            var diffObject = (TauAngledDifficultyHitObject)current;

            if (diffObject.Distance == 0 || diffObject.DeltaTime == 0)
                return 0;

            if (diffObject.Distance >= diffObject.AngleRange / 2)
                return diffObject.Distance / diffObject.DeltaTime;

            return 0;
        }

        #region PP Calculation

        public static double ComputePerformance(TauPerformanceContext context)
        {
            double rawAim = context.DifficultyAttributes.AimDifficulty;
            double aimValue = Math.Pow(5.0 * Math.Max(1.0, rawAim / 0.0675) - 4.0, 3.0) / 100000.0; // TODO: Figure values here.

            double lengthBonus = 0.95 + 0.4 * Math.Min(1.0, context.TotalHits / 2000.0) +
                                 (context.TotalHits > 2000 ? Math.Log10(context.TotalHits / 2000.0) * 0.5 : 0.0);  // TODO: Figure values here.
            aimValue *= lengthBonus;

            // Penalize misses by assessing # of misses relative to the total # of objects. Default a 3% reduction for any # of misses.
            if (context.EffectiveMissCount > 0)
                aimValue *= 0.97 * Math.Pow(1 - Math.Pow(context.EffectiveMissCount / context.TotalHits, 0.775), context.EffectiveMissCount);  // TODO: Figure values here.

            aimValue *= context.Accuracy;

            return aimValue;
        }

        #endregion
    }
}
