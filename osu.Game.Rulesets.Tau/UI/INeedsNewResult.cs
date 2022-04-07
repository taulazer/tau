using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Tau.UI
{
    public interface INeedsNewResult
    {
        public void OnNewResult(DrawableHitObject judgedObject, JudgementResult result);
    }
}
