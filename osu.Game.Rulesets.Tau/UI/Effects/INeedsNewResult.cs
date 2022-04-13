using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Tau.UI.Effects
{
    public interface INeedsNewResult
    {
        public void OnNewResult(DrawableHitObject judgedObject, JudgementResult result);
    }
}
