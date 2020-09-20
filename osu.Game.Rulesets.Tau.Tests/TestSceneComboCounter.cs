using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Judgements;
using osu.Game.Rulesets.Tau.UI.Combo;
using osu.Game.Tests.Visual;
using osuTK;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneComboCounter : OsuTestScene
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            var counter = new ComboCounter(CreateBeatmap(new TauRuleset().RulesetInfo))
            {
                Size = new Vector2(0.6f),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };

            Add(counter);

            AddStep("Hit note", () => counter.AddResult(createJudgement(r => r.Type = HitResult.Great)));
            AddStep("Miss note", () => counter.AddResult(createJudgement(r => r.Type = HitResult.Miss)));
        }

        private JudgementResult createJudgement(Action<JudgementResult> application)
        {
            var result = new JudgementResult(null, new TauJudgement());

            application?.Invoke(result);

            return result;
        }
    }
}
