using osu.Framework.Bindables;

namespace osu.Game.Rulesets.Tau.Objects
{
    /// <summary>
    /// A <see cref="TauHitObject"/> that has an angle parameter.
    /// </summary>
    public class AngledTauHitObject : TauHitObject, IHasAngle
    {
        public BindableFloat AngleBindable = new();

        public float Angle
        {
            get => AngleBindable.Value;
            set => AngleBindable.Value = value;
        }
    }
}
