using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Judgements;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class SliderTick : AngledTauHitObject, IHasOffsetAngle
    {
        public Slider ParentSlider { get; set; }

        public float GetOffsetAngle() => ParentSlider.Angle;

        public override Judgement CreateJudgement() => new TauTickJudgement();

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}
