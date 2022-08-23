using osuTK;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableSliderHardBeat : DrawableStrictHardBeat
    {
        public DrawableSlider DrawableSlider => (DrawableSlider)ParentHitObject;

        public DrawableSliderHardBeat()
        {
        }

        public DrawableSliderHardBeat(SliderHardBeat hitObject)
            : base(hitObject)
        {
            Scale = new Vector2(1.05f);
        }

        protected override float GetSliderOffset() => DrawableSlider.HitObject.Angle;

        public float GetAbsoluteAngle() => HitObject.Angle + GetCurrentOffset();
    }
}
