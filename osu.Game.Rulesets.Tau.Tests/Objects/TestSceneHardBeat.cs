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
    public partial class TestSceneHardBeat : TauTestScene
    {
        private int depthIndex;

        private TauPlayfieldAdjustmentContainer container;
        private BindableBool highlightHardNotes = new BindableBool();

        [Test]
        public void TestHardBeat()
        {
            AddStep("clear screen", Clear);
            AddStep("add container", () => Add(container = new TauPlayfieldAdjustmentContainer()));

            AddStep("Highlighting disabled", () => highlightHardNotes.Value = false);
            AddStep("Miss single", () => container.Child = testSingle());
            AddStep("Hit single", () => container.Child = testSingle(true));
            AddUntilStep("Wait for object despawn", () => !Children.Any(h => h is DrawableHardBeat { AllJudged: false }));
        }

        [Test]
        public void TestHighlightedHardBeat()
        {
            AddStep("clear screen", Clear);
            AddStep("add container", () => Add(container = new TauPlayfieldAdjustmentContainer()));

            AddStep("Highlighting enabled", () => highlightHardNotes.Value = true);
            AddStep("Miss single highlighted", () => container.Child = testSingle());
            AddStep("Hit single highlighted", () => container.Child = testSingle(true));
            AddUntilStep("Wait for object despawn", () => !Children.Any(h => h is DrawableHardBeat { AllJudged: false }));
        }

        [BackgroundDependencyLoader]
        private void load() {
            var config = (TauRulesetConfigManager)RulesetConfigs.GetConfigFor(Ruleset.Value.CreateInstance()).AsNonNull();
            config.BindWith(TauRulesetSettings.HighlightHardBeats, highlightHardNotes);
        }

        private Drawable testSingle(bool auto = false)
        {
            var circle = new HardBeat
            {
                StartTime = Time.Current + 1000,
            };

            circle.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            return new TestDrawableHardBeat(circle, auto)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Depth = depthIndex++,
            };
        }

        private partial class TestDrawableHardBeat : DrawableHardBeat
        {
            private readonly bool auto;

            public TestDrawableHardBeat(HardBeat h, bool auto)
                : base(h)
            {
                this.auto = auto;
            }

            protected override void CheckForResult(bool userTriggered, double timeOffset)
            {
                if (auto && !userTriggered && timeOffset > 0)
                {
                    // force success
                    ApplyResult(HitResult.Great);
                }
                else
                    base.CheckForResult(userTriggered, timeOffset);
            }
        }
    }
}
