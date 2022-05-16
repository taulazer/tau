using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class SliderRepeat : AngledTauHitObject, IHasOffsetAngle
    {
        public int RepeatIndex { get; set; }

        public Slider ParentSlider { get; set; }

        public float GetOffsetAngle() => ParentSlider.Angle;

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}
