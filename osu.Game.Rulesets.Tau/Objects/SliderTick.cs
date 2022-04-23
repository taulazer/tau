namespace osu.Game.Rulesets.Tau.Objects
{
    public class SliderTick : AngledTauHitObject, IHasOffsetAngle
    {
        public int TickIndex { get; set; }
        public Slider ParentSlider { get; set; }

        public float GetOffsetAngle() => ParentSlider.Angle;
    }
}
