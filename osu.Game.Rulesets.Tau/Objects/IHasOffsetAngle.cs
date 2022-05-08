namespace osu.Game.Rulesets.Tau.Objects
{
    public interface IHasOffsetAngle : IHasAngle
    {
        public float GetOffsetAngle();

        public float GetAbsoluteAngle() => (Angle + GetOffsetAngle()).Normalize();
    }
}
