using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Judgements;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.Tau.UI.Effects;

namespace osu.Game.Rulesets.Tau.Tests
{
    [ExcludeFromDynamicCompile]
    public abstract class TestSceneKiaiEffects<T, TEmitter> : TauTestScene
        where T : KiaiEffect<TEmitter>, new()
        where TEmitter : Emitter, new()
    {
        protected T KiaiContainer;

        protected virtual void AddExtraSetupSteps()
        {
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
                    KiaiContainer = new T()
                }
            }));

            AddExtraSetupSteps();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestEffect(bool isInversed)
        {
            AddStep("Set inverse effect", () =>
            {
                Properties.InverseModEnabled.Value = isInversed;
            });

            AddStep("Add beat result",
                () => KiaiContainer.OnNewResult(
                    new DrawableBeat(new Beat()), new JudgementResult(new Beat(), new TauJudgement()) { Type = HitResult.Great }));

            AddStep("Add hard beat result",
                () => KiaiContainer.OnNewResult(
                    new DrawableHardBeat(new HardBeat()), new JudgementResult(new HardBeat(), new TauJudgement()) { Type = HitResult.Great }));
        }
    }
}
