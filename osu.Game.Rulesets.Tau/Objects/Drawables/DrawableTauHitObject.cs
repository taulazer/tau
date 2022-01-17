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

        /// <summary>
        /// A list of <see cref="TauAction"/>s that denotes which keys can trigger this Hit object.
        /// </summary>
        protected virtual TauAction[] Actions { get; set; } =
        {
            TauAction.LeftButton,
            TauAction.RightButton
        };

        protected override double InitialLifetimeOffset => HitObject.TimePreempt;
    }

    public struct ValidationResult
    {
        public bool IsValid;
        public float DeltaFromPaddleCenter;

        public ValidationResult(bool isValid, float delta)
        {
            IsValid = isValid;
            DeltaFromPaddleCenter = delta;
        }
    }
}
