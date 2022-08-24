namespace osu.Game.Rulesets.Tau.Objects
{
    public class SliderHardBeat : StrictHardBeat, IHasOffsetAngle
    {
        public Slider ParentSlider { get; set; }

        public float GetOffsetAngle() => ParentSlider.Angle;
    }
}
