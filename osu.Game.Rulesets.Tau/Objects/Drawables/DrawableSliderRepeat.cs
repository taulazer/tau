using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableSliderRepeat : DrawableBeat
    {
        public DrawableSlider DrawableSlider => (DrawableSlider)ParentHitObject;

        public override bool DisplayResult => false;

        public DrawableSliderRepeat()
        {
        }

        public DrawableSliderRepeat(Beat hitObject)
            : base(hitObject)
        {
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            DrawableBox.Size = Vector2.Multiply(DrawableBox.Size, 0.75f);
            DrawableBox.Rotation = 45;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (HitObject.StartTime <= Time.Current)
                ApplyResult(r => r.Type = DrawableSlider.Tracking.Value ? HitResult.Great : HitResult.Miss);
        }
    }
}
