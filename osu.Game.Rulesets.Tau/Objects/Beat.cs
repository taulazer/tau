using osu.Framework.Bindables;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class Beat : TauHitObject
    {
        public BindableFloat AngleBindable = new BindableFloat();

        public float Angle
        {
            get => AngleBindable.Value;
            set => AngleBindable.Value = value;
        }
    }
}
