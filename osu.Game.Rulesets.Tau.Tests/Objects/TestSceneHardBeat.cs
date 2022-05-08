using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI;

namespace osu.Game.Rulesets.Tau.Tests.Objects
{
    [TestFixture]
    public class TestSceneHardBeat : TauTestScene
    {
        private int depthIndex;

        public TestSceneHardBeat()
        {
            TauPlayfieldAdjustmentContainer container;
            Add(container = new TauPlayfieldAdjustmentContainer());

            AddStep("Miss single", () => container.Child = testSingle());
            AddStep("Hit single", () => container.Child = testSingle(true));
            AddUntilStep("Wait for object despawn", () => !Children.Any(h => h is DrawableHardBeat { AllJudged: false }));
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

        private class TestDrawableHardBeat : DrawableHardBeat
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
                    ApplyResult(r => r.Type = HitResult.Great);
                }
                else
                    base.CheckForResult(userTriggered, timeOffset);
            }
        }
    }
}
