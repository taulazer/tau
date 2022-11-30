using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Judgements;
using osuTK;

namespace osu.Game.Rulesets.Tau.Scoring
{
    public partial class TauScoreProcessor : ScoreProcessor
    {
        public TauScoreProcessor(Ruleset ruleset)
            : base(ruleset)
        {
        }

        protected override HitEvent CreateHitEvent(JudgementResult result)
            => base.CreateHitEvent(result).With(new Vector2((result as TauJudgementResult)?.DeltaAngle ?? 0, 0));

        protected override JudgementResult CreateResult(HitObject hitObject, Judgement judgement) => new TauJudgementResult(hitObject, judgement);
    }
}
