using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public partial class DrawableSliderTick : DrawableAngledTauHitObject<SliderTick>
    {
        public DrawableSlider DrawableSlider => (DrawableSlider)ParentHitObject;

        public override bool DisplayResult => false;

        public DrawableSliderTick()
            : this(null)
        {
        }

        public DrawableSliderTick(SliderTick hitObject)
            : base(hitObject)
        {
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (HitObject.StartTime <= Time.Current)
                ApplyResult(r => r.Type = DrawableSlider.Tracking.Value ? HitResult.SmallTickHit : HitResult.SmallTickMiss);
        }
    }
}
