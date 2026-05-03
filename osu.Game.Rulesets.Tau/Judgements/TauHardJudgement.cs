using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Tau.Judgements
{
    public class TauHardJudgement : TauJudgement
    {
        public override HitResult MaxResult => HitResult.LargeBonus;

        protected override double HealthIncreaseFor(HitResult result) => 0;
    }
}
