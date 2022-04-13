using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests.Objects
{
    [TestFixture]
    public class TestSceneBeat : TauTestScene
    {
        private int depthIndex;

        public TestSceneBeat()
        {
            TauPlayfieldAdjustmentContainer container;
            Add(container = new TauPlayfieldAdjustmentContainer());

            AddStep("Miss Single", () => container.Child = testSingle());
            AddStep("Hit Single", () => container.Child = testSingle(true));
            AddStep("Miss Stream", () => Add(testStream()));
            AddStep("Hit Stream", () => Add(testStream(true)));
            AddUntilStep("Wait for object despawn", () => !Children.Any(h => h is DrawableTauHitObject<Beat> { AllJudged: false }));
        }

        private Drawable testSingle(bool auto = false, double timeOffset = 0, float angle = 0)
        {
            var beat = new Beat
            {
                StartTime = Time.Current + 1000 + timeOffset,
                Angle = angle
            };

            beat.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            return new TestDrawableBeat(beat, auto)
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

        private class TestDrawableBeat : DrawableBeat
        {
            private readonly bool auto;

            public TestDrawableBeat(Beat h, bool auto)
                : base(h)
            {
                this.auto = auto;
            }

            protected override void CheckForResult(bool userTriggered, double timeOffset)
            {
                if (auto && !userTriggered && timeOffset > 0)
                    // Force success.
                    ApplyResult(r => r.Type = HitResult.Great);
                else if (timeOffset > 0)
                    // We'll have to manually apply the result anyways because we have no way of checking if the paddle is in the correct spot.
                    ApplyResult(r => r.Type = HitResult.Miss);
            }
        }
    }
}
