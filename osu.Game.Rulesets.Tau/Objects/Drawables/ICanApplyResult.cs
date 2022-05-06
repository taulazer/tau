using System;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public interface ICanApplyResult
    {
        /// <summary>
        /// Forces a result to the <see cref="DrawableHitObject{TObject}"/>.
        /// </summary>
        /// <param name="application">The adjustment to be made to the result.</param>
        public void ForcefullyApplyResult(Action<JudgementResult> application);
    }
}
