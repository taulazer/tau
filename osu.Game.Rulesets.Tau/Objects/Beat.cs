using osu.Framework.Bindables;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class Beat : TauHitObject, IHasAngle
    {
        public BindableFloat AngleBindable = new();

        public float Angle
        {
            get => AngleBindable.Value;
            set => AngleBindable.Value = value;
        }
    }
}
