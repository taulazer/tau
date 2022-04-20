namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableSliderHead : DrawableBeat
    {
        public DrawableSlider Slider;

        public DrawableSliderHead()
        {
        }

        public DrawableSliderHead(Beat hitObject)
            : base(hitObject)
        {
        }

        protected override float GetCurrentOffset() => Slider.HitObject.Angle;
    }
}
