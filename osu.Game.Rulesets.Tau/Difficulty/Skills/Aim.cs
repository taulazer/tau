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
    }
}
