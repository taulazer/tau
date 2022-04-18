using System;
using osu.Game.Rulesets.Judgements;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public interface ICanApplyResult
    {
        public void ForcefullyApplyResult(Action<JudgementResult> application);
    }
}
