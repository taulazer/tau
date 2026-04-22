using System.Collections.Generic;
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
    public partial class TestSceneSliderHardBeat : TauTestScene
    {
        private int depthIndex;

        private TauPlayfield playfield;

        [Test]
        public void TestSingleSlider()
        {
            AddStep("clear screen", Clear);
            AddStep("add container", () => Add(new TauPlayfieldAdjustmentContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = playfield = new TauPlayfield()
            }));

            AddStep("Miss Single", () => playfield.Add(testSingle()));
            AddStep("Hit Single", () => playfield.Add(testSingle(true)));
            AddUntilStep("Wait for object despawn", () => !Children.Any(h => h is DrawableSlider { AllJudged: false }));
        }

        [Test]
        public void TestSliderPerformance()
        {
            AddStep("clear screen", Clear);
            AddStep("add container", () => Add(new TauPlayfieldAdjustmentContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = playfield = new TauPlayfield()
            }));

            AddStep("Miss Multiple", () =>
            {
                foreach (var slider in testMultiple(100))
                    playfield.Add(slider);
            });
            AddStep("Hit Multiple", () =>
            {
                foreach (var slider in testMultiple(100, true))
                    playfield.Add(slider);
            });
        }

        private IEnumerable<TestDrawableSlider> testMultiple(int count, bool auto = false)
            => Enumerable.Range(0, count).Select(x => testSingle(auto, 1000 + x * 100));

        private TestDrawableSlider testSingle(bool auto = false, double timeOffset = 1000)
        {
            var slider = createSlider(timeOffset);

            slider.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            return new TestDrawableSlider(slider, auto)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Depth = depthIndex++
            };
        }

        private Slider createSlider(double timeOffset)
            => new()
            {
                StartTime = Time.Current + timeOffset,
                IsHard = true,
                Path = new PolarSliderPath([
                    new SliderNode(0, 0),
                    new SliderNode(500, 90),
                    new SliderNode(1000, 180)
                ])
            };

        private partial class TestDrawableSlider : DrawableSlider
        {
            private readonly bool auto;

            public TestDrawableSlider(Slider h, bool auto)
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
