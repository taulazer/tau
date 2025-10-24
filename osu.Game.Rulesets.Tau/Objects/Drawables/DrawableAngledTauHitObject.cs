using System;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Tau.Judgements;
using osu.Game.Rulesets.Tau.UI.Cursor;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    /// <summary>
    /// Abstracted class to handle any angular hit objects. e.g. <see cref="DrawableBeat"/> or <see cref="DrawableSlider"/>.
    /// </summary>
    public abstract partial class DrawableAngledTauHitObject<T> : DrawableTauHitObject<T>
        where T : AngledTauHitObject
    {
        /// <summary>
        /// Proxy method to the playfield's cursor to validate the angle.
        /// </summary>
        /// <param name="angle">The absolute angle to validate (normalized).</param>
        /// <returns>The validation result, including the delta from the paddle's center.</returns>
        protected virtual Paddle.AngleValidationResult ValidateAngle(float angle)
            => Playfield?.Cursor.ValidateAngle(angle) ?? throw new NullReferenceException($"{nameof(Playfield)} is null.");

        private readonly BindableFloat angleBindable = new();

        protected DrawableAngledTauHitObject(T obj)
            : base(obj)
        {
            angleBindable.BindValueChanged(r => Rotation = r.NewValue);
        }

        protected override void OnApply()
        {
            base.OnApply();
            angleBindable.BindTo(HitObject.AngleBindable);
        }

        protected override void OnFree()
        {
            base.OnFree();
            angleBindable.UnbindFrom(HitObject.AngleBindable);
        }

        protected override JudgementResult CreateResult(Judgement judgement)
            => new TauJudgementResult(HitObject, judgement);

        protected override bool CheckForValidation() => IsWithinPaddle();

        public bool IsWithinPaddle() => ValidateAngle(GetCurrentAngle()).IsValid;

        public override void ApplyCustomResult(JudgementResult result)
        {
            float delta = ValidateAngle(GetCurrentAngle()).Delta;
            var beatResult = (TauJudgementResult)result;

            if (result.IsHit)
                beatResult.DeltaAngle = delta;
        }

        /// <summary>
        /// The offset to apply when checking if the <see cref="DrawableAngledTauHitObject{T}"/> is in the paddle.
        /// </summary>
        protected virtual float GetCurrentOffset() => 0f;

        /// <summary>
        /// The current angle the <see cref="DrawableAngledTauHitObject{T}"/> is at in relation to the playfield.
        /// </summary>
        /// <returns></returns>
        protected virtual float GetCurrentAngle() => (HitObject.Angle + GetCurrentOffset()).Normalize();
    }
}
