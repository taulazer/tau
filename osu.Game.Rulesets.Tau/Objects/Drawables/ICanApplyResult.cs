using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public interface ICanApplyResult
    {
        /// <summary>
        /// Forces a result to the <see cref="DrawableHitObject{TObject}"/>.
        /// </summary>
        /// <param name="application">The adjustment to be made to the result.</param>
        public void ForcefullyApplyResult(HitResult result);
    }
}
