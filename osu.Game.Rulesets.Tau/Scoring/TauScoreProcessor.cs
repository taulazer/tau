using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Judgements;
using osuTK;

namespace osu.Game.Rulesets.Tau.Scoring
{
    public class TauScoreProcessor : ScoreProcessor
    {
        public TauScoreProcessor(TauRuleset ruleset) : base(ruleset) { }

        protected override HitEvent CreateHitEvent(JudgementResult result)
            => base.CreateHitEvent(result).With(new Vector2((result as TauJudgementResult)?.DeltaAngle ?? 0, 0)); // Just use the X value as the delta value.

        protected override JudgementResult CreateResult(HitObject hitObject, Judgement judgement) => new TauJudgementResult(hitObject, judgement);
    }
}
