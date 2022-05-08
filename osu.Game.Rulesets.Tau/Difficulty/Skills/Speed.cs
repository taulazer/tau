using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Difficulty.Preprocessing;

namespace osu.Game.Rulesets.Tau.Difficulty.Skills
{
    public class Speed : StrainSkill
    {
        public Speed(Mod[] mods)
            : base(mods)
        {
        }

        protected override double StrainValueAt(DifficultyHitObject current)
        {
            var tauObject = current as TauDifficultyHitObject;

            throw new System.NotImplementedException();
        }

        protected override double CalculateInitialStrain(double time)
        {
            throw new System.NotImplementedException();
        }
    }
}
