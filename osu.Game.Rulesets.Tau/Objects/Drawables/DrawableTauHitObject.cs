using System.Diagnostics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;

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

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyResult(r => r.Type = HitResult.Miss);

                return;
            }

            if (!CheckForValidation())
                return;

            var result = HitObject.HitWindows.ResultFor(timeOffset);

            if (result == HitResult.None)
                return;

            ApplyResult(r => r.Type = result);
        }

        protected virtual bool CheckForValidation() => true;
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
