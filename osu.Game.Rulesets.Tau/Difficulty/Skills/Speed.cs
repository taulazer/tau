using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Difficulty.Evaluators;

namespace osu.Game.Rulesets.Tau.Difficulty.Skills
{
    public class Speed : TauStrainSkill
    {
        private const double skill_multiplier = 515;
        private const double strain_decay_base = 0.3;

        private readonly double greatWindow;

        private double currentStrain;
        private double currentRhythm;

        protected override int ReducedSectionCount => 5;
        protected override double DifficultyMultiplier => 1.37;

        public Speed(Mod[] mods, double hitWindowGreat)
            : base(mods)
        {
            greatWindow = hitWindowGreat;
        }

        protected override double StrainValueAt(DifficultyHitObject current)
        {
            currentStrain *= strainDecay(current.DeltaTime);
            currentStrain += SpeedEvaluator.EvaluateDifficulty(current, greatWindow) * skill_multiplier;

            currentRhythm = RhythmEvaluator.EvaluateDifficulty(current, greatWindow);

            return currentStrain * currentRhythm;
        }

        protected override double CalculateInitialStrain(double time, DifficultyHitObject current)
            => (currentStrain * currentRhythm) * strainDecay(time - current.Previous(0).StartTime);

        private double strainDecay(double ms) => Math.Pow(strain_decay_base, ms / 1000);
    }
}
