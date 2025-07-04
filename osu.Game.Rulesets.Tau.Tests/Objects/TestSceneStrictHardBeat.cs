using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI;

namespace osu.Game.Rulesets.Tau.Tests.Objects
{
    [TestFixture]
    public partial class TestSceneStrictHardBeat : TauTestScene
    {
        private int depthIndex;

        private TauPlayfieldAdjustmentContainer container;
        private BindableBool increaseVisualDistinction = new BindableBool();

        [BackgroundDependencyLoader]
        private void load() {
            var config = (TauRulesetConfigManager)RulesetConfigs.GetConfigFor(Ruleset.Value.CreateInstance()).AsNonNull();
            config.BindWith(TauRulesetSettings.IncreaseVisualDistinction, increaseVisualDistinction);
        }

        [Test]
        public void TestStrictHardBeat()
        {
            AddStep("clear screen", Clear);
            AddStep("add container", () => Add(container = new TauPlayfieldAdjustmentContainer()));
            AddStep("visual distinction disabled", () => increaseVisualDistinction.Value = false);

            AddStep("Miss Single", () => container.Add(testSingle()));
            AddStep("Hit Single", () => container.Add(testSingle(true)));
            AddStep("Miss Stream", () => Add(testStream()));
            AddStep("Hit Stream", () => Add(testStream(true)));
            AddUntilStep("Wait for object despawn", () => !container.Any(h => h is DrawableTauHitObject<StrictHardBeat> { AllJudged: false }));
        }

        [Test]
        public void TestVisuallyDistinctStrictHardBeat()
        {
            AddStep("clear screen", Clear);
            AddStep("add container", () => Add(container = new TauPlayfieldAdjustmentContainer()));
            AddStep("visual distinction enabled", () => increaseVisualDistinction.Value = true);

            AddStep("Miss Single", () => container.Add(testSingle()));
            AddStep("Hit Single", () => container.Add(testSingle(true)));
            AddStep("Miss Stream", () => Add(testStream()));
            AddStep("Hit Stream", () => Add(testStream(true)));
            AddUntilStep("Wait for object despawn", () => !container.Any(h => h is DrawableTauHitObject<StrictHardBeat> { AllJudged: false }));
        }

        private Drawable testSingle(bool auto = false, double timeOffset = 0, float angle = 0)
        {
            var strict = new StrictHardBeat
            {
                StartTime = Time.Current + 1000 + timeOffset,
                Angle = angle
            };

            strict.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            return new TestDrawableStrictHardBeat(strict, auto)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Depth = depthIndex++
            };
        }

        private Drawable testStream(bool auto = false)
        {
            var playfield = new TauPlayfieldAdjustmentContainer();

            for (int i = 0; i <= 1000; i += 100)
            {
                playfield.Add(testSingle(auto, i, i / 10f));
            }

            return playfield;
        }

        private partial class TestDrawableStrictHardBeat : DrawableStrictHardBeat
        {
            private readonly bool auto;

            public TestDrawableStrictHardBeat(StrictHardBeat h, bool auto)
                : base(h)
            {
                this.auto = auto;
            }

            protected override void CheckForResult(bool userTriggered, double timeOffset)
            {
                if (auto && !userTriggered && timeOffset > 0)
                    // Force success.
                    ApplyResult(HitResult.Great);
                else if (timeOffset > 0)
                    // We'll have to manually apply the result anyways because we have no way of checking if the paddle is in the correct spot.
                    ApplyResult(HitResult.Miss);
            }
        }
    }
}
