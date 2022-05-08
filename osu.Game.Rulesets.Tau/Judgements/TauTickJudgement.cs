using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Tau.Judgements
{
    public class TauTickJudgement : Judgement
    {
        public override HitResult MaxResult => HitResult.LargeTickHit;
    }
}
