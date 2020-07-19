// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public abstract class DrawableTauHitObject : DrawableHitObject<TauHitObject>
    {
        protected DrawableTauHitObject(TauHitObject obj)
            : base(obj)
        {
        }

        public Func<DrawableTauHitObject, bool> CheckValidation;

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
    }
}
