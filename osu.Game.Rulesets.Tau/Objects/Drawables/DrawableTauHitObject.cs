using System;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Judgements;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public abstract class DrawableTauHitObject : DrawableHitObject<TauHitObject>
    {
        protected DrawableTauHitObject(TauHitObject obj)
            : base(obj)
        {
        }

        public Func<float, (bool, float)> CheckValidation;

        /// <summary>
        /// A list of keys which can result in hits for this HitObject.
        /// </summary>
        protected virtual TauAction[] HitActions { get; set; } =
        {
            TauAction.RightButton,
            TauAction.LeftButton,
        };

        /// <summary>
        /// The action that caused this <see cref="DrawableHit"/> to be hit.
        /// </summary>
        protected TauAction? HitAction { get; set; }

        protected override double InitialLifetimeOffset => HitObject.TimePreempt;

        public Func<DrawableHitObject, double, bool> CheckHittable;

        protected override JudgementResult CreateResult(Judgement judgement) => new TauJudgementResult(HitObject, judgement);

        public void MissForcefully() => ApplyResult(r => r.Type = r.Judgement.MinResult);
    }
}
