using System;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Configuration;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableTauHitObject<T> : DrawableHitObject<TauHitObject>, IKeyBindingHandler<TauAction>, ICanApplyResult
        where T : TauHitObject
    {
        public new T HitObject => (T)base.HitObject;

        protected DrawableTauHitObject(T obj)
            : base(obj)
        {
        }

        /// <summary>
        /// A list of <see cref="TauAction"/>s that denotes which keys can trigger this Hit object.
        /// </summary>
        protected virtual TauAction[] Actions { get; } =
        {
            TauAction.LeftButton,
            TauAction.RightButton
        };

        protected readonly BindableFloat NoteSize = new(16f);

        [BackgroundDependencyLoader(true)]
        private void load(TauRulesetConfigManager config)
        {
            config?.BindWith(TauRulesetSettings.NotesSize, NoteSize);
        }

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

            ApplyResult(r =>
            {
                r.Type = result;
                ApplyCustomResult(r);
            });
        }

        /// <summary>
        /// Should return whether or not a <see cref="DrawableHitObject"/> has the correct set of parameters for it to be hit.
        /// </summary>
        protected virtual bool CheckForValidation() => true;

        protected virtual void ApplyCustomResult(JudgementResult result) { }

        public virtual bool OnPressed(KeyBindingPressEvent<TauAction> e)
        {
            if (Judged)
                return false;

            return Actions.Contains(e.Action) && UpdateResult(true);
        }

        public virtual void OnReleased(KeyBindingReleaseEvent<TauAction> e)
        {
        }

        public void ForcefullyApplyResult(Action<JudgementResult> application)
            => ApplyResult(application);
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
