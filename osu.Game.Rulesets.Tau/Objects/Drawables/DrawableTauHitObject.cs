using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableTauHitObject<T> : DrawableHitObject<TauHitObject>
        where T : TauHitObject
    {
        protected new T HitObject => (T)base.HitObject;

        protected DrawableTauHitObject(T obj)
            : base(obj)
        {
        }

        protected override double InitialLifetimeOffset => HitObject.TimePreempt;
    }
}
