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
using osuTK;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneTurbulenceKiai : OsuTestScene
    {
        private TurbulenceKiaiEffect kiaiContainer;

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
                    kiaiContainer = new TurbulenceKiaiEffect()
                }
            }));

            AddStep("Add turbulence", () =>
            {
                kiaiContainer.Vortices.Add(new Vortex
                {
                    Position = new Vector2(0, -((TauPlayfield.BaseSize.X / 2) + 105)),
                    Velocity = new Vector2(10),
                    Scale = 0.01f,
                    Speed = 5f,
                });
            });
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
