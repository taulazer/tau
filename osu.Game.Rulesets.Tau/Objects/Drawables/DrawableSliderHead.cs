namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableSliderHead : DrawableBeat
    {
        public new SliderHeadBeat HitObject => (SliderHeadBeat)base.HitObject;

        public DrawableSliderHead()
        {
        }

        public DrawableSliderHead(SliderHeadBeat obj)
            : base(obj)
        {
        }
    }
}
