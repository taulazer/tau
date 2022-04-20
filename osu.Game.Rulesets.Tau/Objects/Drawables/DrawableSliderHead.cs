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

        // TODO: This probably shouldn't be here?
        protected override float GetCurrentOffset() => Slider.HitObject.Angle;
    }
}
