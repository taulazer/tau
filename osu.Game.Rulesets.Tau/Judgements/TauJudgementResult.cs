using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Tau.Judgements
{
    public class TauJudgementResult : JudgementResult
    {
        public float? DeltaAngle;

        public TauJudgementResult(HitObject hitObject, Judgement judgement)
            : base(hitObject, judgement)
        {
        }
    }
}
