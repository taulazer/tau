using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Difficulty.Evaluators;

namespace osu.Game.Rulesets.Tau.Difficulty.Skills
{
    public class Complexity : StrainDecaySkill
    {
        protected override double SkillMultiplier => 55;

        protected override double StrainDecayBase => 0.35;

        public Complexity(Mod[] mods)
            : base(mods)
        {
        }

        protected override double StrainValueOf(DifficultyHitObject current)
            => ComplexityEvaluator.EvaluateDifficulty(current);
    }
}
