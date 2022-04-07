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
        private ClassicKiaiContainer kiaiContainer;
        private TauDependencyContainer dependencyContainer;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = base.CreateChildDependencies(parent);
            return dependencyContainer = new TauDependencyContainer(Beatmap.Value.Beatmap, dependencies);
        }

        [SetUpSteps]
        public void SetUp()
        {
            AddStep("Clear contents", Clear);
            AddStep("Add contents", () => Add(new TauPlayfieldAdjustmentContainer
            {
                Children = new Drawable[]
                {
                    new TauPlayfield(),
                    kiaiContainer = new ClassicKiaiContainer()
                }
            }));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestNormalBeats(bool isInversed)
        {
            AddStep("Set inverse effect", () =>
            {
                var properties = (TauCachedProperties)dependencyContainer.Get(typeof(TauCachedProperties));
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
