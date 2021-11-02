using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Judgements
{
    public class TauJudgementResult : JudgementResult
    {
        public TauHitObject TauHitObject => (TauHitObject)HitObject;

        public float? DeltaAngle;

        public TauJudgementResult(HitObject hitObject, Judgement judgement)
            : base(hitObject, judgement)
        {
        }
    }
}
