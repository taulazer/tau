using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Judgements;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class HardBeatNestedHitObject : TauHitObject
    {
        public readonly HardBeat Parent;

        public HardBeatNestedHitObject(HardBeat parent)
        {
            Parent = parent;
        }

        public override Judgement CreateJudgement() => new TauHardJudgement();
        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}
