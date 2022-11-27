using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Difficulty.Evaluators;
using osu.Game.Rulesets.Tau.Difficulty.Preprocessing;

namespace osu.Game.Rulesets.Tau.Difficulty.Skills
{
    public class Aim : StrainDecaySkill
    {
        private readonly Type[] allowedHitObjects;

        protected override double SkillMultiplier => 8;
        protected override double StrainDecayBase => 0.25;

        public Aim(Mod[] mods, Type[] allowedHitObjects)
            : base(mods)
        {
            this.allowedHitObjects = allowedHitObjects;
        }

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            if (current.Index <= 1 || current is not TauAngledDifficultyHitObject tauCurrObj || tauCurrObj.LastAngled == null)
                return 0;

            if (tauCurrObj.Distance < tauCurrObj.AngleRange)
                return 0;

            return AimEvaluator.EvaluateDifficulty(tauCurrObj, tauCurrObj.LastAngled, allowedHitObjects);
        }
    }
}
