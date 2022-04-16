using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Judgements;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.Tau.UI.Effects;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneClassicKiai : OsuTestScene
    {
        private ClassicKiaiEffect kiaiContainer;

        [Cached]
        private TauCachedProperties properties { get; set; } = new();

        [SetUpSteps]
        public void SetUp()
        {
            AddStep("Clear contents", Clear);
            AddStep("Add contents", () => Add(new TauPlayfieldAdjustmentContainer
            {
                Children = new Drawable[]
                {
                    new TauPlayfield(),
                    kiaiContainer = new ClassicKiaiEffect()
                }
            }));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestEffect(bool isInversed)
        {
            AddStep("Set inverse effect", () =>
            {
                properties.InverseModEnabled.Value = isInversed;
            });

            AddStep("Add beat result",
                () => kiaiContainer.OnNewResult(
                    new DrawableBeat(new Beat()), new JudgementResult(new Beat(), new TauJudgement()) { Type = HitResult.Great }));

            AddStep("Add hard beat result",
                () => kiaiContainer.OnNewResult(
                    new DrawableHardBeat(new HardBeat()), new JudgementResult(new HardBeat(), new TauJudgement()) { Type = HitResult.Great }));
        }
    }
}
